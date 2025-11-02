using System;
using UnityEngine;

public class WagonAI : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _frontLeft;
    [SerializeField] private WheelCollider _frontRight;
    [SerializeField] private WheelCollider _rearLeft;
    [SerializeField] private WheelCollider _rearRight;

    [Header("Wheel Meshes")]
    [SerializeField] private Transform _frontLeftMesh;
    [SerializeField] private Transform _frontRightMesh;
    [SerializeField] private Transform _rearLeftMesh;
    [SerializeField] private Transform _rearRightMesh;

    [Header("Path Settings")]
    [SerializeField] private float _targetSpeed = 1f;    // Target speed (m/s)
    [SerializeField] private float _baseTorque = 300f;   // Basic traction
    [SerializeField] private float _slopeBoost = 900f;   // Additional traction uphill
    [SerializeField] private float _brakeTorque = 300f;  // Brake at excessive speed
    [SerializeField] private float _slopeThreshold = 2f; // Minimum tilt angle for compensation
    [SerializeField] private float _waypointThreshold = 3f;
    [SerializeField] private Transform[] _waypoints;
    

    private int _currentWaypoint = 0;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = new Vector3(0, -0.7f, 0); // for stability
        _frontLeft.radius += 0.02f;
        _frontRight.radius += 0.02f;
        _rearLeft.radius += 0.02f;
        _rearRight.radius += 0.02f;
    }

    private void FixedUpdate()
    {
        if (_waypoints.Length == 0) return;

        // Current target point
        Vector3 target = _waypoints[_currentWaypoint].position;
        Vector3 localTarget = transform.InverseTransformPoint(target);

        // Calculating the front wheel steering angle
        float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f) * 30f;
        _frontLeft.steerAngle = steer;
        _frontRight.steerAngle = steer;
        /*
        if (!_frontLeft.isGrounded && !_frontRight.isGrounded)
        {
            _rb.AddForce(Vector3.down * 200f, ForceMode.Acceleration); 
        }
        */       
        // Switch to the next point if close
        if (Vector3.Distance(transform.position, target) < _waypointThreshold)
        {
            _currentWaypoint = (_currentWaypoint + 1) % _waypoints.Length;
        }

        // Updating the positions of the visual wheels
        UpdateWheelMeshes();

        HillAssist();
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelPose(_frontLeft, _frontLeftMesh);
        UpdateWheelPose(_frontRight, _frontRightMesh);
        UpdateWheelPose(_rearLeft, _rearLeftMesh);
        UpdateWheelPose(_rearRight, _rearRightMesh);
    }

    private void UpdateWheelPose(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        // We correct the rotation if the model's X axis does not match
        rot *= Quaternion.Euler(0, 0, 90);

        mesh.position = pos;
        mesh.rotation = rot;
    }

    private void HillAssist()
    {
        // Current speed along the direction of travel
        float forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);

        // Normal to the surface under the cart
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            return;

        Vector3 normal = hit.normal;
        float slopeAngle = Vector3.Angle(normal, Vector3.up);
        bool uphill = Vector3.Dot(transform.forward, Vector3.Cross(Vector3.right, normal)) > 0;

        float torque = _baseTorque;

        // If the slope is greater than the threshold, we add compensation
        if (slopeAngle > _slopeThreshold)
        {
            float slopeFactor = Mathf.Sin(slopeAngle * Mathf.Deg2Rad);

            if (uphill)
                torque += _slopeBoost * slopeFactor;
            else
                torque -= _slopeBoost * slopeFactor;
        }

        // Correction based on current speed (to maintain targetSpeed)
        float speedError = _targetSpeed - forwardSpeed;
        torque += speedError * 100f;

        torque = Mathf.Clamp(torque, -_slopeBoost, _slopeBoost);

        _frontLeft.motorTorque = torque;
        _frontRight.motorTorque = torque;

        // We slow down when exceeding the speed limit
        if (forwardSpeed > _targetSpeed + 0.2f)
        {
            _frontLeft.brakeTorque = _brakeTorque;
            _frontRight.brakeTorque = _brakeTorque;
        }
        else
        {
            _frontLeft.brakeTorque = 0;
            _frontRight.brakeTorque = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < _waypoints.Length - 1; i++)
        {
            Vector3 prev = _waypoints[i].position;
            Vector3 next = _waypoints[i + 1].position;
            Gizmos.DrawLine(prev, next);  
        }
    }
} 
