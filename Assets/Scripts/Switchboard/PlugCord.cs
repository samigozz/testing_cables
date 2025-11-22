using UnityEngine;
using System.Collections;
using Obi;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ObiRigidbody), typeof(Rigidbody))]
public class PlugCord : MonoBehaviour, IDrag
{
    [SerializeField] private Vector3 dockOffset;

    [Header("Cord Settings")] 
    [SerializeField] private float maxLength = 10.0f;
    [SerializeField] private float minLength = 1.0f;
    [SerializeField] private float growthThreshold = 0.1f;
    [SerializeField] private float growthSpeed = 1.0f;
    [SerializeField] private LayerMask jackLayerMask;
    
    [Header("Movement")]
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private float stiffness = 200.0f;
    [SerializeField] private float damping = 20.0f;
    [SerializeField] private float maxAccel = 50.0f;
    [SerializeField] private float minDistance = 0.05f;
    
    private Camera _camera;
    
    [HideInInspector] public JackSlot currentJack;
    private JackSlot _closestJack;

    private Vector3 _prevPos;

    private bool _isDragging = false;
    
    [HideInInspector] public Rigidbody rb;
    private ObiRigidbody _orb;
    
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

    private void Awake()
    {
        _camera = Camera.main;
        rb = GetComponent<Rigidbody>();
        _orb = GetComponent<ObiRigidbody>();
    }

    private void Start()
    {
        if (currentJack == null) 
            return;
        
        currentJack.currentPlug = this;
        transform.position = currentJack.transform.position;
    }

    public void MoveTowards(Vector3 pos)
    {
        var dir = pos - transform.position;
        
        //damped spring
        var accel = stiffness * dir - damping * rb.linearVelocity;
        
        //clamp spring accelerations
        accel = Vector3.ClampMagnitude(accel, maxAccel);
        
        rb.AddForce(accel, ForceMode.Acceleration);
    }

    public void OnDragStart()
    {
        _isDragging = true;
        DetachFromJack();
        StartCoroutine(OnDragUpdate());
    }

    public IEnumerator OnDragUpdate()
    {
        while (_isDragging)
        {
            // Attach the plug to the current mouse position.
            Vector3 screenPos = Mouse.current.position.ReadValue();
            var z = _camera.WorldToScreenPoint(transform.position).z;
            var worldPos = _camera.ScreenToWorldPoint(screenPos + new Vector3(0, 0, z));
            
            MoveTowards(worldPos + dragOffset);
            
            _closestJack = FindOpenJack();
            
            yield return _waitForFixedUpdate;
        } 
    }

    public void OnDragRelease()
    {
        _isDragging = false;
        StopCoroutine(OnDragUpdate());
        
        if (_closestJack != null)
        {
            currentJack = null;
            AttachToJack(_closestJack);
            
            _closestJack.ResetColor();
            _closestJack = null;
        }
        else
        {
            DetachFromJack();
        }
    }
    
    private JackSlot FindOpenJack()
    {
        JackSlot closestJack = null;
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, 100, jackLayerMask))
        {
            if (!hit.collider.gameObject.TryGetComponent<JackSlot>(out var jack)) 
                return null;

            if (jack.currentPlug)
                return null;
            
            closestJack = jack;
            closestJack.Tint();
        }
        else
        {
            _closestJack?.ResetColor();
        }

        return closestJack;
    }
    
    private void AttachToJack(JackSlot jack)
    {
        currentJack = jack;
        currentJack.currentPlug = this;
        
        transform.position = jack.transform.position + dockOffset;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        
        rb.isKinematic = true;
        _orb.kinematicForParticles = false;
    }
    
    private void DetachFromJack()
    {
        if (!currentJack)
            return;
        
        rb.isKinematic = false;
        transform.rotation = Quaternion.identity;
        
        currentJack.currentPlug = null;
        currentJack = null;
    }
}
