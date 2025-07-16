using System;
using UnityEngine;
using Zenject;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class SavesManager : IInitializable, ITickable, IDisposable
{
    private const string CURRENT_BALANCE_KEY = "CurrentBalance";
    private const string UPGRADE_KEY = "Upgrade";
    private const string RECORD_KEY = "Record";
    private const string GAME_OVER_SCREEN_SHOWN_KEY = "ScreenShown";

    private const float SECONDS_BETWEEN_BALANCE_SAVES = 5f;

    private double _previousBalance = 0;
    private float _saveTimer = SECONDS_BETWEEN_BALANCE_SAVES;

    private BalanceManager _balanceManager;
    private LeaderboardController _leaderboardController;

    public SavesManager(LeaderboardController leaderboardController, BalanceManager balanceManager)
    {
        _leaderboardController = leaderboardController;
        _balanceManager = balanceManager;
    }

    public void Initialize()
    {
        _previousBalance = _balanceManager.CurrentBalance;

        _balanceManager.OnBalanceChanged += BalanceChangedHandler;
        Upgrade.OnUpgradeLevelIncreased += UpgradeLevelIncreasedHandler;
        _leaderboardController.OnRecordChanged += RecordChangedController;
        GameOverHandler.OnGameOverScreenShown += GameOverScreenShownHandler;
    }

    public void Tick()
    {
        _saveTimer -= Time.deltaTime;

        if (_saveTimer <= 0)
        {
            SaveBalance(_balanceManager.CurrentBalance);

            _saveTimer = SECONDS_BETWEEN_BALANCE_SAVES;
        }
    }

    public void Dispose()
    {
        _balanceManager.OnBalanceChanged -= BalanceChangedHandler;
        Upgrade.OnUpgradeLevelIncreased -= UpgradeLevelIncreasedHandler;
        _leaderboardController.OnRecordChanged -= RecordChangedController;
        GameOverHandler.OnGameOverScreenShown -= GameOverScreenShownHandler;
    }

    public static double GetBalance() => PlayerPrefs.GetFloat(CURRENT_BALANCE_KEY, 0);

    public static int GetUpgradeLevel(int index) => PlayerPrefs.GetInt(UPGRADE_KEY + index, 0);

    public static int GetRecord() => PlayerPrefs.GetInt(RECORD_KEY, 0);

    public static bool GameOverScreenShown() => PlayerPrefs.GetInt(GAME_OVER_SCREEN_SHOWN_KEY, 0) == 1;

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_BALANCE_KEY);
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey(UPGRADE_KEY + i);
        }
        PlayerPrefs.Save();
    }

    public void SaveBalance()
    {
        SaveBalance(_balanceManager.CurrentBalance);
    }

    private void SaveBalance(double value)
    {
        PlayerPrefs.SetFloat(CURRENT_BALANCE_KEY, (float)value);
        PlayerPrefs.Save();

        _previousBalance = value;
        _saveTimer = SECONDS_BETWEEN_BALANCE_SAVES;
    }

    private void BalanceChangedHandler(double value)
    {
        if (value < _previousBalance)
        {
            SaveBalance(value);
        }
    }

    private void UpgradeLevelIncreasedHandler(Upgrade upgrade)
    {
        PlayerPrefs.SetInt(UPGRADE_KEY + upgrade.Id, upgrade.Level);
        PlayerPrefs.Save();
    }

    private void RecordChangedController(int record)
    {
        PlayerPrefs.SetInt(RECORD_KEY, record);
        PlayerPrefs.Save();
    }

    private void GameOverScreenShownHandler()
    {
        PlayerPrefs.SetInt(GAME_OVER_SCREEN_SHOWN_KEY, 1);
        PlayerPrefs.Save();
    }
}
