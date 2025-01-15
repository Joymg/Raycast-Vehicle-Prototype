using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class RaycastVehicle : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [Header("Debug")] [SerializeField] private float _debugImpulseForce = 10;

    [Header("Suspension")] [SerializeField]
    private List<Transform> suspensionPoints = new List<Transform>(4);

    [SerializeField] private float _suspensionStiffness = 6;
    [SerializeField] private float _damperStiffness = 6;
    [SerializeField] private float _restLength = 0.25f;
    [SerializeField] private float _springTravel = 0.25f;
    [SerializeField] private float _wheelRadius = 0.33f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(Vector3.up * _debugImpulseForce, ForceMode.Impulse);
            rigidBody.AddTorque(transform.forward * _debugImpulseForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        ApplySuspension();
    }

    private void ApplySuspension()
    {
        for (int i = 0; i < suspensionPoints.Count; i++)
        {
            float maxLength = _restLength + _springTravel;
            bool hasHit = Physics.Raycast(suspensionPoints[i].position, -suspensionPoints[i].up, out RaycastHit hit,
                maxLength + _wheelRadius);
            if (hasHit)
            {
                float currentSpringLength = hit.distance - _wheelRadius;
                float springCompression = (_restLength - currentSpringLength) / _springTravel;

                float springVelocity = Vector3.Dot(rigidBody.GetPointVelocity(suspensionPoints[i].position),
                    suspensionPoints[i].up);
                float dampForce = _damperStiffness * springVelocity;

                float springForce = springCompression * _suspensionStiffness;
                float totalForce = springForce - dampForce;

                rigidBody.AddForceAtPosition(suspensionPoints[i].up * totalForce,
                    suspensionPoints[i].position);

                Debug.DrawLine(suspensionPoints[i].position, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(suspensionPoints[i].position,
                    suspensionPoints[i].position + (_wheelRadius + maxLength) * -suspensionPoints[i].up, Color.green);
            }
        }
    }
}