public class ScytheWorkbench : Workbench
{
    protected override int CurrentLevel => _dataManager.Data.ScytheLevel;

    protected override int MaxLevel => _dataManager.ScytheUpgradeConfigs.Count - 1;

    protected override UpgradeConfig NextUpgrade => _dataManager.ScytheUpgradeConfigs[NextLevel];

    protected override void SetCurrentUpgradeLevel(int level)
    {
        _dataManager.SetScytheLevel(level);
    }
}
