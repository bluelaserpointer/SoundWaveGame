using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // 添加TMPro命名空间

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("文本设置")]
    public string normalText = "默认状态";
    public string hoverText = "悬浮中";
    
    private TextMeshProUGUI buttonText; // 改为TextMeshProUGUI类型
    
    void Start()
    {
        // 获取TextMeshProUGUI组件
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            normalText = buttonText.text; // 保存初始文本
            Debug.Log("找到TextMeshPro组件，初始文本: " + normalText);
        }
        else
        {
            Debug.LogError("未找到TextMeshProUGUI组件！");
        }
    }
    
    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.text = hoverText;
            Debug.Log("鼠标进入，文本改为: " + hoverText);
        }
    }
    
    // 鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.text = normalText;
            Debug.Log("鼠标离开，文本恢复: " + normalText);
        }
    }
}