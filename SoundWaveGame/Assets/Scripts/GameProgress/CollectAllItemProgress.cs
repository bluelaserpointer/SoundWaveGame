using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CollectAllItemProgress : GameProgress
{
    [SerializeField]
    string _targetItemIdentifier;

    [Header("Debug")]
    [SerializeField]
    int _collectedCount;
    [SerializeField]
    int _requiredCount;

    public override string StateText => $"{CollectedCount} / {RequiredCount}";
    public int CollectedCount => _collectedCount;
    public int RequiredCount => _requiredCount;
    private void Start()
    {
        if (Item.StageItems.TryGetValue(_targetItemIdentifier, out List<Item> list))
        {
            _requiredCount = list.Count;
        }
        else
        {
            Debug.Log($"[{nameof(CollectAllItemProgress)}] No items named \"{_targetItemIdentifier}\"");
            _requiredCount = 0;
        }
        Item.ItemCollected.AddListener(identifier =>
        {
            if (!_targetItemIdentifier.Equals(identifier))
                return;
            ++_collectedCount;
            if (CollectedCount >= RequiredCount)
            {
                Complete();
            }
            onStateChange.Invoke();
        });
        onStateChange.Invoke();
    }
    public override float GetProgressRatio()
    {
        return _collectedCount / _requiredCount;
    }
}
