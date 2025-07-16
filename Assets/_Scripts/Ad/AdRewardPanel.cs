using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdRewardPanel : MonoBehaviour
{
    [SerializeField] private Image _rewardImage;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Sprite _defaultRewardSprite;

    public void ShowUpgradeReward(Sprite upgradeIcon)
    {
        _rewardImage.sprite = upgradeIcon;
        _rewardText.text = "1";
        gameObject.SetActive(true);
    }

    public void ShowCurrencyReward(double amount)
    {
        _rewardImage.sprite = _defaultRewardSprite;
        _rewardText.text = DoubleUtilities.ToCustomScientificNotation(amount);
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
