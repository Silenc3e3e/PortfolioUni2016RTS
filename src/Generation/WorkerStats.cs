using System;
using SwinGameSDK;
using System.Collections.Generic;

namespace RTS
{
	public struct WorkerStats : IMoveUnitStats
	{
		public UnitTypes UT;
		public int cost;
		public Type spawnType;
		public Bitmap image;
		public float speed;
		public int maxHealth;
		public int ResourceCarryAmount;
		public float range;

		public UnitTypes MoveUnitType {
			get {
				return UT;
			}
		}
		public int Cost {
			get {
				return cost;
			}
		}
		public Type SpawnType {
			get {
				return spawnType;
			}
		}
		public Bitmap Image {
			get {
				return image;
			}
		}
		public float Speed {
			get {
				return speed;
			}
		}
		public int MaxHealth {
			get {
				return maxHealth;
			}
		}
		public float Range {
			get {
				return range;
			}
		}
	}
}

