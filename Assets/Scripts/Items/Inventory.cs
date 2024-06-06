using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private int capacity;
    public List<Consumable> items;

    public Inventory(int capacity)
    {
        this.capacity = capacity;
        items = new List<Consumable>();
    }

    public bool IsFull()
    {
        return items.Count >= capacity;
    }

    public bool AddItem(Consumable item)
    {
        if (IsFull())
        {
            return false;
        }

        items.Add(item);
        return true;
    }

    public void RemoveItem(Consumable item)
    {
        items.Remove(item);
    }

    public List<Consumable> GetItems()
    {
        return items;
    }
}
