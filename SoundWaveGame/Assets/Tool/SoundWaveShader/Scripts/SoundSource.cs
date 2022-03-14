using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A marker point to enlights nearby objects outline.
/// </summary>
[DisallowMultipleComponent]
public class SoundSource : MonoBehaviour
{
    public float volume;

    public float LifeTime { get; private set; }
    public static float MAX_LIFE_TIME = 2;

    private void Start()
    {
        SoundSourceTeller.Instance.AddSoundSouce(this);
    }
    private void Update()
    {
        LifeTime += Time.deltaTime;
        if(LifeTime >= MAX_LIFE_TIME)
        {
            Destory();
        }

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
    public static void Generate(Vector3 position, float volume)
    {
        GameObject soundSourceGO = new GameObject("(SoundSouce)");
        soundSourceGO.AddComponent<SoundSource>().volume = volume;
        soundSourceGO.transform.position = position;
    }
}
