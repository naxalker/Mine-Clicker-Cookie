using YG;
using Zenject;

public class UIContainerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        if (YG2.envir.isDesktop)
        {
            Container.Bind<UIContainer>().FromComponentInNewPrefabResource("[DESKTOP]").AsSingle();
        }
        else if (YG2.envir.isMobile || YG2.envir.isTablet)
        {
            Container.Bind<UIContainer>().FromComponentInNewPrefabResource("[MOBILE]").AsSingle();
        }
    }
}