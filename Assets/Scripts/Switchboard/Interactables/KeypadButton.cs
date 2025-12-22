using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class KeypadButton : MonoBehaviour, IInteractable
{
    [SerializeField] private float pressAmount = 0.2f;
    [SerializeField] private UnityEvent onPress = new UnityEvent();
    
    private bool _isPlaying;

    public void Interact()
    {
        onPress.Invoke();
        
        if (_isPlaying)
            return;
        
        transform.DOLocalMoveZ(pressAmount, 0.1f).SetRelative(true).SetEase(Ease.InOutQuad)
            .SetLoops(2, LoopType.Yoyo).OnStart(() => { _isPlaying = true; }).OnComplete(() => { _isPlaying = false; });
    }
}
