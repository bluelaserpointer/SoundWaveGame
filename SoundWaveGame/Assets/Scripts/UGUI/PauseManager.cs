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
    public UnityEvent<bool> onGamePaused; // 暂停状态切换

    public static PauseManager Instance { get; private set; }
    private bool isPaused = false;
    public bool IsPaused => isPaused;
    private EventSystem eventSystem;

    void Awake()
    {
        Instance = this;
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
    }
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
        StaticResources.Instance.ButtonClickSound.Play2dSound();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        
        isPaused = true;
        pausePanel?.SetActive(true);
        Time.timeScale = 0f;
        
        // 触发暂停事件
        onGamePaused.Invoke(true);
        
        // 音频管理
        AudioListener.pause = true;

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
        onGamePaused.Invoke(false);

        // 恢复音频
        AudioListener.pause = false;
    }

    //重新开始游戏方法
    public void RetryGame()
    {
        ResumeGame();
        GameManager.Instance.ResetLevel();
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

}