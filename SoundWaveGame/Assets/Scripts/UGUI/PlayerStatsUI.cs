using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance { get; private set; }
    [SerializeField]
    TextMeshProUGUI _knifeCountText;

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateKnifeCount(int count)
    {
        _knifeCountText.text = count.ToString();
    }
}
