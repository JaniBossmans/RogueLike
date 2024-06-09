using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;
    private Label levelLabel;  // Add a reference for level label
    private Label xpLabel;     // Add a reference for xp label

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        healthBar = root.Q<VisualElement>("HealthBar");
        healthLabel = root.Q<Label>("HealthText");
        levelLabel = root.Q<Label>("LevelText");  // Initialize level label
        xpLabel = root.Q<Label>("XPText");        // Initialize xp label
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        float percent = (float)currentHitPoints / maxHitPoints * 100;
        healthBar.style.width = Length.Percent(percent);
        healthLabel.text = $"{currentHitPoints} / {maxHitPoints} HP";
    }

    public void SetLevel(int level)
    {
        levelLabel.text = $"Level: {level}";
    }

    public void SetXP(int xp)
    {
        xpLabel.text = $"XP: {xp}";
    }
}
