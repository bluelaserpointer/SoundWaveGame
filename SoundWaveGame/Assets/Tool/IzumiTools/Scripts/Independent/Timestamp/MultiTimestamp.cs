using System.Collections.Generic;

public class MultiTimestamp : Timestamp
{
    public readonly List<float> timestamps = new List<float>();

    public override void Stamp()
    {
        base.Stamp();
        timestamps.Add(lastStampTime);
    }
}
