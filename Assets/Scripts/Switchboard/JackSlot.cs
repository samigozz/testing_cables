using System;
using UnityEngine;

public class JackSlot : MonoBehaviour
{
    [SerializeField] private Color tintColor;
    
    public PlugCord currentPlug;

    private Material _instance;
    private Color _deafaultColor;

    private void Awake()
    {
        _instance = GetComponent<Renderer>().material;
        _deafaultColor = _instance.color;
    }

    public void Tint()
    {
        _instance.color = tintColor;
    }

    public void ResetColor()
    {
        _instance.color = _deafaultColor;
    }

    private void OnDestroy()
    {
        Destroy(_instance);
    }
}
