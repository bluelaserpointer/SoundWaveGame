using System.Collections.Generic;
using UnityEngine;

public class RenderReplacementShaderToTexture : MonoBehaviour
{
    [Header("ShaderTextures")]
    [SerializeField]
    List<ShaderToTexture> shaderToTextures;

    private Camera subCamera;

    private void Start()
    {
        // Setup a copy of the camera to render the scene using the normals shader.
        Camera thisCamera = GetComponent<Camera>();
        subCamera = new GameObject("CameraForShaderToTextures").AddComponent<Camera>();
        subCamera.enabled = false;
        subCamera.CopyFrom(thisCamera);
        subCamera.transform.SetParent(transform);
        subCamera.depth = thisCamera.depth - 1;

        foreach(var info in shaderToTextures)
            info.Init(subCamera);
    }
    private void Update()
    {
        // 可选：分辨率变化时重建 RT
        if (subCamera.pixelWidth != shaderToTextures[0].Camera.pixelWidth ||
            subCamera.pixelHeight != shaderToTextures[0].Camera.pixelHeight)
        {
            foreach (var info in shaderToTextures)
                info.ResizeIfNeeded(subCamera.pixelWidth, subCamera.pixelHeight);
        }
        foreach (var info in shaderToTextures)
            info.UpdateTexture();
    }
}
[System.Serializable]
class ShaderToTexture
{
    [SerializeField] string textureName;
    [SerializeField] Shader shader;
    [SerializeField] string replacementTag = "RenderType";

    [Header("RT Settings")]
    [SerializeField] RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;
    [SerializeField] FilterMode filterMode = FilterMode.Point;
    [SerializeField] int renderTextureDepth = 0; // 作为颜色RT使用时通常用 0 就行

    [Header("Clear Settings")]
    [SerializeField] bool clearColor = false;
    [SerializeField] Color clearColorValue = new Color(0, 0, 0, 0);

    public RenderTexture RenderTexture { get; private set; }
    public Camera Camera { get; private set; }

    public void Init(Camera camera)
    {
        Camera = camera;
        CreateRT(camera.pixelWidth, camera.pixelHeight);
        Shader.SetGlobalTexture(textureName, RenderTexture);
    }

    public void ResizeIfNeeded(int w, int h)
    {
        if (RenderTexture != null && (RenderTexture.width == w && RenderTexture.height == h))
            return;

        if (RenderTexture != null)
        {
            RenderTexture.Release();
            Object.Destroy(RenderTexture);
        }
        CreateRT(w, h);
        Shader.SetGlobalTexture(textureName, RenderTexture);
    }

    private void CreateRT(int w, int h)
    {
        RenderTexture = new RenderTexture(w, h, renderTextureDepth, renderTextureFormat);
        RenderTexture.filterMode = filterMode;
        RenderTexture.wrapMode = TextureWrapMode.Clamp;
        RenderTexture.useMipMap = false;
        RenderTexture.autoGenerateMips = false;
        RenderTexture.Create();
    }

    public void UpdateTexture()
    {
        if (!clearColor)
        {
            Camera.RenderWithShader(shader, replacementTag);
        }
        else
        {
            var prevTarget = Camera.targetTexture;
            var prevFlags = Camera.clearFlags;
            var prevBG = Camera.backgroundColor;

            Camera.targetTexture = RenderTexture;
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = clearColorValue; // 如果还要清深度，确保 subCamera.depthTextureMode 或 RenderTexture 有深度缓冲
            Camera.RenderWithShader(shader, replacementTag);

            Camera.clearFlags = prevFlags;
            Camera.backgroundColor = prevBG;
            Camera.targetTexture = prevTarget;
        }
    }
}