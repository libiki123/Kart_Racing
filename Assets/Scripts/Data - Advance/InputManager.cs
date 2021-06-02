// InputManager
using UnityEngine;

public class InputManager
{
	private basicInput[] keyArray_ = new basicInput[6]
	{
		new basicInput(0f, KeyCode.LeftArrow),
		new basicInput(0f, KeyCode.RightArrow),
		new basicInput(0f, KeyCode.UpArrow),
		new basicInput(0f, KeyCode.DownArrow),
		new basicInput(0f, KeyCode.LeftShift),
		new basicInput(0f, KeyCode.LeftControl)
	};

	public void InputUpdate()
	{
		for (int i = 0; i < 6; i++)
		{
			keyArray_[i].InputUpdate();
		}
		/*if (keyArray_[0].keyPushTime_ > keyArray_[1].keyPushTime_)
		{
			keyArray_[1].keyPushTime_ = 0f;
			return;
		}
		if (keyArray_[0].keyPushTime_ < keyArray_[1].keyPushTime_)
		{
			keyArray_[0].keyPushTime_ = 0f;
			return;
		}
		//keyArray_[0].keyPushTime_ = 0f;
		//keyArray_[1].keyPushTime_ = 0f;
		*/
	}

	public float GetKeyPushTime(InputType type)
	{
		return keyArray_[(int)type].keyPushTime_;
	}

	public KeyState GetKeyState(InputType type)
	{
		return keyArray_[(int)type].GetKeyState();
	}
}
