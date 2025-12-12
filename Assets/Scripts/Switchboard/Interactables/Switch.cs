using System;
using DG.Tweening;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform pivotPoint;
    private bool _isToggled = false;

    public void Interact()
    {
        if (!pivotPoint)
            return;
        
        if(_isToggled)
            pivotPoint.transform.DOLocalRotate(new Vector3(0, 0, -45), 0.2f).SetEase(Ease.OutSine);
        else
            pivotPoint.transform.DOLocalRotate(new Vector3(0, 0, 45), 0.2f).SetEase(Ease.OutSine);
        
        _isToggled = !_isToggled;
    }
}
