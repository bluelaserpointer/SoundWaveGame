using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemManager
{
    public static ItemManager Instance { get; private set; }

    public readonly Dictionary<string, List<Item>> stageItems = new();
    public readonly Dictionary<string, int> collectedItemCounts = new();
    public void Init()
    {
        Instance = this;
        foreach (GameObject itemObj in GameObject.FindGameObjectsWithTag("Item"))
        {
            if (itemObj.TryGetComponent(out Item item))
            {
                item.Init();
            }
        }
    }
}
