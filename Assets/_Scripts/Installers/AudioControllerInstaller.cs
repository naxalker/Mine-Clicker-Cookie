using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class AudioControllerInstaller : MonoInstaller
{
    [SerializeField] private AudioMixer _audioMixer;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AudioController>().AsSingle().WithArguments(_audioMixer).NonLazy();
    }
}
