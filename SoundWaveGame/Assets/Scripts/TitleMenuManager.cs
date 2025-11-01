using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Dock Thing";
    [SerializeField] private AudioClip buttonClickSound;
    
    public void StartGame()
    {
        // 可选：添加加载效果或音效
        if (buttonClickSound != null)
        {
            // 播放音效
        }
        
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void ExitGame()
    {
        // 可选：添加确认对话框
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
