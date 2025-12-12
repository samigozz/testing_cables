using DG.Tweening;
using UnityEngine;

public class PressButton : MonoBehaviour, IInteractable
{
    [SerializeField] private float pressAmount = 0.2f;
    private bool _isPlaying;
    public void Interact()
    {
        if (_isPlaying)
            return;
        
        transform.DOLocalMoveZ(pressAmount, 0.2f).SetRelative(true).SetEase(Ease.InOutQuad)
            .SetLoops(2, LoopType.Yoyo).OnStart(() => { _isPlaying = true; }).OnComplete(() => { _isPlaying = false; });
    }
}
