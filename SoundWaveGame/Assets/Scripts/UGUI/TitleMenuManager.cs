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
    public string level3SceneName = "Dock Thing";
    public string level4SceneName = "Dock Thing";
    public string level5SceneName = "Dock Thing";
    public string level6SceneName = "Dock Thing";
    public string level7SceneName = "Dock Thing";
    public string level8SceneName = "Dock Thing";
    public string level9SceneName = "Dock Thing";
    public string level10SceneName = "Dock Thing";

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

     // 加载关卡3
    public void LoadLevel3()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level3SceneName))
        {
            SceneManager.LoadScene(level3SceneName);
        }
        else
        {
            Debug.LogError("关卡3场景名称未设置！");
        }
    }

    // 加载关卡4
    public void LoadLevel4()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level4SceneName))
        {
            SceneManager.LoadScene(level4SceneName);
        }
        else
        {
            Debug.LogError("关卡4场景名称未设置！");
        }
    }

     // 加载关卡5
    public void LoadLevel5()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level5SceneName))
        {
            SceneManager.LoadScene(level5SceneName);
        }
        else
        {
            Debug.LogError("关卡5场景名称未设置！");
        }
    }

    // 加载关卡6
    public void LoadLevel6()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level6SceneName))
        {
            SceneManager.LoadScene(level6SceneName);
        }
        else
        {
            Debug.LogError("关卡6场景名称未设置！");
        }
    }

    // 加载关卡7
    public void LoadLevel7()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level7SceneName))
        {
            SceneManager.LoadScene(level7SceneName);
        }
        else
        {
            Debug.LogError("关卡7场景名称未设置！");
        }
    }

     // 加载关卡8
    public void LoadLevel8()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level8SceneName))
        {
            SceneManager.LoadScene(level8SceneName);
        }
        else
        {
            Debug.LogError("关卡8场景名称未设置！");
        }
    }

     // 加载关卡9
    public void LoadLevel9()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level9SceneName))
        {
            SceneManager.LoadScene(level9SceneName);
        }
        else
        {
            Debug.LogError("关卡9场景名称未设置！");
        }
    }

     // 加载关卡10
    public void LoadLevel10()
    {
        PlayButtonSound();
        if (!string.IsNullOrEmpty(level10SceneName))
        {
            SceneManager.LoadScene(level10SceneName);
        }
        else
        {
            Debug.LogError("关卡10场景名称未设置！");
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
