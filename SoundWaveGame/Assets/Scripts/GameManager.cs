using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //inspector
    [Header("World")]
    [SerializeField]
    Player player;
    
    [Header("UI")]
    [SerializeField]
    Text knifeAmountText;
    //--
    public static GameManager Instance { get; private set; }
    public static Player Player => Instance.player;
    public static Text KnifeAmountText => Instance.knifeAmountText;
    private void Awake()
    {
        Instance = this;
    }
}
