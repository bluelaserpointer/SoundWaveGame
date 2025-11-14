using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

/// <summary>
/// When receive soundwave invoke an event
/// </summary>
[DisallowMultipleComponent]
public class NoiseReceiveEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onReceive;
    public bool oneTime;

    public UnityEvent OnReceive => _onReceive;
    public bool Invoked { get; private set; }

    private void Update()
    {
        if (oneTime && Invoked)
            return;
        if (FindAnyNoise())
        {
            Invoked = true;
            _onReceive.Invoke();
        }
    }
    SoundSource FindAnyNoise()
    {
        foreach (SoundSource soundSource in SoundSource.All)
        {
            if (!soundSource.InsideWaveRadius(transform.position))
            {
                continue;
            }
            return soundSource;
        }
        return null;
    }
}
