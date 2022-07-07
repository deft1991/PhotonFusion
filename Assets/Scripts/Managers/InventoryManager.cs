using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }
    public string EquippedItem { get; private set; }

    private Dictionary<string, int> _items;

    public void Startup(NetworkService networkService)
    {
        Debug.Log("Inventory manager starting...");

        /*
         * Initialize empty inventory
         */
        UpdateData(new Dictionary<string, int>());
       
        Status = ManagerStatus.Started;
        Debug.Log("InventoryManager: started");
    }

    /*
     * Updata data in inventory
     */
    public void UpdateData(Dictionary<string, int> items)
    {
        _items = items;

    }

    /*
     * Get inventory data
     */
    public Dictionary<string, int> GetData()
    {
        return _items;
    }

    /**
     * Show current inventory
     */
    private void DisplayItems()
    {
        string itemDisplay = "Items: ";
        foreach (KeyValuePair<string, int> item in _items)
        {
            itemDisplay += item.Key + " (" + item.Value + ") ";
        }

        Debug.Log(itemDisplay);
    }

    /**
     * Other scripts cannot change item list
     * but can call this method
     */
    public void AddItem(string itemName)
    {
        /*
         * Check that we already have an item
         */
        if (_items.ContainsKey(itemName))
        {
            _items[itemName] += 1;
        }
        else
        {
            _items[itemName] = 1;
        }

        DisplayItems();
    }

    public bool ConsumeItem(string itemName)
    {
        /*
         * Check that we have needed element
         */
        if (_items.ContainsKey(itemName))
        {
            _items[itemName]--;
            /*
             * Remove item if was used last one
             */
            if (_items[itemName] == 0)
            {
                _items.Remove(itemName);
            }
        }
        else
        {
            Debug.Log("Cannot consume: " + itemName);
            return false;
        }

        DisplayItems();
        return true;
    }

    /**
     * Return all keys in dictionary
     */
    public List<string> GetItemList()
    {
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    /**
     * Return item count in inventory
     */
    public int GetItemCount(string itemName)
    {
        if (_items.ContainsKey(itemName))
        {
            return _items[itemName];
        }

        return 0;
    }

    public bool EquipItem(string itemName)
    {
        if (_items.ContainsKey(itemName) && EquippedItem != itemName)
        {
            EquippedItem = itemName;
            Debug.Log("Equipped: " + itemName);
            return true;
        }

        EquippedItem = null;
        Debug.Log("Unequipped");
        return false;
    }
}