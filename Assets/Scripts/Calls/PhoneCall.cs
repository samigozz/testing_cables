using System;

public enum CallStatus
{
    Ringing,
    Answererd,
    Connected,
    Disconnected,
    Failed
}

[Serializable]
public class PhoneCall
{
    public CallStatus callStatus;
    public string CallID { get; private set; }
    public string CallerNumber { get; private set; }
    public string TargetNumber { get; private set; }

    public PhoneCall(string callerNumber, string targetNumber = "")
    {
        CallID = Guid.NewGuid().ToString();
        CallerNumber = callerNumber;
        TargetNumber = targetNumber;
        callStatus = CallStatus.Ringing;
    }
}
