using System;
using UnityEngine;

public class Jack : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.yellow;
    
    [HideInInspector] public PlugCord currentPlug;
    public bool IsOccupied => currentPlug != null;
    public string SocketID { get; set; }
        
    private Material _instance;
    private Color _defaultColor;

    private void Awake()
    {
        _instance = GetComponent<Renderer>().material;
        _defaultColor = _instance.color;
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
}
