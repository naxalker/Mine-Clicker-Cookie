using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AdPopup : MonoBehaviour
{
    public Action OnButtonClicked;

    [SerializeField] private Button _adButton;
    [SerializeField] private SpawnArea _spawnArea;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Image _rewardIcon;
    [SerializeField] private Sprite _defaultSprite;

    private RectTransform _rectTransform;
    private RectTransform _buttonRectTransform;
    private RectTransform _parentRectTransform;
    private Vector2 _normalizedPosition;

    private Sequence _sequence;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _buttonRectTransform = _adButton.GetComponent<RectTransform>();

        _adButton.onClick.AddListener(() => OnButtonClicked?.Invoke());
    }

    private void OnEnable()
    {
        if (_sequence == null)
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_buttonRectTransform.DOScale(1.05f, 0.7f).From(1f).SetEase(Ease.InOutSine));
            _sequence.Append(_buttonRectTransform.DOScale(1.0f, 0.7f).SetEase(Ease.InOutSine));
            _sequence.SetLoops(-1);
            _sequence.Pause();
        }

        _sequence.Restart();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdatePosition();
    }


    private void OnDisable()
    {
        _sequence?.Pause();
    }

    public void Show(double rewardAmount, Sprite sprite = null)
    {
        gameObject.SetActive(true);

        _rewardText.text = "+" + DoubleUtilities.ToIdleNotation(rewardAmount);

        _rewardIcon.sprite = sprite ?? _defaultSprite;

        _parentRectTransform = _spawnArea.GetSpawnArea();
        _rectTransform.SetParent(_parentRectTransform);

        float randomNormalizedX = Random.Range(0f, 1f);
        float randomNormalizedY = Random.Range(0f, 1f);
        _normalizedPosition = new Vector2(randomNormalizedX, randomNormalizedY);

        UpdatePosition();

        _rectTransform.DOScale(1f, .7f).From(0f).SetEase(Ease.OutBack);
    }

    public void Hide(bool hideImmediately = false)
    {
        if (hideImmediately)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _rectTransform
                .DOScale(0f, .7f)
                .From(1f)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }

    private void UpdatePosition()
    {
        if (_parentRectTransform == null) return;

        Rect parentRect = _parentRectTransform.rect;

        float targetLocalX = parentRect.xMin + (_normalizedPosition.x * parentRect.width);
        float targetLocalY = parentRect.yMin + (_normalizedPosition.y * parentRect.height);

        _rectTransform.localPosition = new Vector2(targetLocalX, targetLocalY);
    }
}
