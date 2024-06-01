using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject inventory; // Voeg het GameObject voor het inventory toe

    private InventoryUI inventoryUI; // Variabele voor het InventoryUI component

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

        // Zoek en sla het InventoryUI component op
        inventoryUI = inventory.GetComponent<InventoryUI>();

        // Start de inventaris door het InventoryUI-component te initialiseren
        inventoryUI.Initialize();
    }

    public static UIManager Get { get => instance; }

    // Public getter voor toegang tot het InventoryUI component
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
