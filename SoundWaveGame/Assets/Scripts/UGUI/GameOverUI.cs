using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }
    [SerializeField]
    GameObject _graphicRoot;
    [SerializeField]
    Button _retryButton;

    private void Awake()
    {
        Instance = this;
        _graphicRoot.SetActive(false);
        _retryButton.onClick.AddListener(() => GameManager.Instance.Retry());
    }
    public void Show(bool cond)
    {
        _graphicRoot.SetActive(cond);
    }
}
