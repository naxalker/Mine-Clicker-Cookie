using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Upgrade
{
    private const int MAX_UPGRADE_UI_AMOUNT = 10;

    public static Action<Upgrade> OnUpgradeLevelIncreased;
    public Action<int> OnLevelIncresed;

    private UpgradeConfig _upgradeConfig;
    private BalanceManager _balanceManager;
    private AsyncProcessor _asyncProcessor;
    private UpgradeInstancesContainer _upgradeInstancesContainer;

    private readonly int _id;
    private int _level;
    private List<UpgradeUI> _upgradeUIs = new List<UpgradeUI>();

    public Upgrade(int id, int level, UpgradeConfig upgradeConfig, BalanceManager balanceManager,
        AsyncProcessor asyncProcessor, UpgradeInstancesContainer upgradeInstancesContainer)
    {
        _id = id;
        _level = level;
        _upgradeConfig = upgradeConfig;
        _balanceManager = balanceManager;
        _asyncProcessor = asyncProcessor;
        _upgradeInstancesContainer = upgradeInstancesContainer;

        Initialize();
    }

    public double Cost => Math.Ceiling(_upgradeConfig.CostBase * Math.Pow(_upgradeConfig.RateGrowth, _level));
    public double Income => _upgradeConfig.IncomeBase * _level;
    public double IncomeBase => _upgradeConfig.IncomeBase;
    public UpgradeConfig UpgradeConfig => _upgradeConfig;
    public int Level => _level;
    public int Id => _id;

    public void IncreaseLevel()
    {
        _level++;

        CreateSingleUpgrade();

        OnLevelIncresed?.Invoke(_level);
        OnUpgradeLevelIncreased?.Invoke(this);
    }

    private void Initialize()
    {
        _asyncProcessor.StartCoroutine(CreateAllUpgrades());
    }

    private IEnumerator CreateAllUpgrades()
    {
        yield return new WaitForEndOfFrame();

        WaitForSeconds waitForSeconds = new WaitForSeconds(.3f);

        for (int i = 0; i < _level; i++)
        {
            _asyncProcessor.StartCoroutine(AddPassiveIncome(i));

            CreateSingleUI();

            yield return waitForSeconds;
        }
    }

    private void CreateSingleUpgrade()
    {
        CreateSingleUI();

        _asyncProcessor.StartCoroutine(AddPassiveIncome(_level - 1));
    }

    private void CreateSingleUI()
    {
        if (_upgradeUIs.Count < MAX_UPGRADE_UI_AMOUNT)
        {
            UpgradeUI upgradeUI = _upgradeInstancesContainer.CreateUpgradeUI(_upgradeConfig.Icon);
            _upgradeUIs.Add(upgradeUI);
        }
    }

    private IEnumerator AddPassiveIncome(int index)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

        while (true)
        {
            yield return waitForSeconds;

            _balanceManager.AddCurrency(_upgradeConfig.IncomeBase);

            if (index < _upgradeUIs.Count)
            {
                _upgradeUIs[index].Animate(_upgradeConfig.IncomeBase);
            }
        }
    }
}
