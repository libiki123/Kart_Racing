// PhysicSpec
using System;
using System.IO;
using UnityEngine;

public struct PhysicSpec
{
	public float airFriction;

	public float dragFactor;

	public float forwardAccel;

	public float backwardAccel;

	public float gripBrake;

	public float slipBrake;

	public float maxSteerDeg;

	public float steerConstraint;

	public float frontGripFactor;

	public float rearGripFactor;

	public float driftTrigFactor;

	public float driftTrigTime;

	public float driftSlipFactor;

	public float driftEscapeForce;

	public float cornerDrawFactor;

	public float driftLeanFactor;

	public float steerLeanFactor;

	public float driftMaxGauge;

	public float mass;

	public float width;

	public float length;

	public float springK;

	public float damperCopC;

	public float damperRebC;

	public Vector3[] wheelTranslate;

	public float[] wheelWidth;

	public float normalBoosterTime;

	public float teamBoosterTime;

	public float animalBoosterTime;

	public uint itemSlotCapacity;

	public bool useTransformBooster;

	public void Initialize()
	{
		wheelTranslate = new Vector3[4];
		wheelWidth = new float[4];
		itemSlotCapacity = 2u;
		useTransformBooster = false;
		mass = 100f;
		airFriction = 3f;
		dragFactor = 0.74f;
		forwardAccel = 2000f;
		backwardAccel = 1500f;
		gripBrake = 1800f;
		slipBrake = 1200f;
		maxSteerDeg = 8f;
		steerConstraint = 25f;
		frontGripFactor = 5f;
		rearGripFactor = 5f;
		driftTrigFactor = 0.2f;
		driftTrigTime = 0.2f;
		driftSlipFactor = 0.2f;
		driftEscapeForce = 2500f;
		cornerDrawFactor = 0.2f;
		driftLeanFactor = 0.07f;
		steerLeanFactor = 0.01f;
		driftMaxGauge = 4000f;
		normalBoosterTime = 3000f;
		teamBoosterTime = 4500f;
		animalBoosterTime = 4000f;
	}

	public float getMaxSteerRad()
	{
		return (float)Math.PI * maxSteerDeg / 180f;
	}

}
