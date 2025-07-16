using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class AdPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AdWarningPanel _adWarningPanel;
    [SerializeField] private AdRewardPanel _adRewardPanel;

    private AdController _adController;
    private bool _canBeClosed;

    [Inject]
    private void Construct(AdController adController)
    {
        _adController = adController;

        _adController.OnDelayedAdShow += DelayedAdShowHandler;
        _adController.OnRewardAdShow += RewardAdShowHandler;
    }

    private void OnDestroy()
    {
        _adController.OnDelayedAdShow -= DelayedAdShowHandler;
        _adController.OnRewardAdShow -= RewardAdShowHandler;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == gameObject && _canBeClosed)
        {
            Hide();
        }
    }

    public void Hide()
    {
        _adWarningPanel.Hide();
        _adRewardPanel.Hide();
        gameObject.SetActive(false);
    }

    private void DelayedAdShowHandler(float delay)
    {
        gameObject.SetActive(true);
        _canBeClosed = false;
        _adWarningPanel.Show(delay);
    }

    private void RewardAdShowHandler(AdController.RewardType reward, double rewardAmount, Upgrade upgrade)
    {
        gameObject.SetActive(true);
        _canBeClosed = true;

        if (reward == AdController.RewardType.Currency)
        {
            _adRewardPanel.ShowCurrencyReward(rewardAmount);
        }
        else if (reward == AdController.RewardType.Upgrade)
        {
            _adRewardPanel.ShowUpgradeReward(upgrade.UpgradeConfig.Icon);
        }
    }
}
