using System.Collections;
using TMPro;
using UnityEngine;

public class AdWarningPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private AdPanel _adPanel;

    public void Show(float delay)
    {
        StopAllCoroutines();
        
        gameObject.SetActive(true);

        StartCoroutine(ShowTimer(delay));
    }

    private IEnumerator ShowTimer(float delay)
    {
        while (delay > 0)
        {
            _timerText.text = delay.ToString("F0");
            yield return new WaitForSeconds(1f);
            delay--;
        }

        _adPanel.Hide();
    }

    public void Hide() => gameObject.SetActive(false);
}
