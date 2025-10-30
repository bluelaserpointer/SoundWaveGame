using UnityEngine;

[DisallowMultipleComponent]
public class MarkAsPlayerForSoundVolume : MonoBehaviour
{
    [Range(0f, 1f)]
    public float playerBaseRipple = 0.55f;

    static readonly int ID_IsPlayer = Shader.PropertyToID("_IsPlayer");
    static readonly int ID_PlayerBaseRipple = Shader.PropertyToID("_PlayerBaseRipple");

    MaterialPropertyBlock _mpb;

    void OnEnable() { Apply(true); }
    void OnDisable() { Apply(false); }
#if UNITY_EDITOR
    void OnValidate() { if (enabled) Apply(true); }
#endif

    void Apply(bool isPlayer)
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
                _mpb.SetFloat(ID_IsPlayer, isPlayer ? 1f : 0f);
                _mpb.SetFloat(ID_PlayerBaseRipple, isPlayer ? playerBaseRipple : 0f);
                r.SetPropertyBlock(_mpb, i);
            }
        }
    }
}
