using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    [field: SerializeField] public List<InventoryUpgradeConfig> InventoryUpgradeConfigs { get; private set; }
    [field: SerializeField] public List<ScytheUpgradeConfig> ScytheUpgradeConfigs { get; private set; }
    [field: SerializeField] public List<CollectableConfig> CollectableObjectConfigs { get; private set; }

    public GameData Data { get; private set; }

    public Action OnUpdateInventory;
    public Action OnUpdateCurrency;

    public Action OnUpgradeScythe;
    public Action OnUpgradeBackpack;

    public int InventoryCurrentLoad => Data.Inventory.Sum(i => i.Value);
    public int InventoryCapacity => InventoryUpgradeConfigs[Data.InventoryCapacityLevel].Capacity;

    public ScytheUpgradeConfig EquippedScythe => ScytheUpgradeConfigs[Data.ScytheLevel];
    public InventoryUpgradeConfig EquippedBackpack => InventoryUpgradeConfigs[Data.InventoryCapacityLevel];

    readonly string _saveFile = "gamedata.json";
    private string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        Data = TryLoadGameData() ?? new GameData();

        foreach (var config in CollectableObjectConfigs)
        {
            //just to initialize currency icons:
            AddCurrency(config.Id, 0);
        }

        SetInventoryLevel(Data.InventoryCapacityLevel);
        SetScytheLevel(Data.ScytheLevel);
    }

    private GameData TryLoadGameData()
    {
        var path = GetPath(_saveFile);
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        try { return JsonConvert.DeserializeObject<GameData>(json); }
        catch { return null; }
    }

    private void SaveGameData()
    {
        var json = JsonConvert.SerializeObject(Data);
        File.WriteAllText(GetPath(_saveFile), json);
    }

    public void AddItem(string itemId, int amount)
    {
        var remainingSpace = InventoryCapacity - InventoryCurrentLoad;
        if (remainingSpace == 0) return;
        if (Data.Inventory.ContainsKey(itemId))
        {
            Data.Inventory[itemId] += Math.Min(remainingSpace, amount);
        }
        else
        {
            Data.Inventory.Add(itemId, Math.Min(remainingSpace, amount));
        }
        SaveGameData();
        OnUpdateInventory?.Invoke();
    }

    public bool TryWithdrawItems(string itemId, int amount)
    {
        if (Data.Inventory.ContainsKey(itemId) && Data.Inventory[itemId] >= amount)
        {
            Data.Inventory[itemId] -= amount;
            if (Data.Inventory[itemId] <= 0) Data.Inventory.Remove(itemId);
            SaveGameData();
            OnUpdateInventory?.Invoke();
            return true;
        }
        return false;
    }

    public int GetItemValue(string itemId)
    {
        var config = DataManager.Instance.CollectableObjectConfigs.FirstOrDefault(obj => obj.Id == itemId);
        return config.Value;
    }

    public void AddCurrency(string coinId, int amount)
    {
        if (Data.Currency.ContainsKey(coinId))
        {
            Data.Currency[coinId] += amount;
        }
        else
        {
            Data.Currency.Add(coinId, amount);
        }
        SaveGameData();
        OnUpdateCurrency?.Invoke();
    }

    private bool CanAffordPurchase(Dictionary<string, int> price)
    {
        var canAfford = true;
        foreach (var kvp in price)
        {
            canAfford = canAfford && Data.Currency.ContainsKey(kvp.Key) && Data.Currency[kvp.Key] >= kvp.Value;
        }
        return canAfford;
    }

    public bool TryWithdrawCurrency(Dictionary<string, int> price)
    {
        if (CanAffordPurchase(price))
        {
            foreach (var kvp in price)
            {
                if (Data.Currency.ContainsKey(kvp.Key))
                {
                    Data.Currency[kvp.Key] -= kvp.Value;
                }
            }
            SaveGameData();
            OnUpdateCurrency?.Invoke();
            return true;
        }
        return false;
    }

    public void SetInventoryLevel(int level)
    {
        Data.InventoryCapacityLevel = level;
        OnUpgradeBackpack?.Invoke();
    }

    public void SetScytheLevel(int level)
    {
        Data.ScytheLevel = level;
        OnUpgradeScythe?.Invoke();
    }
}
