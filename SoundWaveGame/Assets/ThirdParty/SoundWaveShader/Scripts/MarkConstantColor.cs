using UnityEngine;

/// <summary>
/// Make this children visible in a color even no sound wave are lighting it 
/// </summary>
[DisallowMultipleComponent]
public class MarkConstantColor : MonoBehaviour
{
    [SerializeField]
    Color _constantColor = Color.white;

    static readonly int ID_ConstantColor = Shader.PropertyToID("_ConstantColor");

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
                r.SetPropertyBlock(_mpb, i);
            }
        }
    }
}
