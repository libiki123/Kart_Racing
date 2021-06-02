// GoKart
using UnityEngine;

public class GoKart
{
	public GameObject m_kart;

	public Vector3 m_KartWLVel;

	public Vector3 m_KartLAVel;

	public Vector3 m_KartRealVelocity;

	public bool stuck_;

	private bool valid_ = true;

	private int status_;

	private bool forcing_;

	private bool isInResetState_;

	private Matrix3 m_ort;

	private Vector3 m_reciprocalMass;

	private Vector3 m_theGravity;

	public Vector3 m_NetWForce;

	private Vector3 m_NetLTorque;

	private int m_boostLeft;

	private float m_slipReserveTime;

	private float m_steer;

	private float m_grip;

	private float[] m_slip = new float[2];

	private bool m_slipBoost;

	public PhysicSpec m_spec;

	public FirstPipelineValue m_first;

	public Suspension m_sus;

	public DriftControl m_drift;

	public DriftGauge m_driftGauge;

	public AdBoost m_adBoost;

	public Control m_ctrl;

	public CollisionState m_cState;

	private External m_extern;

	private DriveFactor[,] m_DriveFactor;

	public bool m_Contact;

	public bool m_isDrift;

	private BoostKind m_BoostKind;

	private uint m_DriveMode;

	public Vector3[] wheelLocalPos_;

	public bool isCrash_;

	public float crashVelocity_;

	public bool isShock_;

	public float shockVelocity_;

	private bool needReset_;

	public bool grounded_;
	public int m_slot;

	private Vector3 backupVelocity_ = Vector3.zero;

	public bool[] isBackupStreer = new bool[3];

	public bool Valid
	{
		get
		{
			return valid_;
		}
		set
		{
			if (valid_ != value)
			{
				valid_ = value;
			}
		}
	}

	public bool IsInResetState
	{
		get
		{
			return isInResetState_;
		}
		set
		{
			isInResetState_ = value;
		}
	}

	public float Wheel
	{
		get
		{
			return m_ctrl.steer;
		}
		set
		{
			m_ctrl.steer = value;
		}
	}


	public GoKart()
	{
		m_theGravity = new Vector3(0f, -49f, 0f);
		m_NetWForce = Vector3.zero;
		m_NetLTorque = Vector3.zero;
		m_slot = 2;
		m_boostLeft = 0;
		m_slipBoost = false;
		m_Contact = true;
		m_spec.Initialize();
		m_sus.Initialize();
		m_drift.Initialize();
		m_driftGauge.Initialize();
		m_adBoost.Initialize();
		m_ctrl.Initialize();
		m_cState.Initialize();
		m_extern.Initialize();
		m_DriveFactor = new DriveFactor[3, 2];
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				m_DriveFactor[i, j].Initialize();
			}
		}
		m_ort = Matrix3.CreateMtxIdentity();
		loadParam();
	}

	public void loadParam()
	{
		m_slipReserveTime = 0f;
		m_steer = (m_grip = 0f);
		m_slip[0] = 2f;
		m_slip[1] = 0.5f;
		m_DriveFactor[1, 0].speedLimit = 340f;
		m_DriveFactor[1, 0].betaCut = 0.6f;
		m_DriveFactor[1, 0].frontGripFactor = -1f;
		m_DriveFactor[1, 0].rearGripFactor = -1f;
		m_DriveFactor[1, 1].betaCut = 0.85f;
		m_DriveFactor[2, 0].speedLimit = 180f;
		m_DriveFactor[2, 0].betaCut = 0.6f;
		m_DriveFactor[2, 0].driftSlipFactor = 0.5f;
		m_DriveFactor[2, 1].frontGripFactor = -2f;
		m_DriveFactor[2, 1].rearGripFactor = -2f;
	}


	public void basicAction(float tick)
	{
		m_isDrift = false;
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (!forcing_ && !isInResetState_)
		{
			if (m_boostLeft > 0)
			{
				m_boostLeft -= Mathf.Min(m_boostLeft, (int)(fixedDeltaTime * 1000f));
				if (m_boostLeft == 0)
				{
					m_BoostKind = BoostKind.NoBoost;
				}
			}
			processAdBoostTime(fixedDeltaTime);
			beginNetForce(fixedDeltaTime);
			if (decideKartContact(fixedDeltaTime, wallInc: false))
			{
				calcNonpenetrateForce(fixedDeltaTime);
				calcKartTractionForce(fixedDeltaTime);
				calcKartSteeringForce(fixedDeltaTime);
			}
			else
			{
				calcFlyingKartForce();
			}
			calcResistForce();
			endNetForce(fixedDeltaTime);

			m_isDrift = (m_isDrift || m_drift.slipMode || m_drift.slipTime > 0f || m_drift.forceSlip);
		}
		else
		{
			m_KartWLVel = Vector3.zero;
			m_KartLAVel = Vector3.zero;
			m_NetWForce = Vector3.zero;
			m_NetLTorque = Vector3.zero;
			if (m_boostLeft > 0)
			{
				m_boostLeft -= Mathf.Min(m_boostLeft, (int)(fixedDeltaTime * 1000f));
				if (m_boostLeft == 0)
				{
					m_BoostKind = BoostKind.NoBoost;
				}
			}
		}
	}

	private void calcResistForce()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		zero -= m_KartWLVel * m_spec.airFriction;
		zero2 -= m_KartLAVel * m_spec.airFriction;
		if (m_Contact)
		{
			zero -= m_KartWLVel * m_KartWLVel.magnitude * m_spec.dragFactor * m_extern.dragFactor * m_extern.compensationDragFactor;
		}
		m_NetWForce += zero;
		m_NetLTorque += zero2;
	}

	private void calcNonpenetrateForce(float deltaT)
	{
		for (uint num = 0u; num < 4; num++)
		{
			float num2 = (!m_sus.wheelContact[num]) ? 0f : ((!(m_sus.deltaTravel[num] > 0f)) ? (m_spec.springK * m_sus.travel[num] + m_spec.damperRebC * (m_sus.deltaTravel[num] / deltaT)) : (m_spec.springK * m_sus.travel[num] + m_spec.damperCopC * (m_sus.deltaTravel[num] / deltaT)));
			num2 = ((!(num2 > 0f)) ? 0f : (Vector3.Dot(m_sus.wheelContactN[num], m_first.up) * num2));
			Vector3 lhs = new Vector3(m_spec.width * m_sus.wheelOff[num].x, 0f, (0f - m_spec.length) * m_sus.wheelOff[num].y);
			Vector3 vector = Vector3.Cross(lhs, new Vector3(0f, num2, 0f)) * 0.1f;
			m_NetLTorque += vector;
		}
		m_NetWForce += m_theGravity * m_spec.mass * m_extern.gravityFactor * 0.8f;
	}

	private void calcKartTractionForce(float deltaT)
	{
		if (m_extern.slip)
		{
			return;
		}
		Vector3 vector = Vector3.Cross(m_first.left, m_sus.contactN);
		if (m_ctrl.getRealAccel() != 0f && !stuck_)
		{
			if (isRealBoost())
			{
				float d = 1.5f;
				if (m_BoostKind == BoostKind.BoostDrift)
				{
					d = 2.5f;
				}
				m_NetWForce += vector * m_ctrl.getRealAccel() * d * ((!m_drift.forceSlip) ? m_spec.forwardAccel : m_spec.driftEscapeForce);
			}
			else
			{
				m_NetWForce += vector * m_ctrl.getRealAccel() * ((!m_drift.forceSlip) ? m_spec.forwardAccel : m_spec.driftEscapeForce);
			}
			if (m_first.frontVel < 0f)
			{
				if (m_drift.slipMode || m_drift.forceSlip)
				{
					m_NetWForce += m_first.front * m_first.speed * m_spec.mass * 9.8f;
				}
				else
				{
					m_NetWForce += m_first.front * Mathf.Min(5f, m_first.speed) * m_spec.mass * 9.8f;
				}
			}
			m_ctrl.stayTime = 0f;
		}
		else if (m_ctrl.getRealBrake() != 0f || stuck_)
		{
			bool flag = true;
			if (m_first.frontVel < 0.5f)
			{
				m_ctrl.stayTime += deltaT;
				if (m_first.frontVel < -0.5f)
				{
					m_ctrl.stayTime = 1f;
				}
				if (m_ctrl.stayTime > 0.2f)
				{
					if (stuck_)
					{
						if (!(m_first.frontVel < -0.5f))
						{
							m_KartWLVel = Vector3.zero;
							flag = false;
						}
					}
					else
					{
						m_NetWForce += vector * m_ctrl.getRealBrake() * (0f - m_spec.backwardAccel);
						flag = false;
					}
				}
				else if (Mathf.Abs(m_first.leftVel) < 0.2f)
				{
					m_KartWLVel = Vector3.zero;
					flag = false;
				}
			}
			if (flag || (m_extern.speedLimit > 0f && m_KartWLVel.sqrMagnitude > 0f))
			{
				Vector3 normalized = m_KartWLVel.normalized;
				normalized -= Vector3.Dot(normalized, m_first.up) * m_first.up;
				if (Vector3.Dot(normalized, vector) > 0.8f)
				{
					m_NetWForce -= normalized * m_spec.gripBrake;
				}
				else
				{
					m_NetWForce -= normalized * m_spec.slipBrake;
				}
			}
		}
		else if (m_first.frontVel <= 0.5f && m_first.frontVel >= -0.5f)
		{
			m_ctrl.stayTime += deltaT;
		}
	}


	private void calcKartSteeringForce(float deltaT)
	{
		if (m_extern.slip)
		{
			return;
		}
		float num = Mathf.Sqrt(m_first.frontVel * m_first.frontVel + m_first.leftVel * m_first.leftVel);
		float num2 = (!(m_first.frontVel > 0f)) ? (-1f) : 1f;
		m_ctrl.steerAngle = m_ctrl.getRealSteer() * m_spec.getMaxSteerRad();
		m_ctrl.steerAngle *= Mathf.Exp(0f - Mathf.Abs(m_first.frontVel / m_spec.steerConstraint * m_extern.wheelFactor));
		isBackupStreer[0] = (m_ctrl.getRealAccel() != 0f);
		isBackupStreer[1] = (m_ctrl.oldSteerAngle * m_ctrl.steerAngle > 0f);
		isBackupStreer[2] = (Mathf.Abs(m_ctrl.oldSteerAngle) < Mathf.Abs(m_ctrl.steerAngle));
		if (isBackupStreer[0] && isBackupStreer[1] && isBackupStreer[2])
		{
			m_ctrl.steerAngle = m_ctrl.oldSteerAngle;
		}
		else
		{
			m_ctrl.oldSteerAngle = m_ctrl.steerAngle;
		}
		float num3 = 0.5f;
		float num4 = 0.5f;
		bool flag = false;
		uint driveMode = 0u;
		bool flag2 = m_drift.slipMode || m_drift.forceSlip;
		m_drift.forceSlip = false;
		if (num > 5f)
		{
			float num5 = 0f;
			float num6 = 0f;
			float num7 = m_KartLAVel.y * num3 / num;
			float num8 = m_KartLAVel.y * num4 / num;
			float num9 = m_first.leftVel / num;
			bool flag3 = false;
			if (m_DriveMode != 0 && m_slipBoost && Mathf.Abs(num9) > m_DriveFactor[m_DriveMode, 0].betaCut && m_adBoost.useLeftTime <= 0f)
			{
				flag = true;
				driveMode = m_DriveMode;
				m_DriveMode = 0u;
				m_adBoost.validTime = 0.5f;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow) && m_adBoost.validTime > 0f && (float)m_boostLeft < 0.5f)
			{
				m_adBoost.validTime = 0f;
				m_adBoost.useLeftTime = 0.5f;
				m_BoostKind = BoostKind.BoostDrift;
				InGameStatistics.Instance.Others.DriftBooster++;
			}
			if (!m_drift.slipMode && !m_drift.trigger && Mathf.Abs(m_first.leftVel) > Mathf.Abs(m_first.frontVel) * 1.2f && num > 15f)
			{
				m_drift.forceSlip = true;
			}
			float z = 0f;
			if (m_drift.trigger)
			{
				m_steer = 0f;
				m_grip = 0f;
				num5 = 0f;
				num6 = (0f - 9.8f * m_spec.mass) * m_spec.frontGripFactor * (m_ctrl.getRealSteer() * m_spec.getMaxSteerRad() * m_spec.driftTrigFactor);
				if (m_drift.triggerTime <= 0f)
				{
					m_drift.triggerTime = m_spec.driftTrigTime;
					m_drift.slipTime = m_drift.triggerTime * 2f;
				}
				else
				{
					m_drift.triggerTime -= deltaT;
					if (m_drift.triggerTime <= 0f)
					{
						m_drift.triggerTime = 0f;
						m_drift.trigger = false;
						if (!m_driftGauge.progressOn)
						{
							m_driftGauge.progressOn = true;
							m_driftGauge.progressTime = 0f;
							m_driftGauge.progress = 0f;
						}
					}
				}
			}
			else if (m_drift.slipMode || m_drift.slipTime > 0f || m_drift.forceSlip || m_slipReserveTime > 0f)
			{
				if (m_DriveMode == 1)
				{
					float num10 = num / m_DriveFactor[m_DriveMode, 1].speedLimit;
					float num11 = num10 * num10;
					if (num11 > 1f)
					{
						num11 = 1f;
					}
					float num12 = 0f;
					float num13 = 0f;
					float num14 = 0f;
					if (m_drift.slipMode)
					{
						m_steer += 1E-06f * (1f - m_steer) * num11;
						m_grip += 0.005f * (1f - m_grip);
						m_NetWForce *= 1f - m_grip;
						num12 = m_ctrl.getRealSteer() * m_spec.getMaxSteerRad() * m_DriveFactor[m_DriveMode, 1].onDriftSteerFactor;
						num13 = m_DriveFactor[m_DriveMode, 1].frontGripFactor;
						num14 = m_DriveFactor[m_DriveMode, 1].rearGripFactor;
					}
					else
					{
						m_steer += 0.001f * (1f - m_steer);
						m_grip = 0f;
						num12 = m_ctrl.steerAngle * m_DriveFactor[m_DriveMode, 1].onRestTimeSteerFactor;
						num13 = m_DriveFactor[m_DriveMode, 1].backFrontGripFactor;
						num14 = m_DriveFactor[m_DriveMode, 1].backRearGripFactor;
					}
					float num15 = m_spec.frontGripFactor + num13;
					float num16 = m_spec.rearGripFactor + num14;
					if (m_steer > 1f)
					{
						m_steer = 1f;
					}
					num16 -= m_grip;
					num5 = 9.8f * m_spec.mass * num15 * (num12 * num2 - num9 - num7);
					num6 = 9.8f * m_spec.mass * num16 * (0f - num9 + num8);
					num5 *= m_spec.driftSlipFactor * m_steer;
					num6 *= m_spec.driftSlipFactor * m_steer;
					//m_slipReserveTime = Mathf.Max(m_slipReserveTime - deltaT, 0f);
				}
				else if (m_drift.slipMode)
				{
					num5 = 9.8f * m_spec.mass * (m_spec.frontGripFactor + m_DriveFactor[m_DriveMode, 1].frontGripFactor) * (m_ctrl.getRealSteer() * m_spec.getMaxSteerRad() * num2 - num9 - num7);
					num6 = 9.8f * m_spec.mass * (m_spec.rearGripFactor + m_DriveFactor[m_DriveMode, 1].rearGripFactor) * (0f - num9 + num8);
					num5 *= m_spec.driftSlipFactor * m_DriveFactor[m_DriveMode, 1].driftSlipFactor;
					num6 *= m_spec.driftSlipFactor * m_DriveFactor[m_DriveMode, 1].driftSlipFactor;
				}
				else
				{
					num5 = 9.8f * m_spec.mass * m_spec.frontGripFactor * (m_ctrl.steerAngle * num2 - num9 - num7);
					num6 = 9.8f * m_spec.mass * m_spec.rearGripFactor * (0f - num9 + num8);
					num5 *= m_spec.driftSlipFactor;
					num6 *= m_spec.driftSlipFactor;
				}
				z = ((!(m_first.speed > 10f)) ? ((0f - (num5 + num6)) * m_spec.driftLeanFactor * 0.5f) : ((0f - (num5 + num6)) * m_spec.driftLeanFactor));
				m_drift.slipTime = Mathf.Max(m_drift.slipTime - deltaT, 0f);
			}
			else
			{
				flag3 = true;
				if (m_DriveMode != 0)
				{
					float num17 = num / m_DriveFactor[m_DriveMode, 0].speedLimit;
					float num18 = m_slip[0] * num17 * num17 + m_slip[1];
					if (num18 > 1f)
					{
						num18 = 1f;
					}
					float num19 = (m_DriveMode != 2) ? (m_ctrl.steerAngle * (1f + num18)) : (m_ctrl.getRealSteer() * m_spec.getMaxSteerRad() * num18);
					num5 = 9.8f * m_spec.mass * m_spec.frontGripFactor * (num19 * num2 - num9 - num7);
					num6 = 9.8f * m_spec.mass * m_spec.rearGripFactor * (0f - num9 + num8);
					num5 *= m_spec.driftSlipFactor * m_DriveFactor[m_DriveMode, 0].driftSlipFactor;
					num6 *= m_spec.driftSlipFactor * m_DriveFactor[m_DriveMode, 0].driftSlipFactor;
				}
				else
				{
					num5 = 9.8f * m_spec.mass * m_spec.frontGripFactor * (m_ctrl.steerAngle * num2 - num9 - num7);
					num6 = 9.8f * m_spec.mass * m_spec.rearGripFactor * (0f - num9 + num8);
				}
				z = (0f - (num5 + num6)) * m_spec.steerLeanFactor;
				if (m_driftGauge.progressOn)
				{
					m_driftGauge.gauge = Mathf.Min(m_spec.driftMaxGauge, m_driftGauge.gauge + m_driftGauge.progress);
					m_driftGauge.progressOn = false;
					m_driftGauge.progressTime = 0f;
					m_driftGauge.lastProgress = m_driftGauge.progress;
					m_driftGauge.progress = 0f;
				}
			}
			m_NetWForce += m_ort * new Vector3(num5 + num6, 0f, (!flag3) ? 0f : ((0f - Mathf.Abs(num5 + num6)) * m_spec.cornerDrawFactor));
			m_NetLTorque += new Vector3(0f, num3 * num5 - num4 * num6, z);
		}
		else
		{
			m_drift.slipMode = false;
			m_drift.slipTime = 0f;
			m_drift.trigger = false;
			m_drift.triggerTime = 0f;
			m_adBoost.validTrigger = false;
			if (m_driftGauge.progressOn)
			{
				m_driftGauge.gauge = Mathf.Min(m_spec.driftMaxGauge, m_driftGauge.gauge + m_driftGauge.progress);
			}
			m_driftGauge.progressOn = false;
			m_driftGauge.progressTime = 0f;
			m_driftGauge.lastProgress = m_driftGauge.progress;
			m_driftGauge.progress = 0f;
			float num20 = m_KartLAVel.y * num3 / 5f;
			float num21 = m_KartLAVel.y * num4 / 5f;
			float num22 = m_first.leftVel / 5f;
			float num23 = 9.8f * m_spec.mass * m_spec.frontGripFactor * (((!(num < 0.5f)) ? (m_ctrl.steerAngle * num2) : 0f) - num22 - num20);
			float num24 = 9.8f * m_spec.mass * m_spec.rearGripFactor * (0f - num22 + num21);
			m_NetWForce += m_ort * new Vector3(num23 + num24, 0f, 0f);
			m_NetLTorque += new Vector3(0f, num3 * num23 - num4 * num24, 0f);
		}
		if (m_adBoost.validTrigger && flag2 && !m_drift.slipMode && !m_drift.forceSlip && m_adBoost.validTime == 0f)
		{
			m_adBoost.validTrigger = false;
			m_adBoost.validTime = 0.5f;
		}
		if (flag)
		{
			m_DriveMode = driveMode;
		}
	}

	public void setReKartOld(GameObject obj, Transform[] wheels)
	{
		m_kart = obj;
		setDefaultSpec();
	}

	private void setDefaultSpec()
	{
		m_spec.springK = m_spec.mass * Mathf.Abs(m_theGravity.y) * 0.5f;
		m_spec.damperCopC = 0f;
		m_spec.damperRebC = m_spec.springK * 0.2f;
		m_reciprocalMass = Vector3.zero;
		Vector3Helper.SetVector3(ref m_reciprocalMass, 12f / m_spec.mass);
	}

	private void beginNetForce(float deltaT)
	{
		m_NetWForce = m_extern.force;
		m_NetLTorque = m_extern.torque;
		m_extern.force = Vector3.zero;
		m_extern.torque = Vector3.zero;
		m_extern.liftVel = Vector3.zero;
		m_first.left = m_kart.transform.right;
		m_first.front = m_kart.transform.forward;
		m_first.up = m_kart.transform.up;
		m_first.frontVel = Vector3.Dot(m_KartWLVel, m_first.front);
		m_first.leftVel = Vector3.Dot(m_KartWLVel, m_first.left);
		m_first.upVel = Vector3.Dot(m_KartWLVel, m_first.up);
		m_first.speed = m_KartWLVel.magnitude;
		m_ort.setCol(m_first.left, m_first.up, m_first.front);
	}

	private void endNetForce(float deltaT)
	{
		m_NetWForce += m_extern.annexForce;
		m_KartWLVel += m_NetWForce / m_spec.mass * deltaT;
		m_KartLAVel += Vector3.Scale(m_reciprocalMass, (m_NetLTorque - Vector3.Cross(m_KartLAVel, Vector3.Scale(m_reciprocalMass, m_KartLAVel))) * deltaT);
	}

	public bool decideKartContact(float deltaT, bool wallInc)
	{
		m_cState.shock = m_Contact;
		m_Contact = false;
		if (m_kart == null)
		{
		}
		for (uint num = 0u; num < 4; num++)
		{
			Vector3 position = m_kart.transform.position;
			position += m_first.left * m_spec.width * m_sus.wheelOff[num].x * 0.8f;
			position += m_first.front * m_spec.length * m_sus.wheelOff[num].y * 0.8f;
			position += m_first.up * m_sus.maxTravel;
			Vector3 origin = position + m_first.up;
			float distance = m_sus.maxTravel * 2f + 1f;
			m_sus.wheelContact[num] = Physics.Raycast(origin, -m_first.up, out RaycastHit hitInfo, distance, LayerMask.GetMask("Road"));
			if (m_sus.wheelContact[num])
			{
				m_Contact = true;
				m_sus.wheelContactN[num] = hitInfo.normal;
				float a = Vector3.Dot(hitInfo.point, m_first.up) - (Vector3.Dot(m_kart.transform.position, m_first.up) - m_sus.maxTravel);
				float num2 = m_sus.travel[num];
				m_sus.travel[num] = Mathf.Max(0f, Mathf.Min(a, m_sus.maxTravel * 2f));
				m_sus.deltaTravel[num] = m_sus.travel[num] - num2;
			}
			else
			{
				m_sus.travel[num] = 0f;
				m_sus.deltaTravel[num] = 0f;
			}
			if (m_sus.wheelContact[num])
			{
			}
		}
		m_sus.contactN = Vector3.zero;
		if (m_Contact)
		{
			uint num3 = 0u;
			for (uint num4 = 0u; num4 < 4; num4++)
			{
				if (m_sus.wheelContact[num4])
				{
					num3++;
					m_sus.contactN += m_sus.wheelContactN[num4];
				}
			}
			m_sus.contactN /= (float)num3;
			m_cState.shock = !m_cState.shock;
			if (num3 > 2 && m_cState.hop)
			{
				m_cState.hop = false;
			}
		}
		else
		{
			m_cState.shock = false;
		}
		return m_Contact;
	}

	public void setAccel(bool accel)
	{
		if (accel)
		{
			m_ctrl.accel = 1f;
			m_slipBoost = true;
		}
		else
		{
			m_ctrl.accel = 0f;
			if (!isZoneBoost())
			{
				m_boostLeft = 0;
				m_BoostKind = BoostKind.NoBoost;
			}
		}
	}

	public void setBrake(bool brake)
	{
		if (brake)
		{
			m_ctrl.brake = 1f;
			if (m_ctrl.accelBrakeSwap && m_adBoost.validTime > 0f)
			{
				m_adBoost.validTime = 0f;
				m_adBoost.useLeftTime = 0.5f;
				m_BoostKind = BoostKind.BoostDrift;
				InGameStatistics.Instance.Others.DriftBooster++;
			}
		}
		else
		{
			m_ctrl.brake = 0f;
		}
	}

	public void setDrift(bool drift)
	{
		if (drift)
		{
			if (m_drift.slipTime <= 0f)
			{
				m_drift.slipMode = true;
				m_drift.trigger = true;
				m_adBoost.validTrigger = (m_first.frontVel > 0f);
			}
		}
		else
		{
			if (m_DriveMode == 1 && m_drift.slipMode && m_slipReserveTime <= 0f)
			{
				float num = m_first.speed / 120f;
				m_slipReserveTime = 10f * num * num;
			}
			m_drift.slipMode = false;
		}
	}

	private void calcFlyingKartForce()
	{
		m_drift.slipMode = false;
		m_drift.forceSlip = false;
		m_drift.trigger = false;
		m_NetWForce += m_theGravity * m_spec.mass * m_extern.gravityFactor;
		m_NetLTorque -= m_KartLAVel * 30f;
		m_ctrl.oldSteerAngle = 0f;
		if (m_cState.hop)
		{
			if (m_first.up.y < 0.05f)
			{
				m_NetLTorque.z += ((!(m_first.left.y > 0f)) ? ((1f - m_first.up.y) * -90f) : ((1f - m_first.up.y) * 90f));
			}
			if (m_first.front.y > 0.5f)
			{
				m_NetLTorque.x += (m_first.front.y + 1f) * 90f;
			}
			else if (m_first.front.y < -0.5f)
			{
				m_NetLTorque.x -= (1f - m_first.front.y) * 90f;
			}
		}
	}

	public bool isRealBoost()
	{
		return isRealBoost(m_BoostKind);
	}

	public bool isRealBoost(BoostKind kind)
	{
		return kind == BoostKind.BoostStart || kind == BoostKind.BoostNormal || kind == BoostKind.BoostTeam || kind == BoostKind.BoostDrift || kind == BoostKind.BoostAnimal;
	}


	public bool isZoneBoost()
	{
		return isZoneBoost(m_BoostKind);
	}

	public bool isZoneBoost(BoostKind kind)
	{
		return kind == BoostKind.BoostZone || kind == BoostKind.BoostJumpZone || kind == BoostKind.BoostDelivery;
	}

	private void processAdBoostTime(float deltaT)
	{
		if (m_adBoost.validTime > 0f)
		{
			m_adBoost.validTime -= deltaT;
			m_adBoost.validTime = Mathf.Max(0f, m_adBoost.validTime);
		}
		if (!(m_adBoost.useLeftTime > 0f))
		{
			return;
		}
		m_adBoost.useLeftTime -= deltaT;
		if (m_adBoost.useLeftTime <= 0f)
		{
			m_adBoost.useLeftTime = 0f;
			if (m_BoostKind == BoostKind.BoostDrift)
			{
				m_BoostKind = BoostKind.NoBoost;
			}
		}
	}
	}

