using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static string CurrentLevelName { get; private set; }
    public readonly UnityEvent onGameEnd = new();
    public bool IsGameEnd { get; private set; }
    private void Awake()
    {
        Instance = this;

        // 使用协程异步加载关卡场景
        StartCoroutine(LoadLevelAsync(CurrentLevelName));
    }
    public static void LoadLevel(string sceneName)
    {
        //TODO: 卸载当前关卡场景（如果有关卡内跳转其它关卡）
        CurrentLevelName = sceneName;
        // 首先加载System场景（包含GameManager本体）
        SceneManager.LoadScene("System", LoadSceneMode.Single);
        /*
        // 如果System场景已经加载，则只加载关卡
        Scene systemScene = SceneManager.GetSceneByName("System");
        if (!systemScene.isLoaded)
        {
            SceneManager.LoadScene("System", LoadSceneMode.Single);
        }
        */
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        // 加载目标关卡场景到System之上
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return async;

        // 设置激活场景为关卡（避免摄像机/灯光受System场景影响）
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    /// <summary>
    /// 重置当前关卡（卸载后重新加载）
    /// </summary>
    public void ResetLevel()
    {
        LoadLevel(CurrentLevelName);
        //以后尝试只更新关卡部分
        /*
        if (string.IsNullOrEmpty(CurrentLevelName))
        {
            Debug.LogWarning("No level loaded to reset.");
            return;
        }

        Instance.StartCoroutine(Instance.ReloadLevelAsync());
        */
    }

    private IEnumerator ReloadLevelAsync()
    {
        string sceneName = CurrentLevelName;

        // 卸载当前关卡
        AsyncOperation unload = SceneManager.UnloadSceneAsync(sceneName);
        yield return unload;

        // 重新加载
        AsyncOperation reload = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return reload;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
    public void BackToTile()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Title");
    }
    public void GameOver()
    {
        GameOverUI.Instance.Show(true);
        IsGameEnd = true;
        onGameEnd.Invoke();
    }
    public void GameClear()
    {
        GameClearUI.Instance.Show(true);
        IsGameEnd = true;
        onGameEnd.Invoke();
    }
}
