using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class StageClearUI : MonoBehaviour
{
    public static StageClearUI Instance { get; private set; }
    [SerializeField]
    GameProgress _stageClearGameProgress;
    [SerializeField]
    GameObject _graphicRoot;
    [SerializeField]
    Button _retryButton;

    private void Awake()
    {
        Instance = this;
        _retryButton.onClick.AddListener(() => GameManager.Instance.Retry());
        _stageClearGameProgress.OnComplete.AddListener(() =>
        {
            _graphicRoot.SetActive(true);
            Player.Instance.Controllable = false;
        });
    }
}
