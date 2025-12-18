using System;
using UnityEngine;
using System.Collections;
using Obi;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ObiRigidbody), typeof(Rigidbody))]
public class PlugCord : MonoBehaviour, IDrag
{
    [Header("Cord Settings")] 
    [SerializeField] private Vector3 dockOffset;
    [SerializeField] private LayerMask jackLayerMask;
    public bool isAnsweringCord = false;
    
    [Header("Movement")]
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private float stiffness = 200.0f;
    [SerializeField] private float damping = 20.0f;
    [SerializeField] private float maxAccel = 50.0f;

    private bool _isDragging;

    private Vector3 _startPos;
    
    private Camera _camera;
    private Rigidbody _rb;
    
    private Jack _closestJack;
    private Jack _currentJack;
    public Jack GetCurrentJack => _currentJack;
    
    public bool IsConnected => _currentJack != null;
    
    // OBI
    private ObiRigidbody _orb;

    public event Action<PlugCord> OnPlugged;
    public event Action<PlugCord> OnUnplugged;
    
    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

    private void Awake()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _orb = GetComponent<ObiRigidbody>();
    }

    private void Start()
    {
        _startPos = transform.position;
        
        if (_currentJack != null)
        {
            _currentJack.currentPlug = this;
            transform.position = _currentJack.transform.position;
        }
    }


    public void OnDragStart()
    {
        _isDragging = true;
        DetachFromJack();
        StartCoroutine(OnDragUpdate());
    }

    public IEnumerator OnDragUpdate()
    {
        var dragPlane = new Plane(Vector3.forward, _startPos);
        while (_isDragging)
        {
            // Attach the plug to the current mouse position.
            Vector3 screenPos = Mouse.current.position.ReadValue();
            //var worldPos = _camera.ScreenToWorldPoint(screenPos);
            var ray = _camera.ScreenPointToRay(screenPos);
            
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            if (dragPlane.Raycast(ray, out var point))
            {
                var worldPos = ray.GetPoint(point);
                MoveTowards(worldPos +  dragOffset);
            }
            
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
            _currentJack = null;
            AttachToJack(_closestJack);
            
            _closestJack.ResetColor();
            _closestJack = null;
        }
        else
        {
            DetachFromJack();
            //TODO: Reset rope and plug to origin state so it prevents unwanted behaviour like clipping or
            //losing the visibility to interact with it.
        }
    }
    
    private void MoveTowards(Vector3 pos)
    {
        var dir = pos - transform.position;
        
        //damped spring
        var accel = stiffness * dir - damping * _rb.linearVelocity;
        
        //clamp spring accelerations
        accel = Vector3.ClampMagnitude(accel, maxAccel);
        
        _rb.AddForce(accel, ForceMode.Acceleration);
    }
    
    private Jack FindOpenJack()
    {
        Jack closestJack = null;
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, 100, jackLayerMask))
        {
            if (!hit.collider.gameObject.TryGetComponent<Jack>(out var jack)) 
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
    
    private void AttachToJack(Jack jack)
    {
        _currentJack = jack;
        _currentJack.currentPlug = this;
        
        OnPlugged?.Invoke(this);
        
        transform.position = jack.transform.position + dockOffset;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        
        _rb.isKinematic = true;
        _orb.kinematicForParticles = false;
    }
    
    private void DetachFromJack()
    {
        if (!_currentJack)
            return;
        
        OnUnplugged?.Invoke(this);
        
        _rb.isKinematic = false;
        transform.rotation = Quaternion.identity;
        
        _currentJack.currentPlug = null;
        _currentJack = null;
    }
}
