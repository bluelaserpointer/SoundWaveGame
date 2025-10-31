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


    SoundSource chasingSoundSource;

    int _screamRemainCount;
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
            Scream();
            chasingSoundSource = newSound;
            aiController.NotifyMoveTo(newSound.transform.position);
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
