// KeyStateTransfer
public class KeyStateTransfer
{
	public class KeyStateTransferElem
	{
		private KeyState push_;

		private KeyState release_;

		public KeyStateTransferElem(KeyState push, KeyState release)
		{
			push_ = push;
			release_ = release;
		}

		public KeyState GetKeyState(bool isPush)
		{
			return (!isPush) ? release_ : push_;
		}
	}

	private static KeyStateTransferElem[] KEY_STATE_TRANSFER = new KeyStateTransferElem[4]
	{
		new KeyStateTransferElem(KeyState.PUSH, KeyState.NONE),
		new KeyStateTransferElem(KeyState.PRESS, KeyState.RELEASE),
		new KeyStateTransferElem(KeyState.PRESS, KeyState.RELEASE),
		new KeyStateTransferElem(KeyState.PUSH, KeyState.NONE)
	};

	public static KeyState GetKeyState(KeyState src, bool isPush)
	{
		return KEY_STATE_TRANSFER[(int)src].GetKeyState(isPush);
	}
}
