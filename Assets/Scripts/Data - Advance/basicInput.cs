// basicInput
using UnityEngine;

public struct basicInput
{
	public float keyPushTime_;

	public KeyCode keyCode_;

	private KeyState keyState_;

	public basicInput(float time, KeyCode code1)
	{
		keyPushTime_ = 0f;
		keyCode_ = code1;
		keyState_ = KeyState.NONE;
	}

	public void InputUpdate()
	{
		if (keyCode_ == KeyCode.UpArrow)
		{
			
			if (Input.GetKey(KeyCode.UpArrow))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		else if (keyCode_ == KeyCode.DownArrow)
		{
			if (Input.GetKey(KeyCode.DownArrow))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		else if (keyCode_ == KeyCode.LeftArrow)
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		else if (keyCode_ == KeyCode.RightArrow)
		{
			if (Input.GetKey(KeyCode.RightArrow))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		if (keyCode_ == KeyCode.LeftShift)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		if (keyCode_ == KeyCode.LeftControl)
		{
			if (Input.GetKey(KeyCode.LeftControl))
			{
				keyPushTime_ = Time.time;
			}
			else
			{
				keyPushTime_ = 0f;
			}
		}
		keyState_ = KeyStateTransfer.GetKeyState(keyState_, keyPushTime_ > 0f);
	}

	public KeyState GetKeyState()
	{
		return keyState_;
	}
}
