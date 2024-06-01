using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
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
                    inventory.DropItem(selectedItem);
                    // Voeg hier logica toe om het item op de grond te plaatsen
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

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    private void UseItem(Consumable item)
    {
        if (item is HealthPotion)
        {
            HealthPotion healthPotion = (HealthPotion)item;
            GameManager.Get.Player.Heal(healthPotion.HealingAmount);
            UIManager.Get.AddMessage($"You used a Health Potion and gained {healthPotion.HealingAmount} HP!", Color.green);
        }
        else if (item is Fireball)
        {
            Fireball fireball = (Fireball)item;
            List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);

            foreach (Actor enemy in nearbyEnemies)
            {
                enemy.DoDamage(fireball.Damage);
            }

            UIManager.Get.AddMessage($"You used a Fireball and dealt {fireball.Damage} damage to nearby enemies!", Color.red);
        }
        else if (item is ScrollOfConfusion)
        {
            ScrollOfConfusion scroll = (ScrollOfConfusion)item;
            List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);

            foreach (Actor enemy in nearbyEnemies)
            {
                enemy.GetComponent<Enemy>().Confuse();
            }

            UIManager.Get.AddMessage($"You used a Scroll of Confusion and confused nearby enemies!", Color.yellow);
        }
    }

}
