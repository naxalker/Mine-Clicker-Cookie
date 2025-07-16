using System;
using UnityEngine.Audio;
using Zenject;

public class AudioController : IInitializable, IDisposable
{
    private AudioMixer _audioMixer;
    private VolumeButton _volumeButton;
    private UIContainer _uiContainer;

    public AudioController(AudioMixer audioMixer, UIContainer uiContainer)
    {
        _audioMixer = audioMixer;
        _uiContainer = uiContainer;
    }

    public void Initialize()
    {
        _volumeButton = _uiContainer.GetComponentInChildren<VolumeButton>(true);

        _volumeButton.OnButtonClicked += ButtonClickedHandler;
    }

    public void Dispose()
    {
        _volumeButton.OnButtonClicked -= ButtonClickedHandler;
    }

    public void ButtonClickedHandler(bool isOn)
    {
        if (isOn)
        {
            _audioMixer.SetFloat("MasterVolume", 0f);
        }
        else
        {
            _audioMixer.SetFloat("MasterVolume", -80f);
        }
    }
}
