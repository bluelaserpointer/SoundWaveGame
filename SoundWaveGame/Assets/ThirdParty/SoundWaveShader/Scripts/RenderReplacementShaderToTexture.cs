using UnityEngine;
using System.Collections.Generic;

public class RenderReplacementShaderToTexture : MonoBehaviour
{
    [Header("ShaderTextures")]
    [SerializeField]
    List<ShaderToTexture> shaderToTextures;

    private Camera subCamera;

    private void Start()
    {
        foreach (Transform t in transform)
            DestroyImmediate(t.gameObject);

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
        foreach (var info in shaderToTextures)
            info.UpdateTexture();
    }
}
[System.Serializable]
class ShaderToTexture
{
    [SerializeField]
    string textureName;
    [SerializeField]
    Shader shader;
    [SerializeField]
    string replacementTag = "RenderType";
    [SerializeField]
    RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;
    [SerializeField]
    FilterMode filterMode = FilterMode.Point;
    [SerializeField]
    int renderTextureDepth = 24;
    public RenderTexture RenderTexture { get; private set; }
    public Camera Camera { get; private set; }

    public void Init(Camera camera)
    {
        Camera = camera;
        // Create a render texture matching the main camera's current dimensions.
        RenderTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, renderTextureDepth, renderTextureFormat);
        RenderTexture.filterMode = filterMode;
        // Surface the render texture as a global variable, available to all shaders.
        Shader.SetGlobalTexture(textureName, RenderTexture);
    }
    public void UpdateTexture()
    {
        Camera.targetTexture = RenderTexture;
        Camera.RenderWithShader(shader, replacementTag);
    }
}
