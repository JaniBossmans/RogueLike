using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject inventory; // Reference to the inventory GameObject

    private InventoryUI inventoryUI; // Variable for the InventoryUI component

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
        inventoryUI = inventory.GetComponent<InventoryUI>();

        // Remove this line since the Initialize method doesn't exist
        // inventoryUI.Initialize();
    }

    public static UIManager Get { get => instance; }

    // Public getter for accessing the InventoryUI component
    public InventoryUI InventoryUI { get => inventoryUI; }

    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }
}
