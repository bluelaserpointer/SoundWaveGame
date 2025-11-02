using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSupply : Item
{
    [SerializeField]
    int _supplyKnifeCount;
    public override void Collected()
    {
        base.Collected();
        GameManager.Instance.onKnifeAdd.Invoke(_supplyKnifeCount);
    }
}
