using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;

    [Header("FieldOfView")]
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;
    [SerializeField] private int level = 1;            // Add level variable
    [SerializeField] private int xp = 0;               // Add xp variable
    [SerializeField] private int xpToNextLevel = 100;  // Add xpToNextLevel variable

    public int MaxHitPoints { get => maxHitPoints; }
    public int HitPoints { get => hitPoints; }
    public int Defense { get => defense; }
    public int Power { get => power; }
    public int Level { get => level; }                // Getter for level
    public int XP { get => xp; }                      // Getter for xp
    public int XPToNextLevel { get => xpToNextLevel; } // Getter for xpToNextLevel

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(HitPoints, MaxHitPoints);
            UIManager.Get.UpdateLevel(Level);        // Update UI with level
            UIManager.Get.UpdateXP(XP);              // Update UI with xp
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }

    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }

    public void DoDamage(int hp, Actor attacker)       // Modified function signature
    {
        hitPoints -= hp;

        if (hitPoints < 0) hitPoints = 0;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
        }

        if (hitPoints == 0)
        {
            if (attacker.GetComponent<Player>())       // Check if attacker is player
            {
                attacker.AddXp(xp);                   // Add XP to the player
            }
            Die();
        }
    }

    public void Heal(int hp)
    {
        int actualHealing = Mathf.Min(hp, MaxHitPoints - HitPoints);
        hitPoints += actualHealing;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
            UIManager.Get.AddMessage($"You were healed for {actualHealing} HP!", Color.green);
        }
    }

    public void AddXp(int xpToAdd)
    {
        xp += xpToAdd;
        UIManager.Get.UpdateXP(XP);                 // Update UI with new xp

        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.FloorToInt(xpToNextLevel * 1.5f); // Increase xpToNextLevel exponentially
            maxHitPoints += 10;                     // Increase maxHitPoints
            defense += 2;                           // Increase defense
            power += 2;                             // Increase power

            UIManager.Get.UpdateLevel(Level);       // Update UI with new level
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
            UIManager.Get.AddMessage("You leveled up!", Color.yellow);
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.Get.AddMessage("You died!", Color.red); //Red
        }
        else
        {
            UIManager.Get.AddMessage($"{name} is dead!", Color.green); //Light Orange
        }
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        GameManager.Get.CreateGameObject("Dead", position).name = $"Remains of {name}";
        GameManager.Get.RemoveEnemy(this);
        Destroy(gameObject);
    }
}
