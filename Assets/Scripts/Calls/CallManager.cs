using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CallManager : MonoBehaviour
{
    public static CallManager Instance { get; private set; }
    
    [Header("Call's Settings")]
    [SerializeField] private Vector2 timeoutCallRange;
    
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI currentCallText;

    [HideInInspector] public List<LineJack> lineJacks;
    [HideInInspector] public List<Jack> extensionJacks;
    
    public Dictionary<string, Call> ActiveCalls = new Dictionary<string, Call>();
    public Call SelectedCall { get; private set; }

    [SerializeField] private InputAction callKey;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        callKey.Enable();
        callKey.performed += CallTesting;
    }

    private void OnDisable()
    {
        callKey.Disable();
        callKey.performed -= CallTesting;
    }
    
    private void CallTesting(InputAction.CallbackContext context)
    {
        if(lineJacks != null && lineJacks.Count > 0 && GetOpenLineJacks())
            StartCoroutine(AddNewCall());
    }

    private bool GetOpenLineJacks()
    {
        var openJacks = lineJacks.Where(jack => !jack.IsOccupied).ToList();
        return openJacks.Count > 0;
    }

    private IEnumerator AddNewCall()
    {
        var freeLineJacks = lineJacks.Where(jack => !jack.IsOccupied).ToList();
        
        var randLine = freeLineJacks[Random.Range(0, freeLineJacks.Count)];

        var newCall = new Call("");
        ActiveCalls.Add(newCall.ID, newCall);
        randLine.InitCall(newCall);
        
        yield return new WaitForSeconds(Random.Range(timeoutCallRange.x, timeoutCallRange.y));

        if (newCall.callStatus == CallStatus.Ringing)
        {
            randLine.TimeOutCall();
            ActiveCalls.Remove(newCall.ID);
        }
    }

    public void ConnectToTarget(string inputNumber)
    {
        
    }
}
