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
    public UnityEvent onGamePaused;
    public UnityEvent onGameResumed;
    
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
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Title");
    }
    
    public void QuitGame()
    {
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