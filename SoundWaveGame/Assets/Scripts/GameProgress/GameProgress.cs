using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class GameProgress : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onComplete = new();
    public UnityEvent OnComplete => _onComplete;
    public abstract float GetProgressRatio();
    public bool IsComplete => GetProgressRatio() == 1;
    public bool IsCompleted { get; private set; }

    public void Complete()
    {
        if (IsCompleted)
            return;
        IsCompleted = true;
        _onComplete.Invoke();
    }
}
