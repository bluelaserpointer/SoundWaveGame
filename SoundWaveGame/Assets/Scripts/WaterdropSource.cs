using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterdropSource : MonoBehaviour
{
    [SerializeField]
    Cooldown interval = new Cooldown(1.5F);

    // Update is called once per frame
    void Update()
    {
        if(interval.ChargeAndCheckReady())
        {
            Waterdrop waterdrop = Instantiate(Resources.Load<Waterdrop>("Waterdrop"));
            waterdrop.transform.position = transform.position;
        }
    }
}
