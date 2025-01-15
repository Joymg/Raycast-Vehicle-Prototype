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

    [Space] private int[] wheelIsGrounded = new int[4];
    private bool isGrounded;

    [Header("Input")] [SerializeField] private float moveInput;
    [SerializeField] private float steerInput;

    [Header("Vehicle Settings")] 
    [SerializeField] private float _acceleration = 25f;
    [SerializeField] private float _maxSpeed = 100f;
    [SerializeField] private float _deceleration = 10f;
    [SerializeField] private float _steerStrength = 15f;
    [SerializeField] private AnimationCurve _turningCurve;
    [SerializeField] private float _dragCoefficient = 1f;

    private Vector3 _currentVehicleLocalVelocity = Vector3.zero;
    private float _vehicleVelocityRatio = 0f;

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
        if (Input.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(Vector3.up * _debugImpulseForce, ForceMode.Impulse);
            rigidBody.AddTorque(transform.forward * _debugImpulseForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        ApplySuspension();
        GroundCheck();
        CalculateVehicleVelocity();
        Movement();
    }

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
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

            wheelIsGrounded[i] = hasHit ? 1 : 0;
        }
    }

    private void CalculateVehicleVelocity()
    {
        _currentVehicleLocalVelocity = transform.InverseTransformDirection(rigidBody.velocity);
        _vehicleVelocityRatio = _currentVehicleLocalVelocity.z / _maxSpeed;
    }

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelIsGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        if (_currentVehicleLocalVelocity.z < _maxSpeed)
        { 
            rigidBody.AddForce(moveInput * _acceleration * transform.forward, ForceMode.Acceleration);
        }
    }

    private void Deceleration()
    {
        rigidBody.AddForce(_vehicleVelocityRatio * _deceleration * -transform.forward, ForceMode.Acceleration);
    }

    private void Turn()
    {
        rigidBody.AddTorque(
            _steerStrength * steerInput * _turningCurve.Evaluate(Mathf.Abs(_vehicleVelocityRatio)) *
            Mathf.Sign(_vehicleVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = _currentVehicleLocalVelocity.x;
        
        float dragMagnitude = _dragCoefficient * -currentSidewaysSpeed;
        Vector3 dragForce = transform.right * dragMagnitude;
        
        rigidBody.AddForceAtPosition(dragForce, rigidBody.worldCenterOfMass, ForceMode.Acceleration);
    }
}