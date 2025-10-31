using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A marker point to enlights nearby objects outline.
/// </summary>
[DisallowMultipleComponent]
public class SoundSource : MonoBehaviour
{
    public enum SoundType { Environment, Enemy }
    public static List<SoundSource> All => SoundSourceTeller.Instance.SoundSources;
    public float volume;
    public Color color;
    public SoundType type;

    public float SpawnTime {  get; private set; }
    public float LifeTime { get; private set; }

    public static float MAX_LIFE_TIME = 4;
    public static readonly float SOUND_VELOCITY = 4;

    private void Start()
    {
        SoundSourceTeller.Instance.AddSoundSouce(this);
        SpawnTime = Time.timeSinceLevelLoad;
    }
    private void Update()
    {
        LifeTime += Time.deltaTime;
        if(LifeTime >= MAX_LIFE_TIME)
        {
            Destory();
        }
    }
    public float WaveDistance()
    {
        return Mathf.Min(volume, SOUND_VELOCITY * LifeTime);
    }
    public bool InsideWaveRadius(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < WaveDistance();
    }
    public void Destory()
    {
        SoundSourceTeller.Instance.RemoveSoundSouce(this);
        Destroy(gameObject);
    }
    /// <summary>
    /// Instantiate a sound source.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SoundSource Generate(Vector3 position, float volume, Color color, SoundType soundType)
    {
        GameObject soundSourceGO = new GameObject("(SoundSouce)");
        soundSourceGO.transform.position = position;
        SoundSource soundSouce = soundSourceGO.AddComponent<SoundSource>();
        soundSouce.volume = volume;
        soundSouce.color = color;
        soundSouce.type = soundType;
        return soundSouce;
    }
}
