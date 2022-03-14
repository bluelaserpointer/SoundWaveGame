using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tell shaders sound source info 
/// </summary>
[DisallowMultipleComponent]
public class SoundSourceTeller : MonoBehaviour
{
    public static SoundSourceTeller Instance { get; private set; }
    public static int MAX_SOUND_SOURCE_COUNT = 64;
    private void Awake()
    {
        Instance = this;
    }
    List<SoundSource> soundSources = new List<SoundSource>();

    public void AddSoundSouce(SoundSource soundSource)
    {
        soundSources.Add(soundSource);
    }
    public void RemoveSoundSouce(SoundSource soundSource)
    {
        soundSources.Remove(soundSource);
    }
    struct SoundSourceData
    {
        public float volume;
        public float lifeTime;
        public SoundSourceData(SoundSource soundSource)
        {
            //position = soundSource.transform.position;
            volume = 1;
            lifeTime = soundSource.LifeTime;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(soundSources.Count > 0)
        {
            Vector4[] soundSourcePositions = new Vector4[MAX_SOUND_SOURCE_COUNT];
            float[] soundSourceVolumes = new float[MAX_SOUND_SOURCE_COUNT];
            float[] soundSourceLifeTimes = new float[MAX_SOUND_SOURCE_COUNT];

            int loops = 0;
            foreach (SoundSource soundSource in soundSources)
            {
                if(soundSource != null) //(maybe redundant)
                {
                    soundSourcePositions[loops] = soundSource.transform.position;
                    soundSourceVolumes[loops] = soundSource.volume;
                    soundSourceLifeTimes[loops] = soundSource.LifeTime;
                    if (++loops >= MAX_SOUND_SOURCE_COUNT)
                    {
                        Debug.LogWarning("Too many sound sources! (>" + MAX_SOUND_SOURCE_COUNT + ")");
                        break;
                    }
                }
            }
            Shader.SetGlobalInt("_SoundSourceCount", loops);
            Shader.SetGlobalVectorArray("_SoundSourcePositions", soundSourcePositions);
            Shader.SetGlobalFloatArray("_SoundSourceVolumes", soundSourceVolumes);
            Shader.SetGlobalFloatArray("_SoundSourceLifeTimes", soundSourceLifeTimes);
        }
        else
        {
            Shader.SetGlobalInt("_SoundSourceCount", 0);
        }
    }
}
