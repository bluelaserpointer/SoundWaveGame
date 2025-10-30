using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    SoundSource chasingSouncSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //chase louder noises
        float minSpawnTime = -1;
        if (chasingSouncSource != null)
            minSpawnTime = chasingSouncSource.volume;
        SoundSource newSound = FindLatestNoise(minSpawnTime);
        if (newSound != null && newSound != chasingSouncSource)
        {
            chasingSouncSource = newSound;
            aiController.NotifyMoveTo(newSound.transform.position);
        }
    }
    SoundSource FindLatestNoise(float minSpawnTime)
    {
        SoundSource candidate = null;
        foreach (SoundSource soundSource in SoundSource.All)
        {
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
