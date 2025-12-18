using System;
using UnityEngine;

public enum CallStatus
{
    Disconnected,
    Ringing,
    Answererd,
    Connected,
    Timeout
}

[Serializable]
public class Call
{
    public CallStatus callStatus;
    public string ID { get; private set; }
    public string CallerNumber { get; private set; }
    public string TargetNumber { get; private set; }
    
    public float RingDuration { get; private set; }

    private float _ringTime;
    
    public Call(string callerNumber, string targetNumber = "" , 
        float minRingTime = 20f, float maxRingTime = 45f)
    {
        ID = Guid.NewGuid().ToString();
        CallerNumber = callerNumber;
        TargetNumber = targetNumber;
        callStatus = CallStatus.Ringing;
        
        RingDuration = UnityEngine.Random.Range(minRingTime, maxRingTime);
        _ringTime = RingDuration;
    }
    
    private void TimeoutCall()
    {
        if (callStatus != CallStatus.Ringing) 
            return;
        
        callStatus = CallStatus.Timeout;
        
        Debug.Log($"Call timed out: {CallerNumber} (Rang for {RingDuration}s)");
    }
}
