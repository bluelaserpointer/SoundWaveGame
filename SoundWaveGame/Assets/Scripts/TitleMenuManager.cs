using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [Header("界面Canvas")]
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    [Header("关卡场景名称")]
    public string level1SceneName = "Dock Thing";
    public string level2SceneName = "Dock Thing";
    
    [Header("音效")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource;
    
    void Start()
    {
        // 确保开始时只显示主菜单
        ShowMainMenu();
        
        // 初始化音效
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    // === 主菜单按钮功能 ===
    
    // 开始游戏 - 显示关卡选择界面
    public void StartGame()
    {
        PlayButtonSound();
        ShowLevelSelection();
    }
    
    // 结束游戏
    public void ExitGame()
    {
        PlayButtonSound();
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

// === 关卡选择按钮功能 ===
    
    // 加载关卡1
    public void LoadLevel1()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level1SceneName))
        {
            SceneManager.LoadScene(level1SceneName);
        }
        else
        {
            Debug.LogError("关卡1场景名称未设置！");
        }
    }
    
    // 加载关卡2
    public void LoadLevel2()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level2SceneName))
        {
            SceneManager.LoadScene(level2SceneName);
        }
        else
        {
            Debug.LogError("关卡2场景名称未设置！");
        }
    }

    // 返回主菜单
    public void ReturnToMainMenu()
    {
        PlayButtonSound();
        ShowMainMenu();
    }


    // === 界面切换方法 ===
    
    // 显示主菜单
    private void ShowMainMenu()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(false);
    }
    
    // 显示关卡选择
    private void ShowLevelSelection()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(true);
    }

}
