using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private RectTransform[] _spawnAreas;

    private int _currentSpawnArea;

    public RectTransform GetSpawnArea() => _spawnAreas[(_currentSpawnArea++)% _spawnAreas.Length];
}
