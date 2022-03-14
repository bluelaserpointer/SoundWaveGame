public class Timestamp
{
    protected float lastStampTime;
    public float LastStampTime => lastStampTime;
    public float PassedTime => UnityEngine.Time.timeSinceLevelLoad - LastStampTime;
    public virtual void Stamp()
    {
        lastStampTime = UnityEngine.Time.timeSinceLevelLoad;
    }
}
