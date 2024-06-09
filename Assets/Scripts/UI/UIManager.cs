using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject Inventory; // Reference to the inventory GameObject
    public GameObject FloorInfo; // Reference to the FloorInfo GameObject

    private InventoryUI inventoryUI; // Variable for the InventoryUI component
    private FloorInfo floorInfo; // Variable for the FloorInfo component

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Find and store the InventoryUI component
        inventoryUI = Inventory.GetComponent<InventoryUI>();

        // Find and store the FloorInfo component
        floorInfo = FloorInfo.GetComponent<FloorInfo>();
    }

    public static UIManager Get { get => instance; }

    // Public getter for accessing the InventoryUI component
    public InventoryUI InventoryUI { get => inventoryUI; }

    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void UpdateLevel(int level)
    {
        HealthBar.GetComponent<HealthBar>().SetLevel(level);
    }

    public void UpdateXP(int xp)
    {
        HealthBar.GetComponent<HealthBar>().SetXP(xp);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }

    public void UpdateFloor(int floor)
    {
        if (floorInfo != null)
        {
            floorInfo.UpdateFloor(floor);
        }
    }

    public void UpdateEnemies(int enemyCount)
    {
        if (floorInfo != null)
        {
            floorInfo.UpdateEnemies(enemyCount);
        }
    }
}
