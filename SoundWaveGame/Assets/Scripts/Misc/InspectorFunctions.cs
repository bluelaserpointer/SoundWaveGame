using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InspectorFunctions : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void MoveTo(Transform target)
    {
        transform.SetPositionAndRotation(target.position, target.rotation);
    }
}
