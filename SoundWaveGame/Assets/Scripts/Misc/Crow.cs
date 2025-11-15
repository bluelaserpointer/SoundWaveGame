using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Crow : MonoBehaviour
{
    [SerializeField]
    Animator _animator;
    [SerializeField]
    NoiseReceiveEvent _noiseReceiver;
    [SerializeField]
    SoundSourceGenerator _flySoundGenerator;
    [SerializeField]
    Cooldown _flySoundCd;
    [SerializeField]
    int _flySoundMaxCount;
    [SerializeField]
    List<AudioClip> _flySEs;

    int _flySoundCount;
    bool _isFlying;

    void Start()
    {
        _noiseReceiver.OnReceive.AddListener(Fly);
    }
    private void Update()
    {
        if (_isFlying)
        {
            if (_flySoundCount < _flySoundMaxCount)
            {
                if (_flySoundCd.ChargeAndCheckReady())
                {
                    ++_flySoundCount;
                    _flySoundGenerator.GenerateSound();
                }
            }
        }
    }

    public void Fly()
    {
        AudioSource.PlayClipAtPoint(_flySEs.GetRandomElement(), transform.position);
        _isFlying = true;
        _flySoundCd.Ratio = 1;
        _animator.SetBool("fly", true);
        Destroy(gameObject, 5);
    }
}
