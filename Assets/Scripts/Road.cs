using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Road : MonoBehaviour
{
    #region Fields

    [FormerlySerializedAs("sockets")] [Header("Road")]
    public RoadSocket[] Sockets;

    public Collider collider;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    public RoadSocket GetCloserSocket(Vector3 hitInfoPoint)
    {
        float distance = float.MaxValue;
        int index = 0;
        for (var i = 0; i < Sockets.Length; i++)
        {
            var socket = Sockets[i];
            float sqrtDistance = Vector3.Distance(hitInfoPoint, socket.Transform.position);
            if (sqrtDistance < distance)
            {
                distance = sqrtDistance;
                index = i;
            }
        }

        return Sockets[index];
    }

    public void LockInSocket(RoadSocket socket)
    {
        Vector3 transformPosition = socket.Transform.position - Sockets[0].Transform.localPosition;
        if (Sockets[0].Direction != socket.Direction.Opposite())
            RotateRoad(socket.Direction);
        transform.position = transformPosition;
    }

    private void RotateRoad(Direction direction)
    {
        Direction tmpDir = Sockets[0].Direction;
        Direction newDir = direction;
        int diff = tmpDir.Difference(newDir);
        transform.Rotate(transform.up, 90 * diff);
        foreach (RoadSocket socket in Sockets)
        {
            socket.Direction = socket.Direction.Rotate(diff);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var socket in Sockets)
        {
            Gizmos.DrawWireSphere(socket.Transform.position, 0.2f);
        }
    }
}