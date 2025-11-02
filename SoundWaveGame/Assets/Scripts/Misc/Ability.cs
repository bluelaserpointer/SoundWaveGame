using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    public readonly UnityEvent<string> onStatusChange = new UnityEvent<string>();
    public abstract string Name { get; }
    public abstract bool IsUssable { get; }
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
