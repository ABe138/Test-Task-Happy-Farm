using UnityEngine;

public class PlayerUpgradesGraphic : MonoBehaviour
{
    [SerializeField] private Transform _scytheAnchor;
    [SerializeField] private Transform _backpackAnchor;

    private GameObject _currentBackpack = null;
    private GameObject _currentScythe = null;

    private void Start()
    {
        DataManager.Instance.OnUpgradeBackpack += UpgradeBackpack;
        DataManager.Instance.OnUpgradeScythe += UpgradeScythe;

        UpgradeBackpack();
        UpgradeScythe();
    }

    private void UpgradeBackpack()
    {
        var dataManager = DataManager.Instance;
        var currenLevel = dataManager.Data.InventoryCapacityLevel;

        if (_currentBackpack != null) Destroy(_currentBackpack);

        var config = dataManager.InventoryUpgradeConfigs[currenLevel];
        _currentBackpack = Instantiate(config.BackpackPrefab, _backpackAnchor);
        _currentBackpack.transform.localPosition = Vector3.zero;
        _currentBackpack.transform.localRotation = Quaternion.identity;
    }

    private void UpgradeScythe()
    {
        var dataManager = DataManager.Instance;
        var currenLevel = dataManager.Data.ScytheLevel;

        if (_currentScythe != null) Destroy(_currentScythe);

        var config = dataManager.ScytheUpgradeConfigs[currenLevel];
        _currentScythe = Instantiate(config.ScythePrefab, _scytheAnchor);
        _currentScythe.transform.localPosition = Vector3.zero;
        _currentScythe.transform.localRotation = Quaternion.identity;
    }
}
