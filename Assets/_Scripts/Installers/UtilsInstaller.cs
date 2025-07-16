using Zenject;

public class UtilsInstaller : MonoInstaller 
{
    public override void InstallBindings()
    {
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();
    }
}
