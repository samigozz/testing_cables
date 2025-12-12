using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class CallManager : MonoBehaviour
{
    public static CallManager Instance { get; private set; }
    
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI currentCallText;

    [HideInInspector] public List<LineJack> lineJacks;
    [HideInInspector] public List<Jack> extensionJacks;
    
    public PhoneCall CurrentCall { get; private set; }

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

    public void CreateMockCall()
    {
        if(lineJacks != null && lineJacks.Count > 0 
           && extensionJacks != null && extensionJacks.Count > 0)
            StartCoroutine(MockCallGenerator());
    }

    private IEnumerator MockCallGenerator()
    {
        var freeLineJacks = new List<LineJack>();
        
        foreach (var jack in lineJacks)
        {
            if(jack.IsOccupied)
                continue;
            freeLineJacks.Add(jack);
        }
        
        var randLine = freeLineJacks[Random.Range(0, freeLineJacks.Count)];

        randLine.Call();
        
        yield return new WaitForSeconds(Random.Range(15, 25));
    }
}
