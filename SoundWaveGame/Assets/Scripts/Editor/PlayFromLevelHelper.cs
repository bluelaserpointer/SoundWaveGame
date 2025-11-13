#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 在编辑器中：
/// 直接从关卡场景按下 Play 时，
/// 自动通过 GameManager.LoadLevel() 补上 System 场景。
/// 放在 Editor/ 目录下，不会进入构建。
/// </summary>
[InitializeOnLoad]
public static class PlayFromLevelHelper
{
    // 根据你的实际场景名字改
    private const string SystemSceneName = "System";
    private const string TitleSceneName = "Title";

    private const string EditorPrefsKey = "PlayFromLevelHelper_LevelName";

    // 静态构造函数，在 Editor 加载时自动注册回调
    static PlayFromLevelHelper()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                // 即将进入 Play 模式时，记录当前活动场景
                var activeScene = SceneManager.GetActiveScene();

                // 如果当前是 System 或 Title，就认为是正常启动，不做特殊处理
                if (activeScene.name == SystemSceneName || activeScene.name == TitleSceneName)
                {
                    EditorPrefs.DeleteKey(EditorPrefsKey);
                }
                else
                {
                    // 把当前场景名当作“关卡名”记下来
                    EditorPrefs.SetString(EditorPrefsKey, activeScene.name);
                }
                break;

            case PlayModeStateChange.EnteredPlayMode:
                // 正式进入 Play 模式后，检查是否需要通过 GameManager 加载关卡
                string levelName = EditorPrefs.GetString(EditorPrefsKey, string.Empty);

                if (!string.IsNullOrEmpty(levelName))
                {
                    // 调用你的静态接口，走正式流程：
                    // 1. 设置 CurrentLevelName
                    // 2. 加载 System 场景
                    // 3. GameManager.Awake()中再异步加载关卡
                    try
                    {
                        Debug.Log($"[{nameof(PlayFromLevelHelper)}] 正在对当前场景尝试补充GameManager。");
                        GameManager.LoadLevel(levelName);
                    }
                    catch
                    {
                        Debug.LogWarning(
                            $"[{nameof(PlayFromLevelHelper)}] 无法调用 GameManager.LoadLevel(\"{levelName}\")。\n" +
                            $"请确认 GameManager 脚本在 System 场景中，且类名/命名空间未改动。"
                        );
                    }
                }
                break;
        }
    }
}
#endif
