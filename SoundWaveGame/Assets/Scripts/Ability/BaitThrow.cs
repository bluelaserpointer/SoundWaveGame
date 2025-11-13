using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitThrow : Ability
{
    public Vector3 targetPosition;
    [SerializeField]
    Transform _throwAnchor;
    [SerializeField]
    Bait baitPrefab;
    [SerializeField]
    float throwForce = 100;
    [SerializeField]
    Cooldown throwCD = new Cooldown(1);
    [SerializeField]
    AudioClip throwSE;

    [Header("Debug")]
    [SerializeField]
    int _initialCount;

    int _remainCount;
    public int RemainCount
    {
        get => _remainCount;
        set
        {
            _remainCount = value;
            PlayerStatsUI.Instance.UpdateAbilityUsageText();
        }
    }
    public override string UsageText => RemainCount.ToString();

    public override string Name => "Bait";
    public override AbilityType Type => AbilityType.Bait;

    public override bool IsUssable => _remainCount > 0 && throwCD.IsReady;

    // Start is called before the first frame update
    void Start()
    {
        RemainCount = _initialCount;
    }

    // Update is called once per frame
    void Update()
    {
        throwCD.Charge();
    }
    public void Throw()
    {
        throwCD.Reset();
        Vector3 throwingPosition = _throwAnchor.position;
        Vector3 forceDirection = (targetPosition - throwingPosition).normalized;
        Quaternion throwingRotation = Quaternion.LookRotation(forceDirection);
        Bait bait = Instantiate(baitPrefab);
        bait.transform.SetPositionAndRotation(throwingPosition + forceDirection * 0.5f, throwingRotation);
        bait.Rigidbody.AddForce(throwForce * forceDirection);
        AudioSource.PlayClipAtPoint(throwSE, throwingPosition);
    }

    public override void ActivateAbility()
    {
        --RemainCount;
        Throw();
    }
}
