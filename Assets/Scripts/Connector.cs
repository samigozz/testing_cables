using System;
using UnityEngine;

/*
Copyright (c) 2022 Hrober

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

public class Connector : MonoBehaviour
{
    public enum EConnectorType
    {
        Plug,
        Socket
    }
    
    public EConnectorType connectorType = EConnectorType.Plug;
    [SerializeField] private Transform connectionPoint;
    public bool IsKinematic = false;
    
    // Connection transform required variables.
    public Vector3 ConnectionPosition => connectionPoint ? connectionPoint.position : transform.position;
    public Quaternion ConnectionRotation => connectionPoint ? connectionPoint.rotation : transform.rotation;
    public Quaternion RotationOffset => connectionPoint ? connectionPoint.localRotation : Quaternion.Euler(Vector3.zero);
    public Vector3 ConnectedOutOffset => connectionPoint ? connectionPoint.right : transform.right;
    
    public Connector Connection { get; private set; }
    public bool IsConnected =>  Connection != null;

    private Rigidbody _rigidbody;
    private FixedJoint _fixedJoint;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (Connection != null)
        {
            Connect(Connection);
            Connection = null;
        }
    }

    public void SetConnection(Connector connector)
    {
        Connection = connector;
    }
    
    public bool CanConnect(Connector connector) =>
        this != connector
        && !this.IsConnected && !connector.IsConnected
        && this.connectorType != connector.connectorType;

    public void Connect(Connector connector)
    {
        if (!connector)
            return;
        
        if(IsConnected)
            Disconnect();

        connector.transform.rotation = ConnectionRotation * connector.RotationOffset;
        connector.transform.position = ConnectionPosition - (connector.ConnectionPosition - connector.transform.position);

        _fixedJoint = gameObject.AddComponent<FixedJoint>();
        _fixedJoint.connectedBody = connector._rigidbody;
        
        connector.SetConnection(this);
        Connection = connector;
    }

    public void Disconnect()
    {
        if (!Connection)
            return;
        
        Connector toDisconnect = Connection;
        Destroy(_fixedJoint);
        Connection = null;
        toDisconnect.Disconnect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.connectorType != EConnectorType.Socket)
            return;

        if (!other.gameObject.TryGetComponent(out Connector connector))
            return;
        
        if (CanConnect(connector))
        {
            connector.Connect(this);
        }
        else if (!connector.IsConnected)
        {
            transform.rotation = connector.ConnectionRotation * this.RotationOffset;
            transform.position = (connector.ConnectionPosition + connector.ConnectedOutOffset * 0.2f) - (ConnectionPosition - connector.transform.position);
        }
    }
}
