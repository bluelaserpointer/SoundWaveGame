using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class Rat : MonoBehaviour
{
    [SerializeField]
    Item _triggerTakenItem;

    [Header("Stop & Go")]
    [SerializeField]
    [Range(0f, 1f)]
    float _stopProbability;
    [SerializeField]
    Vector2 _stopDurationRange;
    [SerializeField]
    Cooldown _navmeshUpdateCd;

    [Header("Random Walk Parameters")]
    [SerializeField] float _wanderSampleRadius;

    [Header("Squeak")]
    [SerializeField]
    Cooldown _squeakMinCd;
    [SerializeField]
    [Range(0f, 1f)]
    float _squakProbabilityDefault;
    [SerializeField]
    [Range(0f, 1f)]
    float _squakProbabilityWhenChasing;

    [Header("Component")]
    [SerializeField]
    SoundSourceGenerator _idleSoundGenerator;
    [SerializeField]
    Animator _animator;

    NavMeshAgent _navMeshAgent;

    public Transform ChaseTarget { get; private set; }
    public bool IsChasing => ChaseTarget != null;
    bool _isStoppedRandomly;
    float _lazyness;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _lazyness = Random.value;

        if (_triggerTakenItem != null)
        {
            _triggerTakenItem.onPickup.AddListener(OnItemTaken);
        }
    }
    private void Start()
    {
        _squeakMinCd.Ratio = Random.value;
    }
    void OnItemTaken()
    {
        _triggerTakenItem.onPickup.RemoveListener(OnItemTaken);
        ChasePlayer();
    }
    // Update is called once per frame
    void Update()
    {
        if (ChaseTarget == null || _isStoppedRandomly)
        {
            _navMeshAgent.isStopped = true;
            _animator.SetBool("isMoving", false);
        }
        else
        {
            if (_navmeshUpdateCd.ChargeAndCheckReady())
            {
                HandleRandomStops();
                Vector3 offsetPos = GetWanderOffset(ChaseTarget.position);
                _navMeshAgent.SetDestination(offsetPos);
                _navMeshAgent.isStopped = false;
            }
            _animator.SetBool("isMoving", _navMeshAgent.velocity.sqrMagnitude > 0.001f);
        }
        HandleSqueak();
    }
    void HandleSqueak()
    {
        if (_squeakMinCd.ChargeAndCheckReady())
        {
            if (Random.value < (IsChasing ? _squakProbabilityWhenChasing : _squakProbabilityDefault))
            {
                _idleSoundGenerator.GenerateSound();
            }
        }
    }
    void HandleRandomStops()
    {
        if (Random.value < Mathf.Lerp(_stopProbability, Mathf.Min(0.75F, _stopProbability * 2), _lazyness))
        {
            StartCoroutine(DoRandomStop());
        }
    }

    IEnumerator DoRandomStop()
    {
        _isStoppedRandomly = true;
        float duration = Random.Range(_stopDurationRange.x, _stopDurationRange.y);
        yield return new WaitForSeconds(duration);
        _isStoppedRandomly = false;
    }
    Vector3 GetWanderOffset(Vector3 targetPosition)
    {
        // 尝试若干次找一个“既有偏移，又在 NavMesh 上”的点
        const int maxTries = 4;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 wanderOffset2d = Random.insideUnitCircle * _wanderSampleRadius;
            Vector3 candidate = targetPosition + new Vector3(wanderOffset2d.x, 0, wanderOffset2d.y);

            NavMeshHit hit;
            // 在 candidate 附近 _wanderSampleRadius 范围内找最近的有效点
            if (NavMesh.SamplePosition(candidate, out hit, _wanderSampleRadius * 2.0F, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // 连续几次都找不到，就别偏移了，直接用目标位置
        return targetPosition;
    }
    public void SetChaseTarget(Transform target)
    {
        ChaseTarget = target;
    }
    public void ChasePlayer()
    {
        ChaseTarget = Player.Instance.transform;
    }
}
