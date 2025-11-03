using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // 添加UI命名空间用于获取Image组件
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("材质设置")]
    public Material defaultMaterial; // 默认材质
    public Material hoverMaterial;   // 悬浮时材质
    
    [Header("文本设置")]
    public string normalText = "默认状态";
    public string hoverText = "悬浮中";
    
    private TextMeshProUGUI buttonText;
    private Image buttonImage; // 用于获取按钮的Image组件
    


    void Start()
    {
        // 获取TextMeshProUGUI组件
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        // 获取Image组件
        buttonImage = GetComponent<Image>();
        
        if (buttonText != null)
        {
            normalText = buttonText.text; // 保存初始文本
            Debug.Log("找到TextMeshPro组件，初始文本: " + normalText);
        }
        else
        {
            Debug.LogWarning("未找到TextMeshProUGUI组件！");
        }
        
        if (buttonImage == null)
        {
            Debug.LogWarning("未找到Image组件！");
        }
    }
    
    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 切换材质
        if (buttonImage != null && hoverMaterial != null)
        {
            buttonImage.material = hoverMaterial;
            Debug.Log("鼠标进入，材质已切换为悬浮材质");
        }
        
        // 切换文本
        if (buttonText != null)
        {
            buttonText.text = hoverText;
            Debug.Log("鼠标进入，文本改为: " + hoverText);
        }
    }
    
    // 鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {

        // 从Resources文件夹加载图片
        Sprite loadedSprite = Resources.Load<Sprite>("Assets/Resources/UIRes/关卡1缩略图.png");


        // 恢复默认材质
        if (buttonImage != null && defaultMaterial != null)
        {
            buttonImage.material = null;
            buttonImage.sprite = loadedSprite;
            Debug.Log("鼠标离开，材质已恢复为默认材质");
        }
        
        // 恢复默认文本
        if (buttonText != null)
        {
            buttonText.text = normalText;
            Debug.Log("鼠标离开，文本恢复: " + normalText);
        }
    }
}