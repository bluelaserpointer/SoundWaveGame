using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUsageSupply : Item
{
    [SerializeField]
    Ability.AbilityType _abilityType;
    [SerializeField]
    int _supplyKnifeCount;
    public override void Collected()
    {
        base.Collected();
        Player.Instance.RestoreAbilityUsage(_abilityType, _supplyKnifeCount);
    }
}
