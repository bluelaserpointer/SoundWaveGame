using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioClipExtension
{
    public static void Play2dSound(this AudioClip clip)
    {
        if (clip == null)
            return;
        var a = new GameObject($"2D Oneshot Audio: {clip.name}").AddComponent<AudioSource>();
        a.clip = clip;
        a.spatialBlend = 0f; // 0 = 2D, 1 = 3D
        a.Play();
        Object.Destroy(a.gameObject, clip.length);
    }
}
