using Zenject;

public class LeaderboardControllerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LeaderboardController>().AsSingle().NonLazy();
    }
}