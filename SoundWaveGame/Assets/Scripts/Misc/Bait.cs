using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Bait : MonoBehaviour
{
    public Cooldown pulseCd;
    public int maxPulseCount;
    [SerializeField]
    SoundSourceGenerator _soundSourceGenerator;

    public int PluseCount { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (pulseCd.ChargeAndCheckReady())
        {
            if (PluseCount >= maxPulseCount)
            {
                Destroy(gameObject);
                return;
            }
            ++PluseCount;
            _soundSourceGenerator.GenerateSound();
        }
    }
}
