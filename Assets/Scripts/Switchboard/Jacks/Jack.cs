using UnityEngine;

public class Jack : MonoBehaviour
{
    [Header("Jack Default Settings")]
    [SerializeField] private Color highlightColor = Color.yellow;
    
    [Header("Light")]
    [SerializeField] protected LightIndicator lightIndicator;
    [SerializeField, ColorUsage(false, true)] protected Color answeredColor = Color.yellow;
    [SerializeField, ColorUsage(false, true)] protected Color connectedColor = Color.green;
    [SerializeField, ColorUsage(false, true)] protected Color offColor = Color.black;
    
    [HideInInspector] public PlugCord currentPlug;
    
    public string SocketID { get; set; }
        
    private Material _instance;
    private Color _defaultColor;

    private void Awake()
    {
        _instance = GetComponent<Renderer>().material;
        _defaultColor = _instance.color;
    }

    private void Start()
    {
        lightIndicator.UpdateLight(offColor);
    }

    public void Tint()
    {
        _instance.color = highlightColor;
    }

    public void ResetColor()
    {
        _instance.color = _defaultColor;
    }

    private void OnDestroy()
    {
        Destroy(_instance);
    }
    
    public void OnAnswerCall()
    {
        lightIndicator.UpdateLight(answeredColor);
    }

    public void OnConnectCall()
    {
        lightIndicator.UpdateLight(answeredColor);
    }
    
    public void OnDisconnectCall()
    {
        lightIndicator.UpdateLight(offColor);
    }
    
}
