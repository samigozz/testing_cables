using UnityEngine;

public class JacksGenerator : MonoBehaviour
{
    [Header("Extensions Grid Settings")] 
    [SerializeField] private int columnLength;
    [SerializeField] private int rowLength;
    [SerializeField] private Vector2 spacing = new (1f, 1f);
    
    [Header("Jack Settings")]
    [SerializeField] private LineJack lineJackPrefab;
    [SerializeField] private Transform lineJacksParent;
    [SerializeField] private Jack extensionJackPrefab;
    [SerializeField] private Transform extensionJacksParent;
    
    private void Start()
    {
        GenerateLineJacks();
        GenerateExtensionJacks();
        CallManager.Instance.CreateMockCall();
    }

    private void GenerateLineJacks()
    {
        if (lineJacksParent == null || lineJackPrefab == null)
            return;

        for (int i = 0; i < columnLength; i++)
        {
            var pos = lineJacksParent.position + new Vector3(i * spacing.x, 0f, 0f);
            var jack = Instantiate(lineJackPrefab, pos, lineJacksParent.transform.rotation, lineJacksParent);
            jack.SocketID = $"Line{i + 1}";
            jack.gameObject.name = $"Jack_{jack.SocketID}";
            
            CallManager.Instance.lineJacks.Add(jack);
        }
    }

    private void GenerateExtensionJacks()
    {
        if (extensionJacksParent == null || extensionJackPrefab == null)
            return;
        
        for (int row = 0; row < rowLength; row++)
        {
            var rowChar = (char)('A' + row);
            
            for (int col = 0; col < columnLength; col++)
            {
                var pos = extensionJacksParent.position + new Vector3(col * spacing.x, -row * spacing.y, 0f);
                var jack = Instantiate(extensionJackPrefab, pos, extensionJackPrefab.transform.rotation, extensionJacksParent);
                jack.SocketID = $"{rowChar}{col + 1}";
                jack.gameObject.name = $"Jack_{jack.SocketID}";
                
                CallManager.Instance.extensionJacks.Add(jack);
            }  
        }
    }
}
