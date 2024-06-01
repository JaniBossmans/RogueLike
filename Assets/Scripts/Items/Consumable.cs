using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    // Define the ItemType enum
    public enum ItemType
    {
        HealthPotion,
        Fireball,
        ScrollOfConfusion
    }

    // Private variable to store the item type
    [SerializeField]
    private ItemType type;

    // Public getter to access the item type
    public ItemType Type
    {
        get { return type; }
    }
  
    void Start()
    {
        // Find the GameManager in the scene
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Add this Consumable to the GameManager's list of items
            gameManager.AddItem(this);
        }
        else
        {
            Debug.LogWarning("GameManager not found in the scene.");
        }
    }
}
