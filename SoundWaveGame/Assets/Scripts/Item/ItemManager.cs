using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemManager
{
    public static ItemManager Instance { get; private set; }
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
