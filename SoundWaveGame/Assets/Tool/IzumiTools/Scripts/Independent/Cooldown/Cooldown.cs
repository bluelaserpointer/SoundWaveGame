using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    //inspector
    [Min(0)]
    public float requiredTime;

    //init
    public Cooldown(float requiredTime)
    {
        this.requiredTime = requiredTime;
    }
    public Cooldown()
    {
    }

    //data
    public float RequiredTime => requiredTime;
    protected float storedTime;
    public float StoredTime {
        get => storedTime;
        set => storedTime = Mathf.Clamp(value, 0, requiredTime);
    }
    public float Ratio
    {
        get => storedTime / requiredTime;
        set => storedTime = requiredTime * value;
    }
    public readonly Timestamp lastResetTime = new Timestamp();
    public bool IsReady => storedTime >= requiredTime;
    public bool ChargeAndCheckReady(bool resetIfReady = true)
    {
        Charge();
        return CheckReady(resetIfReady);
    }
    public bool ChargeAndCheckReady(float time, bool resetIfReady = true)
    {
        Charge(time);
        return CheckReady(resetIfReady);
    }
    public bool CheckReady(bool resetIfReady = true)
    {
        if (IsReady)
        {
            Reset();
            return true;
        }
        return false;
    }
    public void Reset()
    {
        StoredTime = 0;
        lastResetTime.Stamp();
    }
    public void Charge()
    {
        Charge(Time.deltaTime);
    }
    public void Charge(float time)
    {
        if (!IsReady)
            StoredTime += time;
    }
}
