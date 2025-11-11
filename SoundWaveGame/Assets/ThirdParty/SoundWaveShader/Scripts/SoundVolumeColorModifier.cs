using UnityEngine;

/// <summary>
/// Modify child renderers appearance in soundwave shaders.
/// </summary>
[DisallowMultipleComponent]
public class SoundVolumeColorModifier : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Black has no effect.")]
    Color _constantColor = Color.black;
    [SerializeField]
    [Tooltip("Replace constantColor.")]
    bool _sampleTextureColorAsConstantColor;
    [SerializeField]
    bool _ignoreOutlineClip;

    [Header("Particle")]
    [SerializeField]
    bool _useParticleAlphaClip;
    [SerializeField]
    bool _useAdditiveBlackKey;


    static readonly int ID_ConstantColor = Shader.PropertyToID("_ConstantColor");
    static readonly int ID_SampleTextureColorAsConstantColor = Shader.PropertyToID("_SampleTextureColorAsConstantColor");
    static readonly int ID_IgnoreOutlineClip = Shader.PropertyToID("_IgnoreOutlineClip");
    static readonly int ID_UseParticleAlphaClip = Shader.PropertyToID("_UseParticleAlphaClip");
    static readonly int ID_UseAdditiveBlackKey = Shader.PropertyToID("_UseAdditiveBlackKey");

    MaterialPropertyBlock _mpb;

    void OnEnable() { Apply(enabled); }
    void OnDisable() { Apply(false); }
#if UNITY_EDITOR
    void OnValidate() { Apply(enabled); }
#endif

    void Apply(bool visible)
    {
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (!r) continue;

            // 逐 submesh 写入（确保所有材质槽都拿到 MPB）
            int submeshCount = Mathf.Max(1, r.sharedMaterials?.Length ?? 1);
            for (int i = 0; i < submeshCount; i++)
            {
                r.GetPropertyBlock(_mpb, i);
                _mpb.SetColor(ID_ConstantColor, visible ? _constantColor : Color.black);
                _mpb.SetInt(ID_SampleTextureColorAsConstantColor, _sampleTextureColorAsConstantColor ? 1 : 0);
                _mpb.SetInt(ID_IgnoreOutlineClip, _ignoreOutlineClip ? 1 : 0);
                _mpb.SetInt(ID_UseParticleAlphaClip, _useParticleAlphaClip ? 1 : 0);
                _mpb.SetInt(ID_UseAdditiveBlackKey, _useAdditiveBlackKey ? 1 : 0);
                r.SetPropertyBlock(_mpb, i);
            }
        }
    }
}
