using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    
    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    public bool canPause = true;
    
    [Header("Events")]
    public UnityEvent onGamePaused; // 暂停事件
    public UnityEvent onGameResumed; // 恢复游戏事件
    public UnityEvent onGameRetry; // 重新开始事件
    
    private bool isPaused = false;
    public bool IsPaused => isPaused;
    private EventSystem eventSystem;
    
    void Update()
    {
        if (canPause && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    void PlayButtonSound()
    {
        UISoundManager.Instance.PlayButtonSound();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        
        isPaused = true;
        pausePanel?.SetActive(true);
        Time.timeScale = 0f;
        
        // 触发暂停事件
        onGamePaused?.Invoke();
        
        // 音频管理
        AudioListener.pause = true;

        // 解锁并显示鼠标光标
        SetCursorState(true);

        // 确保EventSystem启用
        if (eventSystem != null)
            eventSystem.enabled = true;

    }
    
    public void ResumeGame()
    {
        PlayButtonSound();

        if (!isPaused) return;
        
        isPaused = false;
        pausePanel?.SetActive(false);
        Time.timeScale = 1f;
        
        // 触发继续事件
        onGameResumed?.Invoke();
        
        // 恢复音频
        AudioListener.pause = false;

        // 重新锁定鼠标光标
        SetCursorState(false);
    }

    //重新开始游戏方法
    public void RetryGame()
    {
        PlayButtonSound();
        // 恢复时间尺度
        Time.timeScale = 1f;
        
        // 恢复音频
        AudioListener.pause = false;
        
        // 触发重新开始事件
        onGameRetry?.Invoke();
        
        // 获取当前场景名称并重新加载
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        // 注意：重新加载场景后，当前脚本实例会被销毁，新的实例会被创建
        // 所有游戏状态都会重置到场景初始状态
    }
    
    // 统一的鼠标状态设置方法
    private void SetCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ReturnToMainMenu()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Title");
    }
    
    public void QuitGame()
    {
        PlayButtonSound();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void Start()
    {
        // 获取或创建EventSystem
        eventSystem = FindObjectOfType<EventSystem>();
        
        // 确保暂停面板初始为隐藏状态
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        // 确保游戏时间正常
        Time.timeScale = 1f;
        
        // 初始鼠标状态（根据你的游戏类型调整）
        SetCursorState(false); // 游戏开始时锁定鼠标
    }
    
    // 防止对象被销毁
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}