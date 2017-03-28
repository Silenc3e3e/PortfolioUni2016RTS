using System;
using System.Collections.Generic;
using SwinGameSDK;

namespace RTS
{
	public class Worker : MoveUnit
	{
		private WorkerStats _unitStats;
		private Resource _target;
		private bool _goingToResource;

		public Worker (Point2D position, ref VirtualIntelligence Master, ref IMoveUnitStats WorkerStats) : base (position, ref Master, ref WorkerStats)
		{
			_goingToResource = true;
			_unitStats = (RTS.WorkerStats)WorkerStats;
			_target = _masterRef.TheMap.FindResource (_masterRef, position);
			NewPath (_target.Position);
		}

		public override void Process ()
		{
			if (idle) {
				if (_goingToResource) {
					_goingToResource = false;
					NewPath (_masterRef.HomeBase.Position);
				} else {
					_masterRef.AddResources (_unitStats.ResourceCarryAmount);
					NewPath (_masterRef.TheMap.FindResource (_masterRef, Position).Position);
					_goingToResource = true;
				}
				idle = false;
			}
			base.Process ();
		}
	}
}

