using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Wire : MonoBehaviour
{

    [Header("Config")] 
    [SerializeField] private Connector startConnector; 
    [SerializeField] private Connector endConnector;
    [SerializeField] private Transform segmentParent;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private int segmentCount = 10;
    [SerializeField] private float totalLength = 10f;
    
    [Header("Mesh")]
    [SerializeField] private int sides = 4;
    
    [Header("Physics")]
    [SerializeField] private float totalWeight = 10f;
    [SerializeField] private float linearDamp = 1f;
    [SerializeField] private float angularDamp = 1f;
    [SerializeField] private bool useCollision = false;
    
    [Header("Debug")]
    [SerializeField] private bool debugDraw = false;

    private Transform[] _segments;
    private int _prevSegmentCount;
    
    private Vector3[] _vertices;
    private int[,] _vertexIndicesMap;
    
    private bool _createTriangles = false;
    
    private WireMeshData _meshData;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _segments = new Transform[segmentCount];
        _vertices = new Vector3[segmentCount * sides * 3];
        
        GenerateSegments();
        GenerateMesh();
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;
        
        if(_segments  == null)
            return;
        
        foreach (var segment in _segments)
        {
            Gizmos.DrawWireSphere(segment.position, 0.1f);
        }

        if (_vertices == null)
            return;

        foreach (var vert in _vertices)
        {
            Gizmos.DrawSphere(vert, 0.05f);
        }
    }

    private void GenerateSegments()
    {
        if(!startConnector && !endConnector)
            return;
        
        var startTransform = startConnector.transform;
        var endTransform = endConnector.transform;
        
        JoinSegments(startTransform, null, startConnector.connectorType == Connector.EConnectorType.Socket || startConnector.IsKinematic);
        
        var prevTransform = startTransform;
        var direction = endTransform.position - startTransform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject($"segment_{i}");
            segment.transform.SetParent(segmentParent);
            _segments[i] = segment.transform;
            
            var pos = prevTransform.position + (direction / segmentCount);
            segment.transform.position = pos;

            JoinSegments(segment.transform, prevTransform);
            
            prevTransform = segment.transform;
        }
        
        JoinSegments(endConnector.transform, prevTransform, endConnector.connectorType == Connector.EConnectorType.Socket || endConnector.IsKinematic, true);
    }

    private void JoinSegments(Transform current, Transform connectedTransform, bool isKinematic = false, bool isCloseConnected = false)
    {
        var rb = current.AddComponent<Rigidbody>();
        rb.isKinematic = isKinematic;
        rb.mass = totalWeight / segmentCount;
        rb.linearDamping = linearDamp;
        rb.angularDamping = angularDamp;

        if (useCollision)
        {
            var sphereCollider = current.AddComponent<SphereCollider>();
            sphereCollider.radius = radius;
        }

        if (connectedTransform == null) 
            return;
        
        var joint = current.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedTransform.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;

        joint.connectedAnchor = isCloseConnected ? Vector3.forward * 0.1f : Vector3.forward * (totalLength / segmentCount);

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        var softJointLimit = new SoftJointLimit();
        softJointLimit.limit = 0;
        joint.angularZLimit = softJointLimit;

        var jointDrive = new JointDrive();
        jointDrive.positionDamper = 0;
        jointDrive.positionSpring = 0;
        joint.angularXDrive = jointDrive;
        joint.angularYZDrive = jointDrive;
    }

    private void GenerateMesh()
    {
        _createTriangles = true;

        if (_meshData == null)
        {
            _meshData = new WireMeshData(sides, segmentCount + 1, false);
        }
        else
        {
            _meshData.ResetMesh(sides, segmentCount + 1, false);
        }
        
        GenerateIndicesMap();
        GenerateVertices();
        
        _meshData.ProcessMesh();
        _mesh = _meshData.CreateMesh();
        
        _meshFilter.sharedMesh = _mesh;
        
        _createTriangles = false;
    }

    private void UpdateMesh()
    {
        GenerateVertices();
        _meshFilter.mesh.vertices = _vertices;
    }

    private void GenerateIndicesMap()
    {
        _vertexIndicesMap = new int[segmentCount + 1, sides + 1];
        int meshVertexIndex = 0;

        for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
        {
            for (int sideIndex = 0; sideIndex < sides; sideIndex++)
            {
                _vertexIndicesMap[segmentIndex, sideIndex] = meshVertexIndex;
                meshVertexIndex++;
            }
        }
    }

    private void GenerateVertices()
    {
        for (int i = 0; i < _segments.Length; i++)
        {
            GenerateCircleVerticesAndTriangles(_segments[i], i);
        }
    }

    private void GenerateCircleVerticesAndTriangles(Transform segment, int segmentIndex)
    {
        float angleDiff = 360f / sides;
        Quaternion diffRotation = Quaternion.FromToRotation(Vector3.forward, segment.forward);

        for (int sideIndex = 0; sideIndex < sides; sideIndex++)
        {
            float angleInRad = sideIndex * angleDiff * Mathf.Deg2Rad;
            float x = -1 * Mathf.Cos(angleInRad) * radius;
            float y = Mathf.Sin(angleInRad) * radius;
            
            Vector3 pointOffset = new Vector3(x, y, 0);
            Vector3 pointRot = diffRotation * pointOffset;
            Vector3 vertexPos = segment.position + pointRot;

            var vertexIndex = segmentIndex * sides + sideIndex;
            _vertices[vertexIndex] = vertexPos;

            if (!_createTriangles) 
                continue;
            
            _meshData.AddVertex(vertexPos, new Vector2(0, 0), vertexIndex);

            if (segmentIndex >= segmentCount - 1) 
                continue;
            
            int currentIncrement = 1;
            int a = _vertexIndicesMap[segmentIndex, sideIndex];
            int b = _vertexIndicesMap[segmentIndex + currentIncrement, sideIndex];
            int c = _vertexIndicesMap[segmentIndex, sideIndex + currentIncrement];
            int d = _vertexIndicesMap[segmentIndex + currentIncrement, sideIndex + currentIncrement];

            if (sideIndex == sides - 1)
            {
                c = _vertexIndicesMap[segmentIndex, 0];
                d = _vertexIndicesMap[segmentIndex + currentIncrement, 0];
            }
                
            _meshData.AddTriangle(a, d, c);
            _meshData.AddTriangle(d, a, b);
        }
    }
}
