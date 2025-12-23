using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _itemsContainer;
    [SerializeField] private RectTransform _coinsContainer;

    [SerializeField] private UIItemView _itemViewPrefab;
    [SerializeField] private UICoinView _coinViewPrefab;

    [SerializeField] private Image _backpackImage;
    [SerializeField] private Image _scytheImage;
    [SerializeField] private TextMeshProUGUI _inventoryCapacityText;

    private Dictionary<string, UIItemView> _activeItemViews = new Dictionary<string, UIItemView>();
    private Dictionary<string, UICoinView> _activeCoinViews = new Dictionary<string, UICoinView>();

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
        foreach (var item in inventory)
        {
            var view = GetActiveItemView(item.Key);
            view.UpdateQuantity(item.Value);
            view.gameObject.SetActive(item.Value > 0);
        }

        _inventoryCapacityText.text = $"{dataManager.InventoryCurrentLoad}/{dataManager.InventoryCapacity}";
    }

    private void UpdateCurrency()
    {
        var dataManager = DataManager.Instance;
        var currency = dataManager.Data.Currency;
        foreach (var coin in currency)
        {
            var view = GetActiveCoinView(coin.Key);
            view.UpdateQuantity(coin.Value);
        }
    }

    private void UpgradeBackpack() 
    {
        var dataManager = DataManager.Instance;
        _backpackImage.sprite = dataManager.EquippedBackpack.BackpackIcon;
        _inventoryCapacityText.text = $"{dataManager.InventoryCurrentLoad}/{dataManager.InventoryCapacity}";
    }

    private void UpgradeScythe()
    {
        var dataManager = DataManager.Instance;
        _scytheImage.sprite = dataManager.EquippedScythe.ScytheIcon;
    }

    private UIItemView GetActiveItemView(string itemId)
    {
        if (_activeItemViews.ContainsKey(itemId)) return _activeItemViews[itemId];
        var newView = PoolingManager.Instance.Pool(_itemViewPrefab, _itemsContainer, Vector3.zero, Quaternion.identity);
        _activeItemViews.Add(itemId, newView);
        newView.SetIcon(DataManager.Instance.CollectableObjectConfigs.FirstOrDefault(obj => obj.Id == itemId).InventoryIcon);
        return newView;
    }

    private UICoinView GetActiveCoinView(string coinId)
    {
        if (_activeCoinViews.ContainsKey(coinId)) return _activeCoinViews[coinId];
        var newView = PoolingManager.Instance.Pool(_coinViewPrefab, _coinsContainer, Vector3.zero, Quaternion.identity);
        _activeCoinViews.Add(coinId, newView);
        newView.SetIcon(DataManager.Instance.CollectableObjectConfigs.FirstOrDefault(obj => obj.Id == coinId).InventoryIcon);
        return newView;
    }
}
