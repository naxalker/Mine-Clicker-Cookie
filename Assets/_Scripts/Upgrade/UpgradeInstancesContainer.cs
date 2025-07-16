using UnityEngine;

[RequireComponent(typeof(SpawnArea))]
public class UpgradeInstancesContainer : MonoBehaviour
{
    [SerializeField] UpgradeUI _spawnUpgradePrefab;
    private SpawnArea _spawnArea;

    private void Awake()
    {
        _spawnArea = GetComponent<SpawnArea>();
    }

    public UpgradeUI CreateUpgradeUI(Sprite icon)
    {
        RectTransform spawnArea = _spawnArea.GetSpawnArea();

        float randomNormalizedX = Random.Range(0f, 1f);
        float randomNormalizedY = Random.Range(0f, 1f);
        Vector2 normalizedSpawnPos = new Vector2(randomNormalizedX, randomNormalizedY);

        UpgradeUI spawnedInstance = Instantiate(_spawnUpgradePrefab, spawnArea);

        spawnedInstance.Setup(spawnArea, normalizedSpawnPos, icon);

        return spawnedInstance;

        // Rect rect = spawnArea.rect;

        // float randomX = Random.Range(rect.xMin, rect.xMax);
        // float randomY = Random.Range(rect.yMin, rect.yMax);
        // Vector2 localPoint = new Vector2(randomX, randomY);

        // UpgradeUI upgradeUI = Instantiate(_spawnUpgradePrefab, spawnArea);
        // upgradeUI.Setup(localPoint, icon);

        // return upgradeUI;
    }
}
