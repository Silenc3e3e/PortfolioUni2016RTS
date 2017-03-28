using System;
using SwinGameSDK;
using System.Collections.Generic;

namespace RTS
{
	public class Building : Unit, IProcessor
	{
		private BuildingStats _buildingStatsRef;
		private int _numberUnitsToCreate;
		private float _timer;
		private bool _intact;

		public Building (ref BuildingStats BuildingStats, Point2D position, ref VirtualIntelligence Master) : base(BuildingStats.image, position, ref Master, BuildingStats.health)
		{
			_buildingStatsRef = BuildingStats;
			_timer = 0f;
			_intact = true;
		}

		public void Process ()
		{
			if (_numberUnitsToCreate >= 1) {
				_timer -= 1/60f;
				if (_timer <= 0f) {
					SpawnUnit ();
					_timer = _buildingStatsRef.CreationTime;
					_numberUnitsToCreate -= 1;
				}
			}
		}

		public int CreateUnit ()
		{
			if (_masterRef.TakeResources(_buildingStatsRef.ToSpawn.Cost) ) {
				_numberUnitsToCreate ++;
				return 1;
			}
			return 0;
		}

		private void SpawnUnit ()
		{
			GameMain.NumTeamA++;
			object [] newUnitParams = { Position, _masterRef, _buildingStatsRef.ToSpawn };
			MoveUnit newUnit = (MoveUnit)Activator.CreateInstance (_buildingStatsRef.ToSpawn.SpawnType, newUnitParams);
		}

		public int CostToCreateUnit
		{
			get {
				return _buildingStatsRef.ToSpawn.Cost;
			}
		}
		public bool Intact{
			get{
				return _intact;
			}
		}
		public override void death ()
		{
			GameMain.BuildingsProcessList.Remove (this);
			_masterRef.TheMap.RemoveFromUnitList (this);
			_intact = false;
			_masterRef.ReduceRegisterLessUnits (UnitTypes.Building,1);
			_timer = 999999f;
			_masterRef.ReduceRegisterLessUnits (_buildingStatsRef.ToSpawn.MoveUnitType, _numberUnitsToCreate);
			_numberUnitsToCreate = 0;
			_masterRef.TheMap.RemoveFromGrid (this);
			base.death ();
		}
	}
}