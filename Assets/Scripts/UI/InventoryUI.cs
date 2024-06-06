using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Label[] labels = new Label[8];
    private VisualElement root;
    private int selected;
    private int numItems;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Add labels to the array
        for (int i = 0; i < 8; i++)
        {
            labels[i] = root.Q<Label>("Item" + (i + 1));
        }

        Clear();
        Hide();
    }

    public int Selected { get => selected; }

    public void Clear()
    {
        foreach (var label in labels)
        {
            label.text = "";
        }
    }

    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = new StyleColor(Color.green);
            }
            else
            {
                labels[i].style.backgroundColor = new StyleColor(Color.clear);
            }
        }
    }

    public void SelectNextItem()
    {
        selected = Mathf.Min(selected + 1, numItems - 1);
        UpdateSelected();
    }

    public void SelectPreviousItem()
    {
        selected = Mathf.Max(selected - 1, 0);
        UpdateSelected();
    }

    public void Show(List<Consumable> list)
    {
        selected = 0;
        numItems = list.Count;
        Clear();

        // Set the text of the labels based on the list
        for (int i = 0; i < numItems && i < labels.Length; i++)
        {
            labels[i].text = list[i].name;
        }

        UpdateSelected();
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
