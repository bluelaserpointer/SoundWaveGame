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
    Button _titleButton; // 新增的返回主菜单游戏按钮
    [SerializeField]
    Button _quitButton; // 新增的退出游戏按钮

    private void Awake()
    {
        Instance = this;
        _graphicRoot.SetActive(false);
        _retryButton.onClick.AddListener(() => GameManager.Instance.Retry());
        // 为返回主菜单按钮添加点击事件
        _titleButton.onClick.AddListener(() => BackToTitle());
        // 为退出按钮添加点击事件
        _quitButton.onClick.AddListener(() => QuitGame());
    }
    public void Show()
    {
        _graphicRoot.SetActive(true);
    }
    
    // 返回主菜单的方法
    private void BackToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
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
