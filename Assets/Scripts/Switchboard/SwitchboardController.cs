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
    
    [Header("Input")] 
    [SerializeField] private InputActionReference rightClickAction;
    
    private Camera _camera;
    
    private JackSlot _closestJack;
    private IDrag _draggedObj;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        rightClickAction.action.performed += OnMouseClick;
        rightClickAction.action.canceled += OnMouseClick;
        rightClickAction.action.Enable();
    }
    
    private void OnDisable()
    {
        rightClickAction.action.performed -= OnMouseClick;
        rightClickAction.action.canceled -= OnMouseClick;
        rightClickAction.action.Disable();
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit)) 
                return;

            if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }

            if (hit.collider.gameObject.TryGetComponent<IDrag>(out var draggable))
            {
                _draggedObj = draggable;
                draggable.OnDragStart();
            }
        }
        else if (context.canceled)
        {
            if (_draggedObj == null)
                return;
            
            _draggedObj.OnDragRelease();
            _draggedObj = null;
        }
    }

}
