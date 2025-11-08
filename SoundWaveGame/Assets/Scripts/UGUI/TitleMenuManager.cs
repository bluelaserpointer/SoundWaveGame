using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [Header("界面Canvas")]
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;

    
    void Start()
    {
        // 确保开始时只显示主菜单
        ShowMainMenu();
    }
    

    // 按钮点击音效
    void PlayButtonSound()
    {
        StaticResources.Instance.ButtonClickSound.Play2dSound();
    }

    // === 主菜单按钮功能 ===
    
    // 开始游戏 - 显示关卡选择界面
    public void StartGame()
    {
        PlayButtonSound();
        ShowLevelSelection();
    }
    
    // 加载游戏 - 显示存档选择界面
    public void LoadGame()
    {
        PlayButtonSound();
    }

    // 设置 - 显示设置界面
    public void Setting()
    {
        PlayButtonSound();
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


    // 返回主菜单
    public void ReturnToMainMenu()
    {
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
        StaticResources.Instance.PanelSwitchSound.Play2dSound();
    }
    
    // 显示关卡选择
    private void ShowLevelSelection()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(true);
        StaticResources.Instance.PanelSwitchSound.Play2dSound();
    }

}
