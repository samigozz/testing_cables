using System;
using UnityEngine;

public class CordCircuit : MonoBehaviour
{
    [SerializeField] private PlugCord answeringCord, callingCord;
    
    public PhoneCall ActiveCall { get; private set; }

    private void OnEnable()
    {
        answeringCord.OnPlugConnected += CheckConnection;
        callingCord.OnPlugConnected += CheckConnection;
    }

    private void OnDisable()
    {
        answeringCord.OnPlugConnected -= CheckConnection;
        callingCord.OnPlugConnected -= CheckConnection;
    }

    private void CheckConnection()
    {
        if (!answeringCord || !callingCord)
            return;
        
        var lineJack = answeringCord.GetCurrentJack;
        var extensionJack = callingCord.GetCurrentJack;
        
    }

    private void AnswerCall()
    {
        
    }
}
