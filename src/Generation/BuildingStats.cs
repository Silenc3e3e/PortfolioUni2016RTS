using System;
using SwinGameSDK;

namespace RTS
{
	public struct BuildingStats
	{
		public Bitmap image;
		public IMoveUnitStats ToSpawn;
		public float CreationTime;
		public int health;

		public void SetToSpawn (IMoveUnitStats stats)
		{
			ToSpawn = stats;
		}
	}
}

