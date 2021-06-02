using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Basic : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private bool isBreaking;
	private float currentBreakForce;
	private float currentSteerAngle;

    [SerializeField] private WheelCollider wheelColliderFontRight;
    [SerializeField] private WheelCollider wheelColliderFontLeft;
    [SerializeField] private WheelCollider wheelColliderBackRight;
    [SerializeField] private WheelCollider wheelColliderBackLeft;

	[SerializeField] private Transform wheelFrontRight;
	[SerializeField] private Transform wheelFrontLeft;
	[SerializeField] private Transform wheelBackRight;
	[SerializeField] private Transform wheelBackLeft;

	[SerializeField] private Transform centerOfMass;

	[SerializeField] private float motorForce;
	[SerializeField] private float breakForce;
	[SerializeField] private float maxSteerAngle;


	private void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
	}

	private void FixedUpdate()
	{
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

	private void GetInput()
	{
		isBreaking = Input.GetKey(KeyCode.Space);
	}

	private void HandleMotor()
	{
		wheelColliderFontRight.motorTorque = Input.GetAxis(VERTICAL) * motorForce;
		wheelColliderFontLeft.motorTorque = Input.GetAxis(VERTICAL) * motorForce;
		wheelColliderBackRight.motorTorque = Input.GetAxis(VERTICAL) * motorForce;
		wheelColliderBackLeft.motorTorque = Input.GetAxis(VERTICAL) * motorForce;
		currentBreakForce = isBreaking ? breakForce : 0f;
		if (isBreaking)
		{
			ApplyBreak();
		}
	}
	private void ApplyBreak()
	{
		wheelColliderFontRight.brakeTorque = currentBreakForce;
		wheelColliderFontLeft.brakeTorque = currentBreakForce;
		wheelColliderBackRight.brakeTorque = currentBreakForce;
		wheelColliderBackLeft.brakeTorque = currentBreakForce;
	}

	private void HandleSteering()
	{
		currentSteerAngle = maxSteerAngle * Input.GetAxis(HORIZONTAL);
		wheelColliderFontRight.steerAngle = currentSteerAngle;
		wheelColliderFontLeft.steerAngle = currentSteerAngle;
	}

	private void UpdateWheels()
	{
		UpdateSingleWheel(wheelColliderFontRight, wheelFrontRight);
		UpdateSingleWheel(wheelColliderFontLeft, wheelFrontLeft);
		UpdateSingleWheel(wheelColliderBackRight, wheelBackRight);
		UpdateSingleWheel(wheelColliderBackLeft, wheelBackLeft);
	}

	private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
	{
		Vector3 pos;
		Quaternion rot;
		wheelCollider.GetWorldPose(out pos,out rot);
		wheelTransform.rotation = rot;
		wheelTransform.position = pos;

	}
}
