using System;
using System.Collections.Generic;
using SwinGameSDK;

namespace RTS
{
	public delegate void bias();

	public class VirtualIntelligence : IProcessor
	{
		private Map _mapRef;
		private Building _homeBase;
		private Building _archery;
		private Building _barraks;
		private TeamAIStats _myStats;
		private Color _teamColor;
		private int _teamNumber;

		public Color TeamColor{
			get{
				return _teamColor;
			}
		}
		public int TeamNumber{
			get{
				return _teamNumber;
			}
		}

		//variable
		private int _resources;
		private int _numOfWorkers;
		private int _numberSwordsman;
		private int _numberArchers;
		private int _buildingsLeft;
		private bias _bias;

		private int NumberOfAggressive{
			get{
				return _numberSwordsman+_numberArchers;
			}
		}

		public VirtualIntelligence (ref Map MapRef, Building HomeBase, Building Archery, Building Barraks, TeamAIStats MyStats, Color VITeamColor)
		{
			_teamColor = VITeamColor;

			_numOfWorkers = 0;
			_numberSwordsman = 0;
			_numberArchers = 0;
			_buildingsLeft = 3;

			_mapRef = MapRef;
			_myStats = MyStats;

			_homeBase = HomeBase;
			_homeBase.Master = this;

			_archery = Archery;
			_archery.Master = this;

			_barraks = Barraks;
			_barraks.Master = this;

			switch (_myStats.AggressiveBias){
				case BiasNames.Archers:
					_bias = new bias( CreateArcher);
				break;
				default:
					_bias = new bias (CreateSwords);
				break;
			}
		}

		public Map TheMap {
			get {
				return _mapRef;
			}
		}
		public Building HomeBase {
			get {
				return _homeBase;
			}
		}



		public void Process ()
		{
			if (_numOfWorkers <= 0 && _homeBase.Intact) {
				int cost = _homeBase.CostToCreateUnit;
				if (_resources < cost)
					_resources = cost;
				CreateWorker ();
			}
			else if (_numOfWorkers < GameStats._maxNumberOfWorkers &&(_numOfWorkers<_myStats.workerStartBoost || _numOfWorkers<(NumberOfAggressive/_myStats.fighterToWorkerRatio)*_myStats.workerToFighterRatio) && _homeBase.Intact){
				CreateWorker ();
			} else {//Create aggressive unit
				if ((_numberArchers < (_numberSwordsman / _myStats.ArchersToSwordsmanRatio) * _myStats.SwordsmanToArchersRatio) && _archery.Intact) {
					CreateArcher ();
				} else if ((_numberSwordsman < (_numberArchers / _myStats.SwordsmanToArchersRatio) * _myStats.ArchersToSwordsmanRatio) && _barraks.Intact) {
					CreateSwords ();
				} else{
					_bias.Invoke ();
				}

			}
		}
		public void CreateWorker () { _numOfWorkers += _homeBase.CreateUnit (); }
		public void CreateArcher () { _numberArchers += _archery.CreateUnit (); }
		public void CreateSwords () { _numberSwordsman += _barraks.CreateUnit (); }


		public void ReduceRegisterLessUnits (UnitTypes UT) { ReduceRegisterLessUnits (UT, 1);}
		public void ReduceRegisterLessUnits(UnitTypes UnitType, int Amount){
			switch(UnitType){
				case UnitTypes.Swordsman:
					_numberSwordsman-=Amount;
				break;
				case UnitTypes.Archer:
					_numberArchers-= Amount;
				break;
				case UnitTypes.Worker:
					_numOfWorkers-= Amount;
				break;
				case UnitTypes.Building:
					_buildingsLeft-= Amount;
				break;
			}
			if(_numberSwordsman <= 0 && _numOfWorkers <=0 && _numberArchers<=0 && _buildingsLeft<=0){
				GameMain.VILeft--;
				GameMain.VIProcessList.Remove (this);//Make this check who the winner also is. MUST be a VI, for the team number, and Color
			}
		}

		public bool TakeResources (int Amount)
		{
			if (_resources >= Amount) {
				_resources -= Amount;
				return true;
			} else
				return false;
		}
		public void AddResources (int Amount)
		{
			_resources += Amount;
		}
	}

	public enum UnitTypes{
		Swordsman, Archer, Worker, Building
	}
}