using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _pauseHeader;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _volumeButton;
    [SerializeField] private Button _languageButton;
    [SerializeField] private ResetWarningPanel _resetWarningPanel;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        _resetButton.onClick.AddListener(() => ShowResetWarningPanel());
        _languageButton.onClick.AddListener(async () =>
        {
            await LocalizationSettings.InitializationOperation.Task;

            int currentLocaleIndex =
                LocalizationSettings.AvailableLocales.Locales
                    .IndexOf(LocalizationSettings.SelectedLocale);

            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[1 - currentLocaleIndex];
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == gameObject)
        {
            Hide();
        }
    }

    public void Show()
    {
        IsActive = true;
        gameObject.SetActive(true);
        ShowObjects();
        _resetWarningPanel.gameObject.SetActive(false);
    }

    private void Hide()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    private void ShowResetWarningPanel()
    {
        HideObjects();
        _resetWarningPanel.gameObject.SetActive(true);
    }

    private void HideObjects()
    {
        _pauseHeader.SetActive(false);
        _resetButton.gameObject.SetActive(false);
        _volumeButton.gameObject.SetActive(false);
        _languageButton.gameObject.SetActive(false);
    }

    private void ShowObjects()
    {
        _pauseHeader.SetActive(true);
        _resetButton.gameObject.SetActive(true);
        _volumeButton.gameObject.SetActive(true);
        _languageButton.gameObject.SetActive(true);
    }
}
