using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class VolumeButton : MonoBehaviour
{
    public Action<bool> OnButtonClicked;

    [SerializeField] private Image _volumeImage;
    [SerializeField] private Sprite _volumeSprite;
    [SerializeField] private Sprite _muteSprite;

    private Button _button;
    private bool _isOn = true;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() => ToggleButton());
    }

    private void ToggleButton()
    {
        _isOn = !_isOn;

        _volumeImage.sprite = _isOn ? _volumeSprite : _muteSprite;

        OnButtonClicked?.Invoke(_isOn);
    }
}
