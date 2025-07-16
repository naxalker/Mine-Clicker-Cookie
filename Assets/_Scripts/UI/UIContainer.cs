using UnityEngine;

public class UIContainer : MonoBehaviour
{
    private void Awake()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
}
