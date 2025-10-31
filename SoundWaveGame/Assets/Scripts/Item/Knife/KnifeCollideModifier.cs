using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 更改刀插入时的举动
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class KnifeCollideModifier : MonoBehaviour
{
    public UnityEvent onKnifeCollision;
}
