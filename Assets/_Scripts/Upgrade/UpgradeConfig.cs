using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Configs/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public double CostBase { get; private set; }
    [field: SerializeField] public double IncomeBase { get; private set; }
    [field: SerializeField] public double RateGrowth { get; private set; }
    [field: SerializeField] public LocalizedString UpgradeName { get; private set; }
    [field: SerializeField] public AudioClip[] UpgradeAudioClips { get; private set; }
}
