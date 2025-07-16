using UnityEngine;
using Zenject;

public class AdControllerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AdController>().AsSingle();
    }
}
