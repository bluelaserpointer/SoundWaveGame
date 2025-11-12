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

    [Header("Image Alpha Settings")]
    [Range(0f, 1f)]
    public float normalAlpha = 0.4f; // 正常状态透明度 40%
    [Range(0f, 1f)]
    public float hoverAlpha = 1f;    // 悬停状态透明度 100%
    public float alphaChangeDuration = 0.2f; // 透明度变化持续时间

    private Material buttonMaterial;
    private Coroutine animationCoroutine;
    private Coroutine alphaAnimationCoroutine; // 透明度动画协程
    private bool isMouseOver = false;

    [Header("Ring Effect Settings")]
    public Color ringColor = Color.white; // 白色不透明
    public float ringWidth = 0.02f;
    public float ringSpeed = 1.0f; // 调整这个值可以控制圆环扩散速度

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
        else
        {
            // 设置初始透明度
            SetImageAlpha(normalAlpha);
        }

        // 创建材质实例
        buttonMaterial = new Material(Shader.Find("UIButtonBorder"));
        buttonImage.material = buttonMaterial;
        
        // 设置边框初始属性
        buttonMaterial.SetColor("_BorderColor", Color.clear);
        buttonMaterial.SetFloat("_BorderWidth", borderWidth);
        buttonMaterial.SetFloat("_WaveSpeed", waveSpeed);
        buttonMaterial.SetFloat("_Progress", 0f);

        // 设置圆环属性
        buttonMaterial.SetColor("_RingColor", ringColor);
        buttonMaterial.SetFloat("_RingWidth", ringWidth);
        buttonMaterial.SetFloat("_RingSpeed", ringSpeed);
    }
    
    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;

        // 停止之前的动画
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        if (alphaAnimationCoroutine != null)
            StopCoroutine(alphaAnimationCoroutine);

        // 开始边框动画
        buttonMaterial.SetColor("_BorderColor", borderColor);
        animationCoroutine = StartCoroutine(AnimateBorder(1f, 0f, fadeOutDuration));
        
        // 开始透明度动画（从当前透明度到悬停透明度）
        alphaAnimationCoroutine = StartCoroutine(AnimateAlpha(hoverAlpha, alphaChangeDuration));

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

        // 停止之前的动画
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        if (alphaAnimationCoroutine != null)
            StopCoroutine(alphaAnimationCoroutine);

        // 开始边框动画
        animationCoroutine = StartCoroutine(AnimateBorder(0f, 1f, fadeInDuration));

        // 开始透明度动画（从当前透明度到正常透明度）
        alphaAnimationCoroutine = StartCoroutine(AnimateAlpha(normalAlpha, alphaChangeDuration));
        
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

    // 新增：透明度动画协程
    private IEnumerator AnimateAlpha(float targetAlpha, float duration)
    {
        if (buttonImage == null) yield break;

        float elapsed = 0f;
        Color startColor = buttonImage.color;
        float startAlpha = startColor.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            SetImageAlpha(currentAlpha);
            yield return null;
        }

        SetImageAlpha(targetAlpha);
        alphaAnimationCoroutine = null;
    }

    // 新增：设置图片透明度的方法
    private void SetImageAlpha(float alpha)
    {
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
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
    
    // 新增：公共方法用于动态修改透明度设置
    public void SetNormalAlpha(float alpha)
    {
        normalAlpha = Mathf.Clamp01(alpha);
        if (!isMouseOver)
        {
            SetImageAlpha(normalAlpha);
        }
    }

    public void SetHoverAlpha(float alpha)
    {
        hoverAlpha = Mathf.Clamp01(alpha);
        if (isMouseOver)
        {
            SetImageAlpha(hoverAlpha);
        }
    }
}