using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
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

    
    // Start is called before the first frame update
    void Start()
    {

    }
    // 按钮点击音效
    void PlayButtonSound()
    {
        UISoundManager.Instance.PlayButtonSound();
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

    

}
