using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class KnifeThrow : MonoBehaviour
{
    [SerializeField]
    Transform _throwingAnchor;
    [SerializeField]
    Knife knifePrefab;
    [SerializeField]
    float knifeThrowingForce = 100;
    [SerializeField]
    Cooldown knifeThrowCD = new Cooldown(1);
    [SerializeField]
    AudioClip knifeThrowSE;

    [Header("Debug")]
    [SerializeField]
    int _initialCount;

    int knifeCount;
    public int KnifeCount
    {
        get => knifeCount;
        set
        {
            knifeCount = value;
            PlayerStatsUI.Instance.UpdateKnifeCount(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        KnifeCount = _initialCount;
    }

    // Update is called once per frame
    void Update()
    {
        knifeThrowCD.Charge();
        if (Input.GetMouseButton(0))
        {
            if (KnifeCount > 0 && knifeThrowCD.CheckReady())
            {
                --KnifeCount;
                Vector3 throwingPosition = _throwingAnchor.position;
                Vector3 throwingDirection = _throwingAnchor.forward;
                Quaternion throwingRotation = _throwingAnchor.rotation;
                Knife knife = Instantiate(knifePrefab);
                knife.transform.SetPositionAndRotation(throwingPosition + throwingDirection * 0.5f, throwingRotation);
                knife.Rigidbody.AddForce(knifeThrowingForce * throwingDirection);
                AudioSource.PlayClipAtPoint(knifeThrowSE, throwingPosition);
            }
        }
    }
    public void Throw()
    {

    }
}
