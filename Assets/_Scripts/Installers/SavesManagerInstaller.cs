using Zenject;

public class SavesManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SavesManager>().AsSingle();
    }
}