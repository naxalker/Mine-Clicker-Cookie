using System;
using UnityEngine;
using Zenject;

public class BalanceManager : IInitializable, IDisposable, ITickable
{
    public event Action OnInitialized;
    public event Action<double> OnBalanceChanged;
    public event Action<double> OnClickIncomeChanged;

    private double _currentAmount;
    private double _clickIncome;

    [Inject] private UpgradeManager _upgradeManager;
    private UIContainer _uiContainer;
    private GingerbreadUI _gingerbreadUI;

    public BalanceManager(UIContainer container)
    {
        _uiContainer = container;
    }

    public double CurrentBalance => _currentAmount;
    public double ClickIncome => _clickIncome;

    public void Initialize()
    {
        _gingerbreadUI = _uiContainer.GetComponentInChildren<GingerbreadUI>();

        _gingerbreadUI.OnClicked += OnGingerbreadClickedHandler;
        _upgradeManager.OnInitialized += UpgradeManagerInitializedHandler;
        _upgradeManager.OnTotalIncomeValueChanged += TotalIncomeValueChanged;
    }

    public void Tick()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddCurrency(_currentAmount * 2 + 100);
        }
#endif
    }

    public void Dispose()
    {
        _gingerbreadUI.OnClicked -= OnGingerbreadClickedHandler;
        _upgradeManager.OnInitialized -= UpgradeManagerInitializedHandler;
        _upgradeManager.OnTotalIncomeValueChanged -= TotalIncomeValueChanged;
    }

    private void OnGingerbreadClickedHandler()
    {
        AddCurrency(ClickIncome);
    }

    public void AddCurrency(double amountToAdd)
    {
        _currentAmount += amountToAdd;

        OnBalanceChanged?.Invoke(_currentAmount);
    }

    public bool TrySpendCurrency(double amountToSpend)
    {
        if (amountToSpend <= _currentAmount)
        {
            _currentAmount -= amountToSpend;

            OnBalanceChanged?.Invoke(_currentAmount);

            return true;
        }

        return false;
    }

    private void UpgradeManagerInitializedHandler()
    {
        _currentAmount = SavesManager.GetBalance();

        CalculateClickIncome();

        OnInitialized?.Invoke();
    }

    private void TotalIncomeValueChanged(double incomeValue)
    {
        CalculateClickIncome();
    }

    private void CalculateClickIncome()
    {
        double newClickIncome = 1 + Math.Floor(0.02 * _upgradeManager.TotalIncome);
        if (newClickIncome > _clickIncome)
        {
            _clickIncome = newClickIncome;
            OnClickIncomeChanged?.Invoke(_clickIncome);
        }
    }
}
