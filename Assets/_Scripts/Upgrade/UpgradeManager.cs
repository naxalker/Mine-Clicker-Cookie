using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class UpgradeManager : IInitializable, IDisposable
{
    public event Action OnInitialized;
    public event Action<double> OnTotalIncomeValueChanged;

    private double _totalIncome;
    private Dictionary<UpgradeButton, Upgrade> _buttonToUpgradeDictionary = new Dictionary<UpgradeButton, Upgrade>();

    private UpgradeConfig[] _upgradeConfigs;
    private AsyncProcessor _asyncProcessor;
    private UIContainer _uiContainer;
    [Inject] private BalanceManager _balanceManager;
    private UpgradePanelUI _upgradePanelUI;
    private UpgradeInstancesContainer _upgradeInstancesContainer;

    public UpgradeManager(UpgradeConfig[] upgradeConfigs, AsyncProcessor asyncProcessor, UIContainer uiContainer)
    {
        _upgradeConfigs = upgradeConfigs;
        _asyncProcessor = asyncProcessor;
        _uiContainer = uiContainer;
    }

    public double TotalIncome => _totalIncome;
    public Upgrade[] Upgrades => _buttonToUpgradeDictionary.Values.ToArray();

    public void Initialize()
    {
        _upgradePanelUI = _uiContainer.GetComponentInChildren<UpgradePanelUI>();
        _upgradeInstancesContainer = _uiContainer.GetComponentInChildren<UpgradeInstancesContainer>();

        for (int i = 0; i < _upgradeConfigs.Length; i++)
        {
            Upgrade upgrade = new Upgrade(i, SavesManager.GetUpgradeLevel(i), _upgradeConfigs[i], _balanceManager, _asyncProcessor, _upgradeInstancesContainer);
            UpgradeButton upgradeButton = _upgradePanelUI.SpawnButton(upgrade);
            _buttonToUpgradeDictionary[upgradeButton] = upgrade;
        }

        CalculateTotalIncome();

        UpgradeButton.OnButtonClicked += UpgradeButtonClickedHandler;

        OnInitialized?.Invoke();
    }

    public void Dispose()
    {
        UpgradeButton.OnButtonClicked -= UpgradeButtonClickedHandler;
    }

    private void UpgradeButtonClickedHandler(UpgradeButton button)
    {
        TryIncreaseUpgradeLevel(_buttonToUpgradeDictionary[button]);
    }

    public void TryIncreaseUpgradeLevel(Upgrade upgrade, bool forFree = false)
    {
        if (forFree)
        {
            upgrade.IncreaseLevel();
            CalculateTotalIncome();
        }
        else
        {
            if (_balanceManager.TrySpendCurrency(upgrade.Cost))
            {
                upgrade.IncreaseLevel();
                CalculateTotalIncome();
            }
        }
    }

    private void CalculateTotalIncome()
    {
        double totalIncome = 0;

        foreach (Upgrade upgrade in Upgrades)
        {
            totalIncome += upgrade.Income;
        }

        _totalIncome = totalIncome;

        OnTotalIncomeValueChanged?.Invoke(_totalIncome);
    }
}
