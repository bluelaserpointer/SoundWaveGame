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
    [SerializeField]
    Button _quitButton; // 新增的退出游戏按钮

    private void Awake()
    {
        Instance = this;
        _graphicRoot.SetActive(false);
        _retryButton.onClick.AddListener(() => GameManager.Instance.Retry());
        // 为退出按钮添加点击事件
        _quitButton.onClick.AddListener(() => QuitGame());
    }
    public void Show(bool cond)
    {
        _graphicRoot.SetActive(cond);
    }
    
    // 退出游戏的方法
    private void QuitGame()
    {
        #if UNITY_EDITOR
            // 如果在Unity编辑器中运行，停止播放
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 如果在打包后的游戏中，退出应用程序
            Application.Quit();
        #endif
    }
}
