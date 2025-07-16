using UnityEngine;
using Zenject;

public class QuitSaver : MonoBehaviour
{
    private SavesManager _savesManager;

    [Inject]
    private void Construct(SavesManager savesManager)
    {
        _savesManager = savesManager;
    }

    public void Save()
    {
        _savesManager.SaveBalance();
    }
}
