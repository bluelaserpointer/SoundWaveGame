using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IzumiTools/BGMRandomChooser")]
public class BGMRandomChooser : ScriptableObject
{
    [SerializeField]
    List<AudioClip> bgms;
    
    public AudioClip PickRandom()
    {
        return bgms.GetRandomElement();
    }
}
