using UnityEngine;
using UnityEditor;

public static class FixNegativeScaleMirrors
{
    [MenuItem("Tools/Fix Negative Scale Mirrors (Selection)")]
    private static void FixSelection()
    {
        var roots = Selection.gameObjects;
        if (roots == null || roots.Length == 0)
        {
            Debug.LogWarning("请先在层级视图里选中至少一个根物体，再执行 Tools/Fix Negative Scale Mirrors (Selection)。");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        foreach (var go in roots)
        {
            FixHierarchy(go.transform);
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log("完成：已处理所选物体层级内的负 scale。");
    }

    /// <summary>
    /// 递归处理层级内所有带负数 scale 的 Transform
    /// </summary>
    private static void FixHierarchy(Transform root)
    {
        // 先处理自己
        FixOneTransform(root);

        // 再递归子节点（注意：FixOneTransform 不会修改 root 本身的父亲，只会暂时拆掉它的子节点）
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            FixHierarchy(child);
        }
    }

    /// <summary>
    /// 如果这个 Transform 有负的 localScale，就把它改成正数，
    /// 并用临时脱离 / 重新设父的方式保持子树的世界变换不变。
    /// </summary>
    private static void FixOneTransform(Transform t)
    {
        Vector3 s = t.localScale;
        if (s.x >= 0f && s.y >= 0f && s.z >= 0f)
            return; // 没有负 scale，跳过

        // 这里你可以根据需要限制只处理 -1/1 的情况：
        // if (!Mathf.Approximately(Mathf.Abs(s.x), 1f) ||
        //     !Mathf.Approximately(Mathf.Abs(s.y), 1f) ||
        //     !Mathf.Approximately(Mathf.Abs(s.z), 1f))
        //     return;

        // 先把所有直接子物体记下来
        int childCount = t.childCount;
        var children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = t.GetChild(i);
        }

        // 把每个子物体临时脱离父子关系（保持世界坐标不变）
        foreach (var c in children)
        {
            Undo.SetTransformParent(c, null, "Unparent for negative scale fix");
        }

        // 修改自己的 scale 为绝对值（全部变正数）
        Undo.RecordObject(t, "Fix negative scale");
        t.localScale = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));

        // 把子物体再挂回自己名下，同样保持世界坐标不变
        foreach (var c in children)
        {
            Undo.SetTransformParent(c, t, "Reparent after negative scale fix");
        }
    }
}
