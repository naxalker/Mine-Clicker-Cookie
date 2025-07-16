using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using YG;
using Zenject;
using Random = UnityEngine.Random;

public class AdController : IInitializable, IDisposable
{
    public enum RewardType
    {
        Currency,
        Upgrade
    }

    public Action<float> OnDelayedAdShow;
    public Action<RewardType, double, Upgrade> OnRewardAdShow;

    private const float INTER_AD_COOLDOWN = 60f;
    private const float AD_SHOW_DELAY = 2f;

    private const float REW_AD_COOLDOWN = 30f;
    private const float REW_AD_LIFE_TIME = 15f;
    private Coroutine _rewAdcoroutine;

    private RewardType _reward;
    private double _rewardAmount;
    private Upgrade _upgrade;

    private BalanceManager _balanceManager;
    private UpgradeManager _upgradeManager;

    private AdPopup _adPopup;

    private UIContainer _uiContainer;
    private AsyncProcessor _asyncProcessor;
    private SettingsPanel _settingsPanel;
    private UpgradePanelUIMobile _upgradePanelUI;
    private Coroutine _interAdCoroutine;

    public AdController(AsyncProcessor asyncProcessor, BalanceManager balanceManager, UpgradeManager upgradeManager, UIContainer uiContainer)
    {
        _balanceManager = balanceManager;
        _upgradeManager = upgradeManager;
        _asyncProcessor = asyncProcessor;
        _uiContainer = uiContainer;
    }

    public void Initialize()
    {
        _adPopup = _uiContainer.GetComponentInChildren<AdPopup>(true);
        _rewAdcoroutine = _asyncProcessor.StartCoroutine(HandleAdButton(REW_AD_COOLDOWN));

        _upgradePanelUI = _uiContainer.GetComponentInChildren<UpgradePanelUIMobile>();
        _settingsPanel = _uiContainer.GetComponentInChildren<SettingsPanel>(true);

        _interAdCoroutine = _asyncProcessor.StartCoroutine(ShowInterAd(INTER_AD_COOLDOWN, AD_SHOW_DELAY));

        _adPopup.OnButtonClicked += ButtonClickedHandler;
#if YandexGamesPlatform_yg
        YG2.onOpenRewardedAdv += OpenRewardedAdvHandler;
#endif
        YG2.onOpenInterAdv += OpenInterAdvHandler;
    }

    public void Dispose()
    {
        _adPopup.OnButtonClicked -= ButtonClickedHandler;
#if YandexGamesPlatform_yg
        YG2.onOpenRewardedAdv -= OpenRewardedAdvHandler;
#endif
        YG2.onOpenInterAdv -= OpenInterAdvHandler;
    }

    private IEnumerator HandleAdButton(float cooldown)
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);

            _reward = (RewardType)Random.Range(0, Enum.GetValues(typeof(RewardType)).Length);

            if (_reward == RewardType.Currency)
            {
                _rewardAmount = Math.Max(100.0, DoubleUtilities.RoundToLeadingDigit(_upgradeManager.TotalIncome * 120));

                _adPopup.Show(_rewardAmount);
            }
            else if (_reward == RewardType.Upgrade)
            {
                _upgrade = _upgradeManager.Upgrades.Reverse().FirstOrDefault(upgrade => upgrade.Level > 0);

                if (_upgrade != null)
                {
                    _adPopup.Show(1, _upgrade.UpgradeConfig.Icon);
                }
                else
                {
                    _reward = RewardType.Currency;

                    _rewardAmount = Math.Max(100.0, DoubleUtilities.RoundToLeadingDigit(_upgradeManager.TotalIncome * 120));

                    _adPopup.Show(_rewardAmount);
                }
            }

            yield return new WaitForSeconds(REW_AD_LIFE_TIME);

            _adPopup.Hide();
        }
    }

    private void ButtonClickedHandler()
    {
        if (_reward == RewardType.Currency)
        {
#if GameMonetizePlatform_yg
            YG2.InterstitialAdvShow();
            _balanceManager.AddCurrency(_rewardAmount);
            OnRewardAdShow?.Invoke(_reward, _rewardAmount, _upgrade);
#else 
            YG2.RewardedAdvShow("AddCurrency", () =>
            {
                _balanceManager.AddCurrency(_rewardAmount);

                OnRewardAdShow?.Invoke(_reward, _rewardAmount, _upgrade);
            });
#endif
        }
        else if (_reward == RewardType.Upgrade)
        {
#if GameMonetizePlatform_yg
            YG2.InterstitialAdvShow();
            _upgradeManager.TryIncreaseUpgradeLevel(_upgrade, true);
            OnRewardAdShow?.Invoke(_reward, _rewardAmount, _upgrade);
#else
            YG2.RewardedAdvShow("IncreaseUpgradeLevel", () =>
            {
                _upgradeManager.TryIncreaseUpgradeLevel(_upgrade, true);

                OnRewardAdShow?.Invoke(_reward, _rewardAmount, _upgrade);
            });
#endif
        }

        _adPopup.Hide(true);
#if GameMonetizePlatform_yg
        if (_rewAdcoroutine != null)
        {
            _asyncProcessor.StopCoroutine(_rewAdcoroutine);
        }
        _rewAdcoroutine = _asyncProcessor.StartCoroutine(HandleAdButton(REW_AD_COOLDOWN));
#endif
    }

    private IEnumerator ShowInterAd(float cooldown, float delay)
    {
        yield return new WaitForSeconds(cooldown);

        while (_settingsPanel.IsActive || (_upgradePanelUI != null && _upgradePanelUI.IsOpen))
        {
            yield return null;
        }

        OnDelayedAdShow?.Invoke(delay);

        yield return new WaitForSeconds(delay);

        YG2.InterstitialAdvShow();
    }

#if YandexGamesPlatform_yg
    private void OpenRewardedAdvHandler()
    {
        if (_rewAdcoroutine != null)
        {
            _asyncProcessor.StopCoroutine(_rewAdcoroutine);
        }
        _rewAdcoroutine = _asyncProcessor.StartCoroutine(HandleAdButton(REW_AD_COOLDOWN));

        if (_interAdCoroutine != null)
        {
            _asyncProcessor.StopCoroutine(_interAdCoroutine);
        }
        _interAdCoroutine = _asyncProcessor.StartCoroutine(ShowInterAd(INTER_AD_COOLDOWN / 2f, AD_SHOW_DELAY));
    }
#endif

    private void OpenInterAdvHandler()
    {
        if (_interAdCoroutine != null)
        {
            _asyncProcessor.StopCoroutine(_interAdCoroutine);
        }
        _interAdCoroutine = _asyncProcessor.StartCoroutine(ShowInterAd(INTER_AD_COOLDOWN, AD_SHOW_DELAY));
    }
}
