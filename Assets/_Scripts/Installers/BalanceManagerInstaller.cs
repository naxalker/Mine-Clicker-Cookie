using Zenject;

public class BalanceManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<BalanceManager>().AsSingle();
    }
}