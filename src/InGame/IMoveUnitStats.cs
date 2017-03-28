using System;
using SwinGameSDK;

namespace RTS
{
	public interface IMoveUnitStats
	{
		int Cost{
			get;
		}
		UnitTypes MoveUnitType {
			get;
		}
		Type SpawnType {
			get;
		}
		Bitmap Image {
			get;
		}
		float Speed {
			get;
		}
		int MaxHealth{
			get;
		}
		float Range{
			get;
		}
	}
}