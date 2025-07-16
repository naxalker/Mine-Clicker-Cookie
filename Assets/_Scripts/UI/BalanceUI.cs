using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class BalanceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _balanceText;
    [SerializeField] private TMP_Text _incomeText;
    [SerializeField] private LocalizedString _incomeString;

    private BalanceManager _balanceManager;
    private UpgradeManager _upgradeManager;

    [Inject]
    private void Construct(BalanceManager balanceManager, UpgradeManager upgradeManager)
    {
        _balanceManager = balanceManager;
        _upgradeManager = upgradeManager;
    }

    private void Awake()
    {
        _balanceManager.OnInitialized += BalanceManagerInitializedHandler;
        _balanceManager.OnBalanceChanged += AmountValueChangedHandler;
        _upgradeManager.OnTotalIncomeValueChanged += TotalIncomeValueChangedHandler;
        _incomeString.StringChanged += StringChangedHandler;
    }

    private void OnDestroy()
    {
        _balanceManager.OnInitialized -= BalanceManagerInitializedHandler;
        _balanceManager.OnBalanceChanged -= AmountValueChangedHandler;
        _upgradeManager.OnTotalIncomeValueChanged -= TotalIncomeValueChangedHandler;
        _incomeString.StringChanged -= StringChangedHandler;
    }

    private void BalanceManagerInitializedHandler()
    {
        UpdateVisuals();
    }

    private void AmountValueChangedHandler(double value)
    {
        UpdateBalanceText();
    }

    private void TotalIncomeValueChangedHandler(double value)
    {
        UpdateIncomeText().Forget();
    }

    private void UpdateVisuals()
    {
        UpdateBalanceText();
        UpdateIncomeText().Forget();
    }

    private void StringChangedHandler(string value)
    {
        UpdateIncomeText().Forget();
    }

    private void UpdateBalanceText()
        => _balanceText.text = DoubleUtilities.ToCustomScientificNotation(Math.Floor(_balanceManager.CurrentBalance));

    private async UniTask UpdateIncomeText()
    {
        _incomeString.Arguments = new object[] {
            DoubleUtilities.ToCustomScientificNotation(_upgradeManager.TotalIncome)
        };

        _incomeText.text = await _incomeString.GetLocalizedStringAsync().Task;
    }
}
