using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelMenuManager : MonoBehaviour
{
    // 按钮点击音效
    void PlayButtonSound()
    {
        StaticResources.Instance.ButtonClickSound.Play2dSound();
    }

    // === 关卡选择按钮功能 ===
    public void LoadLevel(string levelSceneName)
    {
        PlayButtonSound();
        GameManager.LoadLevel(levelSceneName);
    }
}
