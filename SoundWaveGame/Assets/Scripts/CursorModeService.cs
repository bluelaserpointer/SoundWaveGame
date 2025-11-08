using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 没有任何<see cref="CursorRequestPanel"/>显示时，隐藏光标，反则显示
/// </summary>
[DisallowMultipleComponent]
public sealed class CursorModeService : MonoBehaviour
{

    [SerializeField] private CursorLockMode gameplayLockWhenNoUI = CursorLockMode.Locked; // 视角模式常用 Locked

    public readonly UnityEvent<bool> onCursorShown = new UnityEvent<bool>();     // true=显示UI光标，false=回到视角

    public static CursorModeService Instance
    {
        set => _instance = value;
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CursorModeService>();
                if (_instance == null)
                {
                    Debug.LogWarning($"No {nameof(CursorModeService)} found. Please add this object initially.");
                }
            }
            return _instance;
        }
    }
    static CursorModeService _instance;

    public int CursorRequesterCount => _requesters.Count;
    public static bool RequestCursor => Instance.CursorRequesterCount > 0;
    // 用 HashSet 防重复；同时清理失效引用
    private readonly HashSet<CursorRequestPanel> _requesters = new();
    static bool isShuttingDown;

    private void Awake()
    {
        Instance = this;
        Recompute();
    }
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) Recompute(); // 避免 Alt+Tab 导致状态漂移
    }

    public static void Add(CursorRequestPanel r)
    {
        if (Instance == null) return;
        Instance.add_internal(r);
    }
    private void add_internal(CursorRequestPanel r)
    {
        if (r == null) return;
        bool wasEmpty = CursorRequesterCount == 0;
        _requesters.Add(r);
        if (wasEmpty && CursorRequesterCount > 0) Apply(show: true);
    }

    public static void Remove(CursorRequestPanel r)
    {
        if (isShuttingDown ||Instance == null || !Instance.gameObject.scene.isLoaded) return;
        Instance.remove_internal(r);
    }
    private void remove_internal(CursorRequestPanel r)
    {
        if (r == null) return;
        _requesters.Remove(r);
        if (CursorRequesterCount == 0) Apply(show: false);
    }

    private void Apply(bool show)
    {
        if (show)
        {
            // 默认 UI 场景：显示光标 + 不锁定（需要可点击）
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            onCursorShown?.Invoke(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = gameplayLockWhenNoUI; // 视角模式
            onCursorShown?.Invoke(false);
        }
    }

    private void Recompute()
    {
        Apply(CursorRequesterCount > 0);
    }
}
