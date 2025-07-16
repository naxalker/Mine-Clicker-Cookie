using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _incomeTextPrefab;

    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;
    private Vector2 _mySize;
    private Vector2 _normalizedPosition;
    private Image _image;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _rectTransform.DOScale(1f, .5f).From(0f).SetEase(Ease.OutBack);

        _rectTransform
            .DORotate(new Vector3(0, 0, 10f), 3f)
            .From(new Vector3(0, 0, -10f))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Setup(RectTransform parent, Vector2 normalizedPosition, Sprite sprite)
    {
        _parentRectTransform = parent;
        _mySize = _rectTransform.sizeDelta;
        _normalizedPosition = normalizedPosition;

        UpdatePosition();

        _image.sprite = sprite;
    }

    void OnRectTransformDimensionsChange()
    {
        UpdatePosition();
    }

    public void Animate(double incomeValue)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_rectTransform.DOScale(1.2f, .3f).SetEase(Ease.InSine));
        sequence.AppendCallback(() => SpawnIncomeText(incomeValue));
        sequence.Append(_rectTransform.DOScale(1f, .3f).SetEase(Ease.OutSine));
    }

    private void SpawnIncomeText(double incomeValue)
    {
        Rect rect = _rectTransform.rect;
        Vector3 localOffset = new Vector3(Random.Range(-rect.width / 2, rect.width / 2), rect.height, 0);
        Vector3 worldPosition = _rectTransform.TransformPoint(localOffset);

        TMP_Text incomeText = Instantiate(
            _incomeTextPrefab,
            worldPosition,
            Quaternion.identity,
            _rectTransform.parent);

        incomeText.text = $"+{DoubleUtilities.ToCustomScientificNotation(incomeValue)}";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(incomeText.rectTransform.DOAnchorPosY(incomeText.rectTransform.anchoredPosition.y + 40f, .7f).SetEase(Ease.Linear));
        sequence.Join(incomeText.DOFade(0f, .3f).SetDelay(.4f));
        sequence.OnComplete(() => Destroy(incomeText.gameObject));
    }

    private void UpdatePosition()
    {
        if (_parentRectTransform == null) return;

        Rect parentRect = _parentRectTransform.rect;

        float myWidth = _mySize.x;
        float myHeight = _mySize.y;

        float availableWidthForCenter = parentRect.width - myWidth;
        float availableHeightForCenter = parentRect.height - myHeight;

        availableWidthForCenter = Mathf.Max(0, availableWidthForCenter);
        availableHeightForCenter = Mathf.Max(0, availableHeightForCenter);

        float minCenterX = parentRect.xMin + myWidth / 2f;
        float minCenterY = parentRect.yMin + myHeight / 2f;

        float targetLocalX = minCenterX + (_normalizedPosition.x * availableWidthForCenter);
        float targetLocalY = minCenterY + (_normalizedPosition.y * availableHeightForCenter);

        _rectTransform.localPosition = new Vector2(targetLocalX, targetLocalY);
    }
}
