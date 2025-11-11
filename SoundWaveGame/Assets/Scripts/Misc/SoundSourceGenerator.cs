using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSourceGenerator : MonoBehaviour
{
    public Color soundColor;
    public SoundSource.SoundType soundType;
    public float volume;
    public AudioClip audioClip;

    public void GenerateSound()
    {
        SoundSource.Generate(transform.position, volume, soundColor, soundType);
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }
}
