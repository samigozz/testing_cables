using System;
using UnityEngine;

public class JackSlot : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.yellow;
    
    public PlugCord currentPlug;

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
