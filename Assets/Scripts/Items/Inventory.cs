using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private int capacity;
    private List<GameObject> items;

    public Inventory(int capacity)
    {
        this.capacity = capacity;
        items = new List<GameObject>();
    }

    public bool IsFull()
    {
        return items.Count >= capacity;
    }

    public bool AddItem(GameObject item)
    {
        if (IsFull())
        {
            return false;
        }

        items.Add(item);
        return true;
    }

    public void RemoveItem(GameObject item)
    {
        items.Remove(item);
    }
}
