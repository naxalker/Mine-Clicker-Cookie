using System;
using YG;
using Zenject;

public class LeaderboardController : IInitializable, IDisposable
{
    public Action<int> OnRecordChanged;

    private UpgradeManager _upgradeManager;

    private int _record;

    public LeaderboardController(UpgradeManager upgradeManager)
    {
        _upgradeManager = upgradeManager;
    }

    public void Initialize()
    {
        _record = SavesManager.GetRecord();

        _upgradeManager.OnTotalIncomeValueChanged += TotalIncomeValueChangedHandler;
    }

    public void Dispose()
    {
        _upgradeManager.OnTotalIncomeValueChanged -= TotalIncomeValueChangedHandler;
    }

    private void TotalIncomeValueChangedHandler(double value)
    {
        int newRecord = (int)(value / 1000);

        if (newRecord > _record)
        {
            _record = newRecord;

            OnRecordChanged?.Invoke(_record);
            
            YG2.SetLeaderboard("passiveIncomeLeaderboard", _record);
        }
    }
}
