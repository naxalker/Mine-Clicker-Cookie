using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeButton _upgradeButtonPrefab;
    [SerializeField] private Transform _upgradeButtonsContainer;
    [SerializeField] private TMP_Text _availableUpgradeText;

    private bool _isSubscribed;

    private DiContainer _container;
    private BalanceManager _balanceManager;
    private UpgradeManager _upgradeManager;

    [Inject]
    private void Construct(DiContainer container, BalanceManager balanceManager, UpgradeManager upgradeManager)
    {
        _container = container;
        _balanceManager = balanceManager;
        _upgradeManager = upgradeManager;
    }

    protected virtual void Awake()
    {
        _balanceManager.OnBalanceChanged += AmountValueChangedHandler;
        _upgradeManager.OnInitialized += UpgradeManagerInitializedHandler;
        _isSubscribed = true;
    }

    private void OnDestroy()
    {
        if (_isSubscribed)
        {
            _balanceManager.OnBalanceChanged -= AmountValueChangedHandler;
            _upgradeManager.OnInitialized -= UpgradeManagerInitializedHandler;
            _isSubscribed = false;
        }
    }

    public UpgradeButton SpawnButton(Upgrade upgrade)
    {
        UpgradeButton button = _container.InstantiatePrefabForComponent<UpgradeButton>(_upgradeButtonPrefab, _upgradeButtonsContainer);
        button.Setup(upgrade);

        return button;
    }

    private void UpgradeManagerInitializedHandler()
    {
        SetAvailableUpgradeText(_balanceManager.CurrentBalance);
    }

    private void AmountValueChangedHandler(double value)
    {
        SetAvailableUpgradeText(value);
    }

    private void SetAvailableUpgradeText(double value)
    {
        Upgrade firstUnavailableUpgrade = _upgradeManager.Upgrades.FirstOrDefault(upgrade => upgrade.Level == 0);

        if (firstUnavailableUpgrade == null)
        {
            if (_isSubscribed)
            {
                _balanceManager.OnBalanceChanged -= AmountValueChangedHandler;
                _isSubscribed = false;
            }

            _availableUpgradeText.gameObject.SetActive(false);
        }
        else
        {
            _availableUpgradeText.gameObject.SetActive(value >= firstUnavailableUpgrade.Cost);
        }
    }
}
