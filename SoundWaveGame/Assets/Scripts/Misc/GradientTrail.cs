using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对移动的物体附加LineRenderer的轨迹特效。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public class GradientTrail : MonoBehaviour
{
    [Header("Core")]
    public LineRenderer lineRenderer;
    [Tooltip("尾巴保留时长(秒)，控制“时间窗口”的长度")]
    public float fadeDuration = 1.5f;

    [Tooltip("采样间隔(秒)")]
    public float sampleInterval = 0.02f;

    [Tooltip("最小采样位移，避免静止时过采样")]
    public float minDistance = 0.01f;

    [Tooltip("最多保留的采样点数")]
    public int maxSamples = 120;

    [Header("Visuals")]
    public Color headColor = Color.white;
    public Color tailColor = Color.black;
    [Tooltip("从头到尾的宽度变化")]
    public AnimationCurve widthOverLength = AnimationCurve.Constant(0, 1, 0.05f);

    struct Sample
    {
        public Vector3 pos;
        public float time;
        public Sample(Vector3 p, float t) { pos = p; time = t; }
    }

    private readonly List<Sample> samples = new List<Sample>();
    private float sampleTimer;
    private Vector3 lastPos;

    void Awake()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthCurve = widthOverLength;

        // 固定渐变
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(headColor, 0f),
                new GradientColorKey(tailColor, 1f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(headColor.a, 0f),
                new GradientAlphaKey(tailColor.a, 1f),
            }
        );
        lineRenderer.colorGradient = g;

        lastPos = transform.position;
    }

    void Update()
    {
        float now = Time.time;
        sampleTimer += Time.deltaTime;

        // 采样：满足时间间隔或位移阈值
        if (samples.Count == 0 ||
            sampleTimer >= sampleInterval ||
            (transform.position - lastPos).sqrMagnitude >= minDistance * minDistance)
        {
            samples.Add(new Sample(transform.position, now));
            if (samples.Count > maxSamples) samples.RemoveAt(0);
            sampleTimer = 0f;
            lastPos = transform.position;
        }

        // 裁剪超时点：只保留“最近 fadeDuration 秒”的轨迹
        int remove = 0;
        for (int i = 0; i < samples.Count; i++)
        {
            if (now - samples[i].time > fadeDuration) remove++;
            else break;
        }
        if (remove > 0) samples.RemoveRange(0, remove);

        if (samples.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // 更新位置
        lineRenderer.positionCount = samples.Count;
        for (int i = 0; i < samples.Count; i++)
            lineRenderer.SetPosition(i, samples[i].pos);
    }
}
