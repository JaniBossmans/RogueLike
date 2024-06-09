using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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
    }

    public static GameManager Get { get => instance; }

    public Actor Player;
    public List<Actor> Enemies = new List<Actor>();
    public Dictionary<Vector3, Consumable> Items = new Dictionary<Vector3, Consumable>();
    public List<Ladder> Ladders = new List<Ladder>();

    public GameObject CreateGameObject(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
        UIManager.Get.UpdateEnemies(Enemies.Count); // Update enemies count
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            UIManager.Get.UpdateEnemies(Enemies.Count); // Update enemies count
        }
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<Enemy>().RunAI();
        }
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player.transform.position == location)
        {
            return Player;
        }
        else
        {
            foreach (Actor enemy in Enemies)
            {
                if (enemy.transform.position == location)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    public void AddItem(Consumable item)
    {
        if (item != null)
        {
            Items[item.transform.position] = item;
        }
    }

    public void RemoveItem(Consumable item)
    {
        if (item != null)
        {
            Items.Remove(item.transform.position);
        }
    }

    public Consumable GetItemAtLocation(Vector3 location)
    {
        Items.TryGetValue(location, out Consumable item);
        return item;
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public Consumable GetItemAtPosition(Vector3 position)
    {
        Items.TryGetValue(position, out Consumable item);
        return item;
    }

    public void RemoveItemAtPosition(Vector3 position)
    {
        if (Items.ContainsKey(position))
        {
            Items.Remove(position);
        }
    }

    public void AddItemAtPosition(Vector3 position, Consumable item)
    {
        if (!Items.ContainsKey(position))
        {
            Items[position] = item;
        }
    }

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        List<Actor> nearbyEnemies = new List<Actor>();

        foreach (Actor enemy in Enemies)
        {
            float distance = Vector3.Distance(location, enemy.transform.position);

            if (distance < 5f)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        return nearbyEnemies;
    }

    public void AddLadder(Ladder ladder)
    {
        if (!Ladders.Contains(ladder))
        {
            Ladders.Add(ladder);
        }
    }

    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (Ladder ladder in Ladders)
        {
            if (ladder.transform.position == location)
            {
                return ladder;
            }
        }
        return null;
    }

    public Actor FindPlayer()
    {
        return Player;
    }

    public void ClearAllObjects()
    {
        // Clear all enemies
        foreach (var enemy in Enemies)
        {
            Destroy(enemy.gameObject);
        }
        Enemies.Clear();

        // Clear all items
        foreach (var item in Items.Values)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();

        // Update UI
        UIManager.Get.UpdateEnemies(Enemies.Count);
    }
}
