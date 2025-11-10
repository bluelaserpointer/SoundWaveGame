using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// 人物移动时产生SoundWave
/// </summary>
public class FootstepSoundWave : MonoBehaviour
{
    [SerializeField]
    vThirdPersonController _movementController;
    [SerializeField]
    Color _footstepSoundColor;
    [SerializeField]
    SoundSource.SoundType _soundType;
    [SerializeField]
    float _walkingSoundVolume;
    [SerializeField]
    float _sprintingSoundVolume;
    [SerializeField]
    float _walkingSoundInterval;
    [SerializeField]
    float _sprintingSoundInvertal;

    [Header("Sound")]
    [SerializeField]
    List<AudioClip> _footstepSEs;

    bool _lastFrameIsGrounded;
    float _keepMovedWithoutNoiseTime;

    void Awake()
    {
        Init();
    }
    public void Init()
    {
        _lastFrameIsGrounded = true;
        _keepMovedWithoutNoiseTime = 0;
    }

    void Update()
    {
        bool isGrounded = _movementController.isGrounded;
        bool makeFootstep = isGrounded &&
            (_movementController.Input != Vector3.zero
            || !_lastFrameIsGrounded);
        _lastFrameIsGrounded = isGrounded;
        if (!makeFootstep)
        {
            _keepMovedWithoutNoiseTime = 0;
            return;
        }
        _keepMovedWithoutNoiseTime += Time.deltaTime;
        bool isSprinting = _movementController.isSprinting;
        float noiseInterval = isSprinting ? _sprintingSoundInvertal : _walkingSoundInterval;
        if (_keepMovedWithoutNoiseTime < noiseInterval)
            return;
        _keepMovedWithoutNoiseTime = 0;
        MakeStepSound();
    }

    public void MakeStepSound()
    {
        bool isSprinting = _movementController.isSprinting;
        float volume = isSprinting ? _sprintingSoundVolume : _walkingSoundVolume;
        SoundSource.Generate(transform.position, volume, _footstepSoundColor, _soundType);

        if (_footstepSEs.Count > 0)
        {
            AudioSource.PlayClipAtPoint(_footstepSEs.GetRandomElement(), transform.position, 4.0F);
        }
    }
}
