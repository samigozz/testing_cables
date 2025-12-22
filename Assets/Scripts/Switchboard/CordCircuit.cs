using UnityEngine;

public class CordCircuit : MonoBehaviour
{
    [SerializeField] private PlugCord answeringCord, callingCord;
    
    public Call ActiveCall { get; private set; }

    private void OnEnable()
    {
        answeringCord.OnPlugged += CordPlugged;
        answeringCord.OnUnplugged += CordUnplugged;
        callingCord.OnPlugged += CordPlugged;
        callingCord.OnUnplugged += CordUnplugged;
    }

    private void OnDisable()
    {
        answeringCord.OnPlugged -= CordPlugged;
        answeringCord.OnUnplugged -= CordUnplugged;
        callingCord.OnPlugged -= CordPlugged;
        callingCord.OnUnplugged -= CordUnplugged;
    }

    private void CordPlugged(PlugCord plug)
    {
        if (!plug)
            return;
        
        var jack = plug.GetCurrentJack;
        
        if (!jack)
            return;

        if (jack is LineJack lineJack && plug == answeringCord)
        {
            ActiveCall = lineJack.activeCall;

            if (ActiveCall == null)
                return;
            
            if (ActiveCall.callStatus != CallStatus.Ringing) 
                return;
            
            ActiveCall.callStatus = CallStatus.Answered;
            jack.OnAnswerCall();
        }
        else if (plug == callingCord && ActiveCall != null && ActiveCall.callStatus == CallStatus.Answered)
        {
            jack.OnAnswerCall();
        }
    }
    private void CordUnplugged(PlugCord plug)
    {
        if (!plug)
            return;
        
        var jack = plug.GetCurrentJack;

        if (!jack)
            return;

        if (plug == answeringCord && ActiveCall != null)
        {
            ActiveCall.callStatus = CallStatus.Disconnected;
        }
        
        jack.OnDisconnectCall();
    }
}
