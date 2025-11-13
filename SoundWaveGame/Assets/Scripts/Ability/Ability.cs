using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class Ability : MonoBehaviour
{
    public readonly UnityEvent<string> onStatusChange = new UnityEvent<string>();
    public enum AbilityType { Knife, Beacon, Detector, Bait }
    public abstract AbilityType Type { get; }
    public abstract string Name { get; }
    public abstract bool IsUssable { get; }
    public abstract string UsageText { get; }
    public bool TryActivateAbility()
    {
        if (!IsUssable)
        {
            return false;
        }
        ActivateAbility();
        return true;
    }
    public abstract void ActivateAbility();
}
