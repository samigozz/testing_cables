using System;
using System.Collections;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiRigidbody), typeof(Rigidbody))]
public class PlugCord : MonoBehaviour
{
    [SerializeField] private Vector3 dockOffset;

    [Header("Cord Settings")] 
    [SerializeField] private float maxLength = 10.0f;
    [SerializeField] private float minLength = 1.0f;
    [SerializeField] private float growthThreshold = 0.1f;
    [SerializeField] private float growthSpeed = 1.0f;
    
    public JackSlot currentJack;

    private Vector3 _originPos;
    private Vector3 _prevPos;

    private ObiRope _rope;
    private ObiRopeCursor cursor;
    
    [HideInInspector] public Rigidbody rb;
    private ObiRigidbody _orb;

    private void Awake()
    {
        _rope = GetComponentInParent<ObiRope>();
        cursor = _rope.GetComponent<ObiRopeCursor>();
        
        rb = GetComponent<Rigidbody>();
        _orb = GetComponent<ObiRigidbody>();
    }

    private void Start()
    {
        _originPos = transform.position;

        //rb.useGravity = true;
        
        if (currentJack == null) 
            return;
        
        currentJack.currentPlug = this;
        transform.position = currentJack.transform.position;
    }

    public void AttachToJack(JackSlot jack)
    {
        currentJack = jack;
        currentJack.currentPlug = this;
        
        transform.position = jack.transform.position + dockOffset;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        
        rb.isKinematic = true;
        _orb.kinematicForParticles = false;
    }
    
    public void DetachFromCurrentJack()
    {
        if (!currentJack)
            return;
        
        rb.isKinematic = false;
        _orb.kinematicForParticles = true;
        
        //rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        
        currentJack.currentPlug = null;
        currentJack = null;
    }
    
    /// <summary>
    /// Deprecated: previous version of rope
    /// </summary>
    public void UpdateCordLength()
    {
        var length = _rope.CalculateLength();
        if (length >= maxLength ||  length <= minLength)
            return;

        if (rb.linearVelocity.magnitude < growthThreshold)
            return;
        
        var dir = (_originPos - transform.position).normalized;
        
        var dot = Vector3.Dot(dir, rb.linearVelocity.normalized);

        var isMovingToOrigin = dot > 0.1f;
        
        Debug.Log(isMovingToOrigin);

        if (isMovingToOrigin)
        {
            cursor.ChangeLength(-growthSpeed * Time.deltaTime);
        }
        else
        {
            cursor.ChangeLength(growthSpeed * Time.deltaTime);
        }
    }

    private IEnumerator MoveToSlot(JackSlot jack)
    {
        throw new NotImplementedException();
    }
}
