using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    public static Action<UpgradeButton> OnButtonClicked;

    [Header("References")]
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private TMP_Text _income;
    [SerializeField] private LocalizedString _incomeString;
    [SerializeField] private AudioSource _audioSource;

    private Button _button;
    private BalanceManager _balanceManager;
    private Upgrade _upgrade;
    public int level = 1;

    [Inject]
    private void Construct(BalanceManager balanceManager)
    {
        _balanceManager = balanceManager;
    }

    private UpgradeConfig UpgradeConfig => _upgrade.UpgradeConfig;

    public void Setup(Upgrade upgrade)
    {
        _upgrade = upgrade;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => ButtonClickedHandler());

        SetupVisuals().Forget();

        _upgrade.OnLevelIncresed += LevelIncreasedHandler;
        _balanceManager.OnBalanceChanged += AmountValueChangedHandler;
        LocalizationSettings.SelectedLocaleChanged += LocaleChangedHandler;
    }

    private void OnDestroy()
    {
        _balanceManager.OnBalanceChanged -= AmountValueChangedHandler;
        _upgrade.OnLevelIncresed -= LevelIncreasedHandler;
        LocalizationSettings.SelectedLocaleChanged -= LocaleChangedHandler;
    }

    private void LevelIncreasedHandler(int level)
    {
        UpdateVisuals(level).Forget();
    }

    private void AmountValueChangedHandler(double amountValue)
    {
        _button.interactable = amountValue >= _upgrade.Cost;
    }

    private async UniTask UpdateVisuals(int level)
    {
        if (level == 1)
        {
            _image.color = Color.white;
            _name.text = await UpgradeConfig.UpgradeName.GetLocalizedStringAsync().Task;
        }

        UpdateUpgradeInfo();
    }

    private async UniTask SetupVisuals()
    {
        _image.sprite = UpgradeConfig.Icon;
        UpdateUpgradeInfo();

        bool isPurchased = _upgrade.Level > 0;

        _image.color = isPurchased ? Color.white : Color.black;
        _name.text = isPurchased ? await UpgradeConfig.UpgradeName.GetLocalizedStringAsync().Task : "???";

        _button.interactable = _balanceManager.CurrentBalance >= _upgrade.Cost;
    }

    private async void UpdateUpgradeInfo()
    {
        _cost.text = DoubleUtilities.ToCustomScientificNotation(_upgrade.Cost);

        _incomeString.Arguments = new object[] {
            _upgrade.Level,
            DoubleUtilities.ToCustomScientificNotation(_upgrade.IncomeBase)
        };

        _income.text = await _incomeString.GetLocalizedStringAsync().Task;
    }

    private void ButtonClickedHandler()
    {
        OnButtonClicked?.Invoke(this);

        _audioSource.clip = UpgradeConfig.UpgradeAudioClips[Random.Range(0, UpgradeConfig.UpgradeAudioClips.Length)];
        _audioSource.Play();
    }

    private async void LocaleChangedHandler(Locale locale)
    {
        _incomeString.Arguments = new object[] {
            _upgrade.Level,
            DoubleUtilities.ToCustomScientificNotation(_upgrade.IncomeBase)
        };

        _income.text = await _incomeString.GetLocalizedStringAsync().Task;

        _name.text =
            await _upgrade.UpgradeConfig.UpgradeName.GetLocalizedStringAsync().Task;
    }
}
