using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class KnifeThrow : Ability
{
    public Vector3 targetPosition;
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

    int _knifeCount;
    public int KnifeCount
    {
        get => _knifeCount;
        set
        {
            _knifeCount = value;
            PlayerStatsUI.Instance.UpdateAbilityUsageText();
        }
    }
    public override string UsageText => KnifeCount.ToString();

    public override string Name => "P-Knife";
    public override AbilityType Type => AbilityType.Knife;

    public override bool IsUssable => _knifeCount > 0 && knifeThrowCD.IsReady;

    // Start is called before the first frame update
    void Start()
    {
        KnifeCount = _initialCount;
    }

    // Update is called once per frame
    void Update()
    {
        knifeThrowCD.Charge();
    }
    public void Throw()
    {
        knifeThrowCD.Reset();
        Vector3 throwingPosition = _throwingAnchor.position;
        Vector3 forceDirection = (targetPosition - throwingPosition).normalized;
        Quaternion throwingRotation = Quaternion.LookRotation(forceDirection);
        Knife knife = Instantiate(knifePrefab);
        knife.transform.SetPositionAndRotation(throwingPosition + forceDirection * 0.5f, throwingRotation);
        knife.Rigidbody.AddForce(knifeThrowingForce * forceDirection);
        AudioSource.PlayClipAtPoint(knifeThrowSE, throwingPosition);
    }

    public override void ActivateAbility()
    {
        --KnifeCount;
        Throw();
    }
}
