using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ClickLevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _clickLevelText;
    [SerializeField] private Image _clickImage;

    private BalanceManager _balanceManager;
    private RectTransform _rectTransform;

    private Sequence _sequence;

    [Inject]
    private void Construct(BalanceManager balanceManager)
    {
        _balanceManager = balanceManager;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _balanceManager.OnClickIncomeChanged += ClickIncomeChangedHandler;
    }

    private void Start()
    {
        SetText(_balanceManager.ClickIncome);
    }

    private void OnDestroy()
    {
        _balanceManager.OnClickIncomeChanged -= ClickIncomeChangedHandler;
    }

    private void ClickIncomeChangedHandler(double incomeValue)
    {
        Animate(incomeValue);
    }

    private void SetText(double incomeValue)
    {
        _clickLevelText.text = "+" + DoubleUtilities.ToCustomScientificNotation(incomeValue);
    }

    private void Animate(double incomeValue)
    {
        _sequence?.Kill();

        _sequence = DOTween.Sequence()
            .Append(_rectTransform.DOScale(1.2f, .5f).From(1f).SetEase(Ease.InBack))
            .Join(_clickImage.DOColor(Color.green, .5f).From(Color.white).SetEase(Ease.InBack))
            .AppendCallback(() => SetText(incomeValue))
            .SetLoops(2, LoopType.Yoyo);
    }
}
