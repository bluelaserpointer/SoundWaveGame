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
    [SerializeField]
    TextMeshProUGUI _mainGameProgressText;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Level.Instance.MainGameProgress.onStateChange.AddListener(UpdateMainGameProgressText);
    }
    public void UpdateMainGameProgressText()
    {
        _mainGameProgressText.text = Level.Instance.MainGameProgress.StateText;
    }
    public void UpdateAbilityUsageText()
    {
        _abilityUsageText.text = Player.Instance.Ability.UsageText;
    }
}
