using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public Rigidbody rb;

    public Transform frontLeftWheelModel;
    public Transform frontRightWheelModel;
    public Transform rearLeftWheelModel;
    public Transform rearRightWheelModel;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public float topSpeed;
    public float acceleration;
    public float brakePower;
    public float steeringAngle;
    public float minimumSteer = 0.2f;

    private bool isAccelerating;
    private bool isBraking;
    private float steerInput;

    [SerializeField] private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        // Adjust wheel colliders settings
        AdjustWheelColliderSettings(frontLeftWheelCollider);
        AdjustWheelColliderSettings(frontRightWheelCollider);
        AdjustWheelColliderSettings(rearLeftWheelCollider);
        AdjustWheelColliderSettings(rearRightWheelCollider);

        rb.sleepThreshold = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWheelPositions();
    }

    private void FixedUpdate()
    {
        bool grounded = AreWheelsGrounded();

        // Apply acceleration
        if (isAccelerating)
        {
            rearLeftWheelCollider.motorTorque = acceleration * 1000000000 * Time.fixedDeltaTime;
            rearRightWheelCollider.motorTorque = acceleration * 1000000000 * Time.fixedDeltaTime;
        }
        else
        {
            rearLeftWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;
        }

        // Apply braking
        if (isBraking)
        {
            frontLeftWheelCollider.brakeTorque = brakePower * 100 * Time.fixedDeltaTime;
            frontRightWheelCollider.brakeTorque = brakePower * 100 * Time.fixedDeltaTime;
            rearLeftWheelCollider.brakeTorque = brakePower * 100 * Time.fixedDeltaTime;
            rearRightWheelCollider.brakeTorque = brakePower * 100 * Time.fixedDeltaTime;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque = 0;
            frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }

        // Apply steering based on input
        float steeringFactor = 1 - (rb.velocity.magnitude / topSpeed);
        steeringFactor = Mathf.Clamp(steeringFactor, minimumSteer, 1f);

        float steer = steerInput * steeringAngle * steeringFactor;
        frontLeftWheelCollider.steerAngle = steer;
        frontRightWheelCollider.steerAngle = steer;

        // Update wheel positions and rotations
        //AlignVelocityWithForward();
    }

    private void UpdateWheelPositions()
    {
        UpdateWheelPosition(frontLeftWheelCollider, frontLeftWheelModel);
        UpdateWheelPosition(frontRightWheelCollider, frontRightWheelModel);
        UpdateWheelPosition(rearLeftWheelCollider, rearLeftWheelModel);
        UpdateWheelPosition(rearRightWheelCollider, rearRightWheelModel);
    }

    private void UpdateWheelPosition(WheelCollider wheelCollider, Transform wheelModel)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelModel.position = pos;
        wheelModel.rotation = rot;
    }

    private bool AreWheelsGrounded()
    {
        return frontLeftWheelCollider.isGrounded || frontRightWheelCollider.isGrounded ||
               rearLeftWheelCollider.isGrounded || rearRightWheelCollider.isGrounded;
    }

    private void AlignVelocityWithForward()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 forward = transform.forward;
        float forwardSpeed = Vector3.Dot(currentVelocity, forward);

        if (currentVelocity.magnitude > 0.1f)
        {
            // Smoothly interpolate the velocity over time
            Vector3 targetVelocity = new Vector3(forwardSpeed, currentVelocity.y, forwardSpeed);
            rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, 0.1f);  // Adjust smoothness with the 0.1f factor
        }
    }



    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAccelerating = true;
        }
        else if (context.canceled)
        {
            isAccelerating = false;
        }
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBraking = true;
        }
        else if (context.canceled)
        {
            isBraking = false;
        }
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        steerInput = context.ReadValue<float>();
        Debug.Log($"Steering: {steerInput}");
    }

    void AdjustWheelColliderSettings(WheelCollider wheelCollider)
    {
        JointSpring suspensionSpring = wheelCollider.suspensionSpring;
        suspensionSpring.spring = 35000f;
        suspensionSpring.damper = 4500f;
        wheelCollider.suspensionSpring = suspensionSpring;
        wheelCollider.suspensionDistance = 0.0f;

        WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
        forwardFriction.stiffness = 1f;
        wheelCollider.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
        sidewaysFriction.stiffness = 1f;
        wheelCollider.sidewaysFriction = sidewaysFriction;
    }
}
