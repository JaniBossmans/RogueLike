using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Public list to hold Consumable items
    public List<Consumable> Items = new List<Consumable>();

    // Public variable to store the maximum number of items
    public int MaxItems;

    // Function to add an item to the inventory
    public bool AddItem(Consumable item)
    {
        if (Items.Count < MaxItems)
        {
            Items.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    // Function to remove an item from the inventory
    public void DropItem(Consumable item)
    {
        Items.Remove(item);
    }
}
