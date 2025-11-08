using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 至少有一个该游戏物体显示期间，光标会被显示，反则隐藏
/// </summary>
[DisallowMultipleComponent]
public class CursorRequestPanel : MonoBehaviour
{
    private void OnEnable()
    {
        CursorModeService.Add(this);
    }
    private void OnDisable()
    {
        CursorModeService.Remove(this);
    }
}