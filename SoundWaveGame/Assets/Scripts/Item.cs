using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Item : MonoBehaviour, IInteractable
{
    public virtual void Interact()
    {
        if (this == null)
            return;
        Player.Instance.AddItem(this);
    }
}
