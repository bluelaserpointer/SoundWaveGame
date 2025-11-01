using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField]
    Transform _screamAnchor;
    [SerializeField]
    Cooldown _screamInterval;
    [SerializeField]
    Color _screamSoundColor;
    [SerializeField]
    float _screamVolume;
    [SerializeField]
    float _singleSoundMaxChaseTime;


    SoundSource chasingSoundSource;

    int _screamRemainCount;
    float _singleSoundChasedTime;
    public bool IsChasing { get; private set; }
    // Update is called once per frame
    void Update()
    {
        _screamInterval.Charge();
        //chase louder noises
        float minSpawnTime = -1;
        if (chasingSoundSource != null)
            minSpawnTime = chasingSoundSource.volume;
        SoundSource newSound = FindLatestNoise(minSpawnTime);
        if (newSound != null && newSound != chasingSoundSource)
        {
            IsChasing = true;
            _singleSoundChasedTime = 0;
            Scream();
            chasingSoundSource = newSound;
            aiController.NotifyMoveTo(newSound.transform.position);
        }
        else
        {
            if (chasingSoundSource != null)
            {
                if (IsChasing)
                {
                    _singleSoundChasedTime += Time.deltaTime;
                    if (_singleSoundChasedTime > _singleSoundMaxChaseTime)
                    {
                        IsChasing = false;
                        aiController.NotifyMoveTo(transform.position);
                    }
                }
            }
        }
        
    }
    public void Scream()
    {
        if (_screamInterval.CheckReady())
            SoundSource.Generate(_screamAnchor.position, _screamVolume, _screamSoundColor, SoundSource.SoundType.Enemy);
    }
    SoundSource FindLatestNoise(float minSpawnTime)
    {
        SoundSource candidate = null;
        foreach (SoundSource soundSource in SoundSource.All)
        {
            if (soundSource.type == SoundSource.SoundType.Enemy)
            {
                continue;
            }
            if (soundSource.SpawnTime < minSpawnTime)
            {
                continue;
            }
            if (!soundSource.InsideWaveRadius(transform.position))
            {
                continue;
            }
            candidate = soundSource;
            minSpawnTime = candidate.SpawnTime;
        }
        return candidate;
    }
}
