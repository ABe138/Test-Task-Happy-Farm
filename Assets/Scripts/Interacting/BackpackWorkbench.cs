public class BackpackWorkbench : Workbench
{
    protected override int CurrentLevel => _dataManager.Data.InventoryCapacityLevel;

    protected override int MaxLevel => _dataManager.InventoryUpgradeConfigs.Count - 1;

    protected override UpgradeConfig NextUpgrade => _dataManager.InventoryUpgradeConfigs[NextLevel];

    protected override void SetCurrentUpgradeLevel(int level)
    {
        _dataManager.SetInventoryLevel(level);
    }
}
