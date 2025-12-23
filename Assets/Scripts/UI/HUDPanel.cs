using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    [SerializeField] private UIInventoryView _inventoryView;
    [SerializeField] private UIPriceView _priceView;

    [SerializeField] private Image _backpackImage;
    [SerializeField] private Image _scytheImage;

    [SerializeField] private TextMeshProUGUI _inventoryCapacityText;

    private void Start()
    {
        DataManager.Instance.OnUpdateInventory += UpdateInventory;
        DataManager.Instance.OnUpdateCurrency += UpdateCurrency;

        DataManager.Instance.OnUpgradeBackpack += UpgradeBackpack;
        DataManager.Instance.OnUpgradeScythe += UpgradeScythe;

        UpdateInventory();
        UpdateCurrency();

        UpgradeBackpack();
        UpgradeScythe();
    }

    private void UpdateInventory()
    {
        var dataManager = DataManager.Instance;
        var inventory = dataManager.Data.Inventory;

        _inventoryView.ClearViews();
        foreach (var item in inventory)
        {
            _inventoryView.UpdateItemQuantity(item.Key, item.Value);
        }

        _inventoryCapacityText.text = $"{dataManager.InventoryCurrentLoad}/{dataManager.InventoryCapacity}";
    }

    private void UpdateCurrency()
    {
        var dataManager = DataManager.Instance;
        var currency = dataManager.Data.Currency;
        foreach (var coin in currency)
        {
            _priceView.UpdatePrice(coin.Key, coin.Value);
        }
    }

    private void UpgradeBackpack() 
    {
        var dataManager = DataManager.Instance;
        _backpackImage.sprite = dataManager.EquippedBackpack.Icon;
        _inventoryCapacityText.text = $"{dataManager.InventoryCurrentLoad}/{dataManager.InventoryCapacity}";
    }

    private void UpgradeScythe()
    {
        var dataManager = DataManager.Instance;
        _scytheImage.sprite = dataManager.EquippedScythe.Icon;
    }
}
