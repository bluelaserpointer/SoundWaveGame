using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance { get; private set; }
    [SerializeField]
    TextMeshProUGUI _abilityUsageText;

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateAbilityUsageText()
    {
        _abilityUsageText.text = Player.Instance.Ability.UsageText;
    }
}
