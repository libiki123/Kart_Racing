// TestDriver
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Car_Advanced : MonoBehaviour
{
	public class PrevState
	{
		public Vector3 position_ = Vector3.zero;

		public Vector3 force_ = Vector3.zero;

		public Vector3 velocity_ = Vector3.zero;

		public Vector3 angular_ = Vector3.zero;

		public Vector3 forward_ = Vector3.zero;

		public Quaternion rotate_;

		public bool isGrounded_;
	}

	private enum CallOrder
	{
		FIXED_UPDATE,
		COLLISION_STAY,
		UPDATE
	}

	public enum KartWheelType
	{
		Front = 1,
		Back,
		All
	}

	public enum WheelType
	{
		FRONT_LEFT,
		FRONT_RIGHT,
		BACK_LEFT,
		BACK_RIGHT,
		WHEEL_TYPE_SIZE
	}


	public float speed = 10f;

	public float gravity = 10f;

	public float maxVelocityChange = 100f;

	public bool canJump = true;

	public float jumpHeight = 2f;

	private bool onLoopRoad = false;

	private bool grounded;

	public float rotate = 1f;

	private float stuckedTime_;

	private PrevState prevState_ = new PrevState();


	private GoKart goPlayKart_;

	public Text SpeedShow;
	public Text debugText_;

	public Slider N2OBar;

	private InputManager inputManager = new InputManager();


	private Transform[] wheels_ = new Transform[4];


	public float suspensionDistance = 0.2f;

	private GameObject boosterWave_;



	private bool isBooster_;


	private bool isSuddenChange;

	private int fixedUpdateCount_;


	private int kartResetState_;

	private float kartResetTick_;

	[Header("Car Values")]
	public float driftescapeforce = 950f;
	public float Accel = 910f;
	public float gripbrake = 950f;
	public float boostertime = 910f;
	public float driftget = 590f;
	[Range(0,1)]
	public float driftgaugesaved = 0.7f;
	[Range(2,3)]
	public int itemSlots = 2;
	private void Awake()
	{
		base.GetComponent<Rigidbody>().freezeRotation = false;
		base.GetComponent<Rigidbody>().useGravity = false;

		goPlayKart_ = new GoKart();
		goPlayKart_.m_slot = itemSlots;
		float num = goPlayKart_.m_spec.mass;
		goPlayKart_.m_driftGauge.savedProgress = driftgaugesaved;

		goPlayKart_.m_spec.forwardAccel += 1000f;
		goPlayKart_.m_spec.backwardAccel += 1000f;
		goPlayKart_.m_spec.driftEscapeForce += 1000f;

		goPlayKart_.m_spec.driftMaxGauge += -driftget;
		goPlayKart_.m_spec.forwardAccel += Accel; //前進加速
		goPlayKart_.m_spec.backwardAccel += Accel; //後退加速,前進加速-300
		goPlayKart_.m_spec.gripBrake = gripbrake;
		goPlayKart_.m_spec.slipBrake = gripbrake; //彎道性能
		goPlayKart_.m_spec.driftEscapeForce += driftescapeforce; //甩尾離心力
		goPlayKart_.m_spec.normalBoosterTime += boostertime; //加速器時間
		goPlayKart_.m_spec.teamBoosterTime += boostertime; //團隊加速器時間
		goPlayKart_.m_spec.animalBoosterTime += boostertime; //動物加速器(???
		float num2 = goPlayKart_.m_spec.forwardAccel;
		goPlayKart_.m_spec.forwardAccel = goPlayKart_.m_spec.forwardAccel * goPlayKart_.m_spec.mass / num;
		goPlayKart_.m_spec.backwardAccel = goPlayKart_.m_spec.backwardAccel * goPlayKart_.m_spec.mass / num;
		goPlayKart_.m_spec.gripBrake = goPlayKart_.m_spec.gripBrake * goPlayKart_.m_spec.mass / num;
		goPlayKart_.m_spec.slipBrake = goPlayKart_.m_spec.slipBrake * goPlayKart_.m_spec.mass / num;
		goPlayKart_.m_spec.driftEscapeForce = goPlayKart_.m_spec.driftEscapeForce * goPlayKart_.m_spec.mass / num;

		goPlayKart_.setReKartOld(base.gameObject, wheels_);
	}


	private void Start()
	{
		base.GetComponent<Rigidbody>().mass = 1000f;
		base.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, 0f, 0f);
	}


	private void Update()
	{
		InputUpdate();

		float time = Time.time;
		if (kartResetState_ != 0)
		{
			if (kartResetState_ == 1 && time > kartResetTick_ + 0.5f)
			{
				if (GameObject.Find("defaultCheckPoint")) { 
					GameObject gameObject = GameObject.Find("defaultCheckPoint");
					base.transform.position = gameObject.transform.position;
					base.transform.rotation = gameObject.transform.rotation;
				}
				else
				{
					base.transform.position = new Vector3(0f,0f,0f);
					base.transform.rotation = new Quaternion(0f,0f,0f,0f);

				}
				base.GetComponent<Rigidbody>().AddForce(-base.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
				kartResetState_ = 2;
			}
			if (kartResetState_ == 2 && time > kartResetTick_ + 1f)
			{
				goPlayKart_.IsInResetState = false;
				kartResetState_ = 3;
			}
			if (kartResetState_ == 3 && time > kartResetTick_ + 3f)
			{
				goPlayKart_.Valid = true;
				kartResetState_ = 0;
				kartResetTick_ = 0f;
			}
		}
	}


	private void InputUpdate()
	{
		inputManager.InputUpdate();
		goPlayKart_.setAccel(inputManager.GetKeyPushTime(InputType.UP) > 0f);
		goPlayKart_.setBrake(inputManager.GetKeyPushTime(InputType.DOWN) > 0f);
		float keyPushTime = inputManager.GetKeyPushTime(InputType.LEFT);
		float keyPushTime2 = inputManager.GetKeyPushTime(InputType.RIGHT);
		if (keyPushTime == 0f && keyPushTime2 == 0f)
		{
			goPlayKart_.Wheel = 0f;
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
				goPlayKart_.Wheel = -1f;
			else if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
				goPlayKart_.Wheel = 1f;
			else if(!(Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)))
				goPlayKart_.Wheel = (((keyPushTime < keyPushTime2)) ? 1f : (-1f));
		}
	
		if (inputManager.GetKeyPushTime(InputType.DRIFT) > 0f && goPlayKart_.Wheel != 0f)
		{
			goPlayKart_.setDrift(drift: true);
		}
		else
		{
			goPlayKart_.setDrift(drift: false);
		}

	}


	private void FixedUpdate()
	{
		//fixedUpdateCount_++;

		goPlayKart_.basicAction(Time.time);


		if (isBooster_ != goPlayKart_.isRealBoost())
		{
			isBooster_ = goPlayKart_.isRealBoost();
			if (boosterWave_ != null)
			{
				boosterWave_.SetActive(isBooster_);
			}
		}
		if (grounded)
		{
			goPlayKart_.m_KartLAVel.x = 0f;
			goPlayKart_.m_KartLAVel.z = 0f;

			Quaternion q = base.transform.localRotation;
			Quaternion rhs = MathHelper.CreateQuaternion(0f, goPlayKart_.m_KartLAVel);
			Quaternion q2 = q * rhs;
			MathHelper.QuaMulScala(ref q2, 0.5f * Time.deltaTime);
			MathHelper.QuaAdd(ref q, q2);
			MathHelper.QuaNormalize(ref q);
			base.transform.localRotation = q;
			prevState_.rotate_ = q;
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			Vector3 force = goPlayKart_.m_KartWLVel - velocity;
			force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
			force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
			force.y = 0f;
			base.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
			base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		base.GetComponent<Rigidbody>().AddForce(new Vector3(0f, (0f - gravity) * base.GetComponent<Rigidbody>().mass, 0f));

		Vector3 a = base.GetComponent<Rigidbody>().position - prevState_.position_;
		goPlayKart_.m_KartRealVelocity = a / Time.deltaTime;
		prevState_.position_ = base.GetComponent<Rigidbody>().position;
		prevState_.velocity_ = goPlayKart_.m_KartWLVel;
		prevState_.angular_ = base.GetComponent<Rigidbody>().angularVelocity;
		prevState_.forward_ = base.transform.forward;
		prevState_.isGrounded_ = grounded;
		goPlayKart_.grounded_ = grounded;

		grounded = false;
		if (isSuddenChange)
		{
			isSuddenChange = false;

		}

	}


	private void OnCollisionStay(Collision collisionInfo)
	{
		OnCollisionDetection(collisionInfo);
	}

	private void OnCollisionDetection(Collision collisionInfo)
	{
		if ((base.GetComponent<Rigidbody>().velocity - prevState_.velocity_).magnitude >= 10f)
		{
			isSuddenChange = true;
		}
		int num = 0;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		ContactPoint[] contacts = collisionInfo.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			zero2 += contactPoint.point;
			zero += contactPoint.normal;
			num++;
		}
		zero2 /= (float)num;
		zero /= (float)num;
		float num2 = Vector3.Dot(zero, prevState_.velocity_);
		if (num2 < 0f)
		{
			if ((double)zero.y >= 0.65)
			{
				grounded = true;

			}
		}
		else
		{
			grounded = true;
		}
		goPlayKart_.m_KartWLVel = base.GetComponent<Rigidbody>().velocity;
	}

}
