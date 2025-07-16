using DG.Tweening;
using UnityEngine;

public class UpgradePanelUIMobile : UpgradePanelUI
{
    private RectTransform _rectTransform;

    private bool _isOpen;
    private bool _toggleInProgress;

    public bool IsOpen => _isOpen;

    protected override void Awake()
    {
        base.Awake();

        _rectTransform = GetComponent<RectTransform>();
    }

    public void TogglePanel()
    {
        if (_toggleInProgress) { return; }

        _toggleInProgress = true;
        if (!_isOpen)
        {
            _rectTransform.DOAnchorPosY(1080f, .5f).SetEase(Ease.InSine).OnComplete(() =>
            {
                _isOpen = !_isOpen;
                _toggleInProgress = false;
            });
        }
        else
        {
            _rectTransform.DOAnchorPosY(130f, .5f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                _isOpen = !_isOpen;
                _toggleInProgress = false;
            });
        }
    }
}
