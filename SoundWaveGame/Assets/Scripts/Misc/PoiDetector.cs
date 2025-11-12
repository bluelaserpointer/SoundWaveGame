using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 高亮硬币与撤离点等兴趣点(POI)
/// </summary>
public class PoiDetector : Ability
{
    [SerializeField]
    Cooldown activationCD = new Cooldown(1);
    [SerializeField]
    AudioClip activationSE;
    [SerializeField]
    float _alterDuraiton;
    [SerializeField]
    Gradient _alterTimeGradient;

    [Header("Debug")]
    [SerializeField]
    int _initialCount;

    int _remainUse;
    public int RemainUse
    {
        get => _remainUse;
        set
        {
            _remainUse = value;
            PlayerStatsUI.Instance.UpdateAbilityUsageText();
        }
    }
    public override string UsageText => RemainUse.ToString();
    public override string Name => "Detector";
    public override AbilityType Type => AbilityType.Detector;
    public override bool IsUssable => _remainUse > 0 && activationCD.IsReady;

    public readonly List<SoundVolumeColorModifier> _alteredModifiers = new();

    // Start is called before the first frame update
    void Start()
    {
        RemainUse = _initialCount;
        activationCD.Ratio = 1;
    }

    // Update is called once per frame
    void Update()
    {
        activationCD.Charge();
    }

    public void Activate_Internal()
    {
        activationCD.Reset();
        activationSE.Play2dSound();

        _alteredModifiers.Clear();
        foreach (Item item in Item.StageItems["Coin"])
        {
            if (item.TryGetComponent(out SoundVolumeColorModifier modifier))
            {
                _alteredModifiers.Add(modifier);
            }
        }

        // 停掉上一次还在跑的渐变
        //if (_gradientRoutine != null) StopCoroutine(_gradientRoutine);
        StartCoroutine(AlterColorOverTime());
    }

    private IEnumerator AlterColorOverTime()
    {
        float elapsed = 0f;

        // 先应用起始颜色（t=0）
        Color startColor = _alterTimeGradient.Evaluate(0f);
        for (int i = 0; i < _alteredModifiers.Count; i++)
        {
            var m = _alteredModifiers[i];
            m.forceFrontMost = true;
            m.constantColor = startColor;
            m.Apply();
        }

        // 在时长内每帧按梯度更新
        while (elapsed < _alterDuraiton)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _alterDuraiton);
            Color c = _alterTimeGradient.Evaluate(t);

            for (int i = 0; i < _alteredModifiers.Count; i++)
            {
                var m = _alteredModifiers[i];
                if (m == null)
                {
                    _alteredModifiers.RemoveAt(i);
                    --i;
                    continue;
                }
                m.constantColor = c;
                m.Apply();
            }

            yield return null;
        }

        // 完成后复位
        for (int i = 0; i < _alteredModifiers.Count; i++)
        {
            var m = _alteredModifiers[i];
            if (m == null)
            {
                continue;
            }
            m.forceFrontMost = false;
            m.constantColor = Color.black; // 想恢复原色的话，改成恢复存档的颜色
            m.Apply();
        }
    }

    public override void ActivateAbility()
    {
        --RemainUse;
        Activate_Internal();
    }
}
