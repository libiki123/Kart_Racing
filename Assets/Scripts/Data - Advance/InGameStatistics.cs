// InGameStatistics
public class InGameStatistics
{
	public class ItemUsage
	{
		private int[] _stats = new int[10];

		public ItemUsage()
		{
			Initialize();
		}

		public void Initialize()
		{
			for (int i = 0; i < _stats.Length; i++)
			{
				_stats[i] = 0;
			}
		}

		public int Stat(GameItem item)
		{
			return _stats[(int)item];
		}

		public void SetStat(GameItem item, int value)
		{
			_stats[(int)item] = value;
		}

		public void IncreaseStat(GameItem item)
		{
			_stats[(int)item]++;
		}
	}

	public class OthersStatistics
	{
		private int _shake_booster;

		private int _drift_booster;

		private int _collision;

		public int ShakeBooster
		{
			get
			{
				return _shake_booster;
			}
			set
			{
				_shake_booster = value;
			}
		}

		public int DriftBooster
		{
			get
			{
				return _drift_booster;
			}
			set
			{
				_drift_booster = value;
			}
		}

		public int Collision
		{
			get
			{
				return _collision;
			}
			set
			{
				_collision = value;
			}
		}

		public OthersStatistics()
		{
			Initialize();
		}

		public void Initialize()
		{
			_shake_booster = 0;
			_drift_booster = 0;
			_collision = 0;
		}
	}

	private static InGameStatistics instance_;

	private ItemUsage _total_item_usage;

	private ItemUsage _effective_item_usage;

	private OthersStatistics _others;

	public static InGameStatistics Instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = new InGameStatistics();
			}
			return instance_;
		}
	}

	public ItemUsage TotalItemUsage => _total_item_usage;

	public ItemUsage EffectiveItemUsage => _effective_item_usage;

	public OthersStatistics Others => _others;

	public InGameStatistics()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (_total_item_usage == null)
		{
			_total_item_usage = new ItemUsage();
		}
		if (_effective_item_usage == null)
		{
			_effective_item_usage = new ItemUsage();
		}
		if (_others == null)
		{
			_others = new OthersStatistics();
		}
		_total_item_usage.Initialize();
		_effective_item_usage.Initialize();
		_others.Initialize();
	}
}
