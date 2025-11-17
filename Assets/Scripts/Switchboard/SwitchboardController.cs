using System;
using System.Collections;
using Obi;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchboardController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float mouseDragSpeed = 10.0f;
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private LayerMask jackLayerMask;

    [Header("Plug And Jack")] 
    [SerializeField] private Transform jacksParent;
    [SerializeField] private float maxDistanceFromJack = 1.5f;
    
    [Header("Obi Settings")]
    [SerializeField] private ObiSolver solver;
    
    private Camera _camera;

    [Header("Input")] [SerializeField] private InputActionReference rightClickAction;
    
    private JackSlot _closestJack;
    private PlugCord _selectedPlug;

    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        rightClickAction.action.performed += RightMouseClick;
        rightClickAction.action.Enable();
    }
    
    private void OnDisable()
    {
        rightClickAction.action.performed -= RightMouseClick;
        rightClickAction.action.Disable();
    }

    // ---------- INPUT EVENTS ---------- //
    private void RightMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit)) 
                return;

            if (!hit.collider)
                return;

            if (!hit.collider.gameObject.TryGetComponent<PlugCord>(out var plug)) 
                return;
            
            _selectedPlug = plug;
            _selectedPlug.DetachFromCurrentJack();
            StartCoroutine(DragUpdate(_selectedPlug, context));
        }
        else if (context.canceled)
        {
            DragRelease();
        }
    }
    // -------------------------------- //
    
    private IEnumerator DragUpdate(PlugCord plug, InputAction.CallbackContext context)
    {
        while (context.ReadValue<float>() > 0f)
        {
            // Attach the plug to the current mouse position.
            Vector3 screenPos = Mouse.current.position.ReadValue();
            var z = _camera.WorldToScreenPoint(_selectedPlug.transform.position).z;
            var worldPos = _camera.ScreenToWorldPoint(screenPos + new Vector3(0, 0, z));
            
            _selectedPlug.MoveTowards(worldPos + dragOffset);
            
            _closestJack = FindOpenJack(_selectedPlug);
            
            yield return _waitForFixedUpdate;
        }
    }

    private void DragRelease()
    {
        if (!_selectedPlug)
            return;
        
        StopAllCoroutines();
                
        if (_closestJack != null)
        {
            _selectedPlug.currentJack = null;
            _selectedPlug.AttachToJack(_closestJack);
            
            _closestJack.ResetColor();
            _closestJack = null;
        }
        else
        {
            _selectedPlug.DetachFromCurrentJack();
        }
        _selectedPlug = null;
    }
    
    private JackSlot FindOpenJack(PlugCord plug)
    {
        JackSlot closestJack = null;
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, 100, jackLayerMask))
        {
            if (!hit.collider)
                return null;
            
            if (!hit.collider.gameObject.TryGetComponent<JackSlot>(out var jack)) 
                return null;
            
            _closestJack?.ResetColor();
            closestJack = jack;
            closestJack.Tint();
        }
        else
        {
            _closestJack?.ResetColor();
        }

        return closestJack;
    }
}
