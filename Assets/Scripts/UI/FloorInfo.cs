using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloorInfo : MonoBehaviour
{
    private Label floorLabel;
    private Label enemiesLabel;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        floorLabel = root.Q<Label>("Floor");
        enemiesLabel = root.Q<Label>("Enemies");
    }

    public void UpdateFloor(int floor)
    {
        if (floorLabel != null)
        {
            floorLabel.text = $"Floor {floor}";
        }
    }

    public void UpdateEnemies(int enemyCount)
    {
        if (enemiesLabel != null)
        {
            enemiesLabel.text = $"{enemyCount} enemies left";
        }
    }
}
