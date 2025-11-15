using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(vPickupItem))]
public class Item : MonoBehaviour, IInteractable
{
    public static UnityEvent<string> ItemCollected = new();
    [SerializeField]
    string _identifier;

    vPickupItem _pickupDetector;
    public static Dictionary<string, List<Item>> StageItems => ItemManager.Instance.stageItems;
    public static Dictionary<string, int> CollectedItemCounts => ItemManager.Instance.collectedItemCounts;

    public readonly UnityEvent onPickup = new(); 
    public string Identifier => _identifier;
    private void Awake()
    {
        _pickupDetector = GetComponent<vPickupItem>();
        _pickupDetector.OnPickup.AddListener(() => onPickup.Invoke());
    }
    public void Init()
    {
        List<Item> sameIdentifierItems;
        if (StageItems.TryGetValue(_identifier, out var list))
        {
            sameIdentifierItems = list;
        }
        else
        {
            sameIdentifierItems = new List<Item>();
            StageItems.Add(_identifier, sameIdentifierItems);
        }
        sameIdentifierItems.Add(this);
    }
    public virtual void Interact()
    {
        if (this == null)
            return;
        Player.Instance.AddItem(this);
    }
    public virtual void Collected()
    {
        if (CollectedItemCounts.ContainsKey(_identifier))
        {
            ++CollectedItemCounts[_identifier];
        }
        else
        {
            CollectedItemCounts[_identifier] = 1;
        }
        ItemCollected.Invoke(_identifier);
    }
}
