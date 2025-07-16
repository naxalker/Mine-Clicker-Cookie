using UnityEngine;
using Zenject;

public class UpgradeManagerInstaller : MonoInstaller
{
    [SerializeField] private UpgradeConfig[] _upgradeConfigs;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<UpgradeManager>().AsSingle().WithArguments(_upgradeConfigs);
    }
}
