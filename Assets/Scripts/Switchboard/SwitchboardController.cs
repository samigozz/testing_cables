using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchboardController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private float mouseDragSpeed = 10.0f;

    [Header("Plug And Jack")] 
    [SerializeField] private Transform jacksParent;
    [SerializeField] private float maxDistanceFromJack = 1.5f;
    
    [Header("Obi Settings")]
    [SerializeField] private ObiSolver solver;
    
    private Camera _camera;
    
    private List<JackSlot> _jackSlots;
    public List<JackSlot> GetAllJacks => _jackSlots;
    
    private JackSlot _closestJack;
    private PlugCord _selectedPlug;

    private int _framesSinceLastContact = 0;
    
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        _camera = Camera.main;
        
        if (jacksParent != null)
            _jackSlots = new List<JackSlot>(jacksParent.GetComponentsInChildren<JackSlot>());
    }

    private void OnEnable()
    {
        if(solver)
            solver.OnParticleCollision += Solver_OnParticleCollision;
    }

    private void OnDisable()
    {
        if(solver)
            solver.OnParticleCollision -= Solver_OnParticleCollision;
    }

    // ---------- INPUT EVENTS ---------- //
    public void RightMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit)) 
                return;

            if (!hit.collider)
                return;
            
            if (hit.collider.gameObject.TryGetComponent<PlugCord>(out var plug))
            {
                _selectedPlug = plug;
                _selectedPlug.DetachFromCurrentJack();
                //_selectedPlug.rb.useGravity = false;
                StartCoroutine(DragUpdate(_selectedPlug, context));
            }
        }
        else if (context.canceled)
        {
            DragRelease();
        }
    }
    
    private IEnumerator DragUpdate(PlugCord plug, InputAction.CallbackContext context)
    {
        float initDistance = Vector3.Distance(plug.transform.position, _camera.transform.position);
        plug.TryGetComponent<Rigidbody>(out var rb);
        while (context.ReadValue<float>() > 0f)
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!rb)
                yield return null;
            
            //Moving with velocity is the best approach in this case for rb.
            Vector2 direction = ray.GetPoint(initDistance) - plug.transform.position;
            rb.linearVelocity = direction * mouseDragSpeed;
            
            //_selectedPlug.UpdateCordLength();
            
            //Find the closest available jack and make a visual indication that it's free.
            _closestJack = FindOpenJack(_selectedPlug);
            if (_closestJack)
                _closestJack.Tint();
            
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
            //_selectedPlug.DetachFromCurrentJack();
            _selectedPlug.AttachToJack(_closestJack);
            _closestJack.ResetColor();
        }
        else
        {
            //Return to initial position if mouse button released
            //_selectedPlug.transform.localPosition = new Vector3(0, 1, 0);
            //_selectedPlug.transform.localRotation = Quaternion.identity;

            var rb = _selectedPlug.GetComponent<Rigidbody>();
            
            if (!rb)
                return;
            
            /*rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;*/
            
            _selectedPlug.DetachFromCurrentJack();
        }
        _selectedPlug = null;
    }
    
    private JackSlot FindOpenJack(PlugCord plug)
    {
        JackSlot closestJack = null;
        var closestDistance = float.MaxValue;

        foreach (var jack in _jackSlots)
        {
            jack.ResetColor();
            
            if (jack.currentPlug != null)
                continue;
            
            var distance = Vector3.Distance(plug.transform.position, jack.transform.position);
            if (distance < closestDistance && distance < maxDistanceFromJack)
            {
                closestJack = jack;
                closestDistance = distance;
            }
        }

        return closestJack;
    }
    
    /// <summary>
    /// Handles collision between two obi ropes.
    /// </summary>
    private void Solver_OnParticleCollision(ObiSolver s, ObiNativeContactList e)
    {
        // Count contacts between different ropes (that is, exclude self-contacts):
        int contactsBetweenRopes = 0;

        for (int i = 0; i < e.count; ++i)
        {
            var ropeA = s.particleToActor[s.simplices[e[i].bodyA]].actor;
            var ropeB = s.particleToActor[s.simplices[e[i].bodyB]].actor;

            if (ropeA != ropeB)
                contactsBetweenRopes++;
        }

        // If there's no contacts, bump the amount of frames we've been contact-free.
        // Otherwise, reset the amount of frames to zero.
        if (contactsBetweenRopes == 0)
            _framesSinceLastContact++;
        else
            _framesSinceLastContact = 0;
    }
}
