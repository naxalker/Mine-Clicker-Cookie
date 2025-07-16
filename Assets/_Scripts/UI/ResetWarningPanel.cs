using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetWarningPanel : MonoBehaviour
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private SettingsPanel _settingsPanel;

    private void Awake()
    {
        _yesButton.onClick.AddListener(() => ResetProgress());
        _noButton.onClick.AddListener(() => Hide());
    }

    private void ResetProgress()
    {
        SavesManager.ResetProgress();
        SceneManager.LoadScene(0);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        _settingsPanel.Show();
    }
}
