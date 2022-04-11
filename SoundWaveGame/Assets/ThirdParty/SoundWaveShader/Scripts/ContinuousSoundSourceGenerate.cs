using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousSoundSourceGenerate : MonoBehaviour
{
    [SerializeField]
    SoundSource soundSourcePrefab;
    [SerializeField]
    float generationInterval = 1;

    float time;
    // Update is called once per frame
    void Update()
    {
        if ((time += Time.deltaTime) > generationInterval)
        {
            time = 0;
            Instantiate(soundSourcePrefab);
        }
    }
}
