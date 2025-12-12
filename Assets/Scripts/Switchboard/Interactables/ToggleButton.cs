using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ToggleButton : MonoBehaviour, IInteractable
{
    [Header("Button Settings")] 
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.black;
    [SerializeField] private GameObject movingPart;
    
    [Header("Toggle Animation")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 destination = new (0, 0.2f, 0);
    [SerializeField] private AnimationCurve toggleCurve;
    [SerializeField, Range(0,1), Tooltip("The time at the duration of the tween that triggers the ON/OFF color, ranged from 0 to 1.")] 
    private float colorTriggerTime = 0.35f;
    
    private bool _isOn;
    private Vector3 _startPos;
    
    private MeshRenderer _renderer;
    
    // Material property blocks.
    private MaterialPropertyBlock _matPropertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

    private void Start()
    {
        _matPropertyBlock = new MaterialPropertyBlock();
        _renderer = movingPart.GetComponent<MeshRenderer>();
        
        UpdateColor();
        _startPos = movingPart.transform.localPosition;
    }

    private void OnValidate()
    {
        if(_matPropertyBlock == null)
            _matPropertyBlock = new MaterialPropertyBlock();
        
        if(_renderer == null)
            _renderer = movingPart.GetComponent<MeshRenderer>();
        
        UpdateColor();
    }

    private void UpdateColor()
    {
        var newColor = _isOn ? onColor : offColor;
        _matPropertyBlock.SetColor(ColorProperty, newColor);
        _renderer.SetPropertyBlock(_matPropertyBlock);
    }
    
    public void Interact()
    {
        StartCoroutine(PlayAnimation());
        _isOn = !_isOn;
    }

    private IEnumerator PlayAnimation()
    {
        Tweener tween = null;
        
        if (_isOn)
        {
            //Move back to OFF position.
            tween = movingPart.transform.DOLocalMove(_startPos, duration - 0.05f).SetEase(ReverseCurve(toggleCurve));
        }
        else
        {
            //Move to ON position
            tween = movingPart.transform.DOLocalMove(destination, duration).SetEase(toggleCurve);
        }
        
        yield return tween.WaitForPosition(colorTriggerTime);
        
        UpdateColor();
    }

    private static AnimationCurve ReverseCurve(AnimationCurve curve)
    {
        var reversedCurve = new AnimationCurve();

        foreach (var key in curve.keys)
        {
            reversedCurve.AddKey(new Keyframe(1 - key.time, 1 - key.value));
        }
        
        return reversedCurve;
    }
}
