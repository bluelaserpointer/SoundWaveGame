using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // 添加UI命名空间用于获取Image组件
using TMPro;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  
    [Header("文本设置")]
    public string normalText = "默认状态";
    public string hoverText = "悬浮中";
    
    private TextMeshProUGUI buttonText;
    private Image buttonImage; // 用于获取按钮的Image组件
    
    [Header("Border Settings")]
    public Color borderColor = Color.white;
    public float borderWidth = 0.05f;
    public float waveSpeed = 2.0f;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.3f;

    private Material buttonMaterial;
    private Coroutine animationCoroutine;
    private bool isMouseOver = false;


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

        // 创建材质实例
        buttonMaterial = new Material(Shader.Find("UI/ButtonBorder"));
        buttonImage.material = buttonMaterial;
        
        // 设置初始属性
        buttonMaterial.SetColor("_BorderColor", Color.clear);
        buttonMaterial.SetFloat("_BorderWidth", borderWidth);
        buttonMaterial.SetFloat("_WaveSpeed", waveSpeed);
        buttonMaterial.SetFloat("_Progress", 0f);
    }
    
    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        buttonMaterial.SetColor("_BorderColor", borderColor);
        animationCoroutine = StartCoroutine(AnimateBorder(1f, 0f, fadeOutDuration));
        
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
        isMouseOver = false;
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateBorder(0f, 1f, fadeInDuration));
        
        // 恢复默认文本
        if (buttonText != null)
        {
            buttonText.text = normalText;
            Debug.Log("鼠标离开，文本恢复: " + normalText);
        }
    }

    private IEnumerator AnimateBorder(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;
        float currentProgress = startValue;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentProgress = Mathf.Lerp(startValue, endValue, elapsed / duration);
            buttonMaterial.SetFloat("_Progress", currentProgress);
            yield return null;
        }

        buttonMaterial.SetFloat("_Progress", endValue);
        animationCoroutine = null;
    }

    void OnDestroy()
    {
        // 清理材质实例
        if (buttonMaterial != null)
        {
            DestroyImmediate(buttonMaterial);
        }
    }

    // 公共方法用于动态修改属性
    public void SetBorderColor(Color newColor)
    {
        borderColor = newColor;
        if (buttonMaterial != null)
            buttonMaterial.SetColor("_BorderColor", borderColor);
    }

    public void SetBorderWidth(float newWidth)
    {
        borderWidth = newWidth;
        if (buttonMaterial != null)
            buttonMaterial.SetFloat("_BorderWidth", borderWidth);
    }
}