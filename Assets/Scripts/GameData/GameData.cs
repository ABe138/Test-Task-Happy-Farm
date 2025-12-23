using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public Dictionary<string, int> Inventory;
    public Dictionary<string, int> Currency;
    public int ScytheLevel;
    public int InventoryCapacityLevel;

    public GameData() 
    { 
        Inventory = new Dictionary<string, int>();
        Currency = new Dictionary<string, int>();
        ScytheLevel = 0;
        InventoryCapacityLevel = 0;
    }
}