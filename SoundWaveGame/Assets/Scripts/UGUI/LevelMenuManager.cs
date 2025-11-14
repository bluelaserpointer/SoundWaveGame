using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelMenuManager : MonoBehaviour
{
    [Header("点击效果设置")]
    public Color clickedBorderColor = Color.red; // 点击后的边框颜色
    public float clickEffectDuration = 0.2f; // 点击效果显示时间
    public float clickedAlpha = 0.7f; // 点击状态透明度

    // 按钮点击音效
    void PlayButtonSound()
    {
        StaticResources.Instance.ButtonClickSound.Play2dSound();
    }

    // === 关卡选择按钮功能 ===
    public void LoadLevel(string levelSceneName)
    {
        PlayButtonSound();
        
        // 显示点击效果
        ShowClickEffect();
        
        // 延迟加载场景，让点击效果有足够时间显示
        StartCoroutine(LoadLevelAfterDelay(levelSceneName, clickEffectDuration));
    }

    // 显示点击效果 - 修改为通过EventSystem获取当前按钮
    private void ShowClickEffect()
    {
        // 获取当前被点击的按钮
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        
        if (clickedButton == null)
        {
            Debug.LogError("无法获取当前点击的按钮！");
            return;
        }
        
        Debug.Log("找到被点击的按钮: " + clickedButton.name);
        
        // 从被点击的按钮上获取ButtonHoverEffect组件
        ButtonHoverEffect hoverEffect = clickedButton.GetComponent<ButtonHoverEffect>();
        
        if (hoverEffect != null)
        {
            Debug.Log("成功获取按钮悬浮效果脚本！");
            // 使用ButtonHoverEffect的公共方法来设置点击效果
            hoverEffect.SetClickedBorderColor(clickedBorderColor);
            hoverEffect.ShowClickEffect(clickedAlpha);
        }
        
    }

    // 延迟加载场景
    private IEnumerator LoadLevelAfterDelay(string levelSceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.LoadLevel(levelSceneName);
    }

}