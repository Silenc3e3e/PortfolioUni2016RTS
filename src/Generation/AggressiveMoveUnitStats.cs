using System;
using SwinGameSDK;

namespace RTS
{
	public struct AggressiveMoveUnitStats : IMoveUnitStats
	{
		public UnitTypes UT;
		public int cost;
		public Type spawnType;
		public Bitmap image;
		public float speed;
		public int maxHealth;
		public int damage;
		public float range;
		public float reloadTime;

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

