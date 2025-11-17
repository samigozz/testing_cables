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
    
    [Header("Movement")]
    [SerializeField] private float stiffness = 200.0f;
    [SerializeField] private float damping = 20.0f;
    [SerializeField] private float maxAccel = 50.0f;
    [SerializeField] private float minDistance = 0.05f;
    
    public JackSlot currentJack;

    private Vector3 _prevPos;
    
    [HideInInspector] public Rigidbody rb;
    private ObiRigidbody _orb;

    private void Awake()
    {
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
        transform.rotation = Quaternion.identity;
        
        currentJack.currentPlug = null;
        currentJack = null;
    }
}
