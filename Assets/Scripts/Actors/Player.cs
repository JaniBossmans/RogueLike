using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;

    public Inventory inventory;

    private void Awake()
    {
        controls = new Controls();
        inventory = new Inventory(10); // Bijvoorbeeld, capaciteit van 10 items
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        GameManager.Get.Player = GetComponent<Actor>();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();

        // Register the ladder interaction
        controls.Player.Ladder.performed += OnLadder;
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();

        // Unregister the ladder interaction
        controls.Player.Ladder.performed -= OnLadder;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (inventoryIsOpen)
        {
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
            if (direction.y > 0)
            {
                UIManager.Get.InventoryUI.SelectPreviousItem();
            }
            else if (direction.y < 0)
            {
                UIManager.Get.InventoryUI.SelectNextItem();
            }
        }
        else if (context.performed)
        {
            Move();
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
            else
            {
                // Voeg hier andere logica toe voor het verlaten van het spel of iets anders
            }
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var item = GameManager.Get.GetItemAtLocation(transform.position)?.GetComponent<Consumable>();
            if (item != null)
            {
                if (inventory.AddItem(item))
                {
                    item.gameObject.SetActive(false);
                    GameManager.Get.RemoveItem(item);
                    UIManager.Get.AddMessage($"You've picked up a {item.name}.", Color.yellow);
                }
                else
                {
                    UIManager.Get.AddMessage("Your inventory is full.", Color.red);
                }
            }
            else
            {
                UIManager.Get.AddMessage("You could not find anything.", Color.yellow);
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed && !inventoryIsOpen)
        {
            UIManager.Get.InventoryUI.Show(inventory.GetItems());
            inventoryIsOpen = true;
            droppingItem = true;
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed && !inventoryIsOpen)
        {
            UIManager.Get.InventoryUI.Show(inventory.GetItems());
            inventoryIsOpen = true;
            usingItem = true;
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryIsOpen)
        {
            var selectedItems = inventory.GetItems();
            if (selectedItems.Count > 0)
            {
                var selectedItem = selectedItems[UIManager.Get.InventoryUI.Selected];
                if (droppingItem)
                {
                    inventory.RemoveItem(selectedItem);
                    // Add logic to place the item on the ground
                }
                else if (usingItem)
                {
                    UseItem(selectedItem);
                    Destroy(selectedItem.gameObject);
                }

                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
        }
    }

    public void OnLadder(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 playerPosition = transform.position;
            Ladder ladder = GameManager.Get.GetLadderAtLocation(playerPosition);

            if (ladder != null)
            {
                if (ladder.Up)
                {
                    MapManager.Get.MoveUp();
                }
                else
                {
                    MapManager.Get.MoveDown();
                }
            }
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    private void UseItem(Consumable item)
    {
        switch (item.Type)
        {
            case Consumable.ItemType.HealthPotion:
                int healingAmount = 50; // Set your actual healing amount
                GameManager.Get.Player.Heal(healingAmount);
                UIManager.Get.AddMessage($"You used a Health Potion and gained {healingAmount} HP!", Color.green);
                break;

            case Consumable.ItemType.Fireball:
                int fireballDamage = 30; // Set your actual fireball damage
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    enemy.DoDamage(fireballDamage, GameManager.Get.Player);  // Pass the attacker as the second argument
                }
                UIManager.Get.AddMessage($"You used a Fireball and dealt {fireballDamage} damage to nearby enemies!", Color.red);
                break;

            case Consumable.ItemType.ScrollOfConfusion:
                List<Actor> enemiesToConfuse = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in enemiesToConfuse)
                {
                    enemy.GetComponent<Enemy>().Confuse();
                }
                UIManager.Get.AddMessage($"You used a Scroll of Confusion and confused nearby enemies!", Color.yellow);
                break;

            default:
                Debug.LogWarning("Unknown item type used.");
                break;
        }
    }
}
