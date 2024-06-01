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
    private Dictionary<Vector3, GameObject> itemsOnMap = new Dictionary<Vector3, GameObject>();


    public GameObject CreateGameObject(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
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

    private List<Consumable> items = new List<Consumable>();

    // Function to add an item to the list
    public void AddItem(Consumable item)
    {
        if (item != null)
        {
            items.Add(item);
        }
    }

    // Function to remove an item from the list
    public void RemoveItem(Consumable item)
    {
        if (item != null)
        {
            items.Remove(item);
        }
    }

    // Function to get an item at a specific location in the list
    public Consumable GetItemAtLocation(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public GameObject GetItemAtPosition(Vector3 position)
    {
        itemsOnMap.TryGetValue(position, out GameObject item);
        return item;
    }

    public void RemoveItemAtPosition(Vector3 position)
    {
        if (itemsOnMap.ContainsKey(position))
        {
            itemsOnMap.Remove(position);
        }
    }

    public void AddItemAtPosition(Vector3 position, GameObject item)
    {
        if (!itemsOnMap.ContainsKey(position))
        {
            itemsOnMap[position] = item;
        }
    }

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        List<Actor> nearbyEnemies = new List<Actor>();

        // Itereer door alle vijanden
        foreach (Actor enemy in Enemies)
        {
            // Bereken de afstand tussen de gegeven locatie en de locatie van de vijand
            float distance = Vector3.Distance(location, enemy.transform.position);

            // Als de afstand minder is dan 5, voeg de vijand toe aan de lijst van nabije vijanden
            if (distance < 5f)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        return nearbyEnemies;
    }
}
