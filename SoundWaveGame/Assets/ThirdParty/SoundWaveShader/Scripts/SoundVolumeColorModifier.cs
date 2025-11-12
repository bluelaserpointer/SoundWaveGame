#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Modify child renderers appearance in soundwave shaders.
/// </summary>
[DisallowMultipleComponent]
public class SoundVolumeColorModifier : MonoBehaviour
{
    [Tooltip("Black has no effect.")]
    public Color constantColor = Color.black;
    [SerializeField]
    [Tooltip("Replace constantColor.")]
    bool _sampleTextureColorAsConstantColor;
    [SerializeField]
    bool _ignoreOutlineClip;
    public bool forceFrontMost;

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

    static readonly string KEYWORD_FRONT_MOST = "FRONT_MOST";

    MaterialPropertyBlock _mpb;

    void OnEnable() { Apply(); }

    public void Apply()
    {
#if UNITY_EDITOR
        // 如果当前对象是 Prefab Asset（不是场景实例），跳过 Apply
        if (!Application.isPlaying || PrefabUtility.IsPartOfPrefabAsset(this))
            return;
#endif
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        var renderers = GetComponentsInChildren<Renderer>(true);
        int overlayQueueOffset = 10;
        foreach (var r in renderers)
        {
            if (!r) continue;

            // 逐 submesh 写入（确保所有材质槽都拿到 MPB）
            int submeshCount = Mathf.Max(1, r.sharedMaterials?.Length ?? 1);
            for (int i = 0; i < submeshCount; i++)
            {
                r.GetPropertyBlock(_mpb, i);
                _mpb.SetColor(ID_ConstantColor, constantColor);
                _mpb.SetInt(ID_SampleTextureColorAsConstantColor, _sampleTextureColorAsConstantColor ? 1 : 0);
                _mpb.SetInt(ID_IgnoreOutlineClip, _ignoreOutlineClip ? 1 : 0);
                _mpb.SetInt(ID_UseParticleAlphaClip, _useParticleAlphaClip ? 1 : 0);
                _mpb.SetInt(ID_UseAdditiveBlackKey, _useAdditiveBlackKey ? 1 : 0);
                r.SetPropertyBlock(_mpb, i);
            }
            // —— 材质实例：调整 Render Queue（MPB改不了队列） —— //
            // 注意：materials 会实例化，避免污染原材质

            var mats = r.materials;
            if (mats != null)
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null) continue;

                    if (forceFrontMost)
                    {
                        // Overlay = 4000
                        m.renderQueue = 4000 + overlayQueueOffset;
                        m.EnableKeyword(KEYWORD_FRONT_MOST);
                    }
                    else
                    {
                        // -1 恢复为 shader 默认
                        m.renderQueue = -1;
                        m.DisableKeyword(KEYWORD_FRONT_MOST);
                    }
                }
            }
        }
    }
}
