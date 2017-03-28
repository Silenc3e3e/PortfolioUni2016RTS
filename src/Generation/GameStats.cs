using System;
using System.IO;
using SwinGameSDK;
using System.Collections.Generic;

namespace RTS
{
	public class GameStats
	{
		public BuildingStats _buildingHome;
		public BuildingStats _buildingArchery;
		public BuildingStats _buildingBarraks;
		public AggressiveMoveUnitStats _archerStats;
		public AggressiveMoveUnitStats _swordsmanStats;
		public WorkerStats _workerStats;
		public static int _maxNumberOfWorkers = 4;
		public Stack<TeamAIStats> _teams;


		public GameStats (string MapName)
		{
			object [] Values;
			Values = GetNumbers.GetTheNumbers ("Workers", new string [] { "Worker max" },new object [] {5});
			_maxNumberOfWorkers = (int)Values [0];

			StreamReader reader = new StreamReader (Directory.GetCurrentDirectory () + GameMain.MapsFolder + MapName);
			try {
				string currentLine = reader.ReadLine ();
				while(currentLine != null){
					if(currentLine.StartsWith("t")){
						GameMain.VILeft++;
					}
					currentLine = reader.ReadLine ();
				}
				} finally {
				reader.Close ();
			}
			_teams = new Stack<TeamAIStats> ();
		
			//AI stats. How it's going to think
			for (int i = 0; i <= (GameMain.VILeft -1); i++){
				Random rnd = new Random ();
				Values = GetNumbers.GetTheNumbers ("Team "+Environment.NewLine +(i+1).ToString(),
			                                   		new string [] { "Worker minimum aim", "Worker to fighter Ratio", "Fighter to worker ratio",
					"Swordsman to Archer ratio", "Archer to Swordsman ratio" },
				                                   new object [] { rnd.Next(1,3), rnd.Next (2, 4), rnd.Next (2, 5), rnd.Next (1, 7), rnd.Next (1, 7) });
				if ((int)Values [0] > _maxNumberOfWorkers)
					Values [0] = _maxNumberOfWorkers;
				newTeamAIStats ((int)Values [0], (int)Values [1], (int)Values [2], (int)Values [3], (int)Values [4], BiasNames.Archers);
			}

			//Set worker stats
			Values = GetNumbers.GetTheNumbers ("Worker", new string [] { "Cost","Speed", "MaxHealth",  "Resource Carry limit"}, 
			                                             new object[]{1, 5.75f, 25, 1});
			_workerStats = CreateWorker ((int)Values [0], (float)Values [1], (int)Values [2], (int)Values [3], "Pisk-Worker.png", "WorkerBitmap");

			//Set aggressive stats
			//Swordsman
			Values = GetNumbers.GetTheNumbers ("Swordsman", new string [] { "Cost", "Speed", "MaxHealth", "Damage", "Attack range", "Reload time" },
														 new object [] { 6, 5.5f, 10, 4, 1f, .75f });
			_swordsmanStats = CreateAggressive ((int)Values [0], (float)Values [1], (int)Values [2], (int)Values [3], (float)Values [4], (float)Values [5], "Pisk-Melee.png", "SwordsmanBitmap", UnitTypes.Swordsman);
			//Archer
			Values = GetNumbers.GetTheNumbers ("Archer", new string [] { "Cost", "Speed", "MaxHealth", "Damage", "Attack range", "Reload time" },
														 new object [] { 5, 5.5f, 10, 2, 2f, .5f });
			_archerStats = CreateAggressive ((int)Values [0], (float)Values [1], (int)Values [2], (int)Values [3], (float)Values [4], (float)Values [5], "Pisk-Ranged.png", "ArcherBitmap", UnitTypes.Archer);

			//home base
			Values = GetNumbers.GetTheNumbers ("Home base", new string [] { "Generate Worker length", "Base building health"},
														 new object [] { 8f, 100});
			_buildingHome = CreateBuildingStats ("Pisk-HomeBase.png", "BaseBitmap", _workerStats, (float)Values [0], (int)Values [1]);

			//Archery
			Values = GetNumbers.GetTheNumbers ("Archery", new string [] { "Generate Archer length", "Archery building health" },
														 new object [] { 8f, 100 });
			_buildingArchery = CreateBuildingStats ("Pisk-Archery.png", "ArcheryBitmap", _archerStats, (float)Values [0], (int)Values [1]);

			//Barraks
			Values = GetNumbers.GetTheNumbers ("Barraks", new string [] { "Generate Swordsman length", "Barraks building health" },
														 new object [] { 8f, 100 });
			_buildingBarraks = CreateBuildingStats ("Pisk-Barraks.png", "BarraksBitmap", _swordsmanStats, (float)Values [0], (int)Values [1]);
		}

		private void newTeamAIStats(int WorkerStartBoost, int WorkerToFighterRatio, int FighterToWorkerRatio, int SToA, int AToS, BiasNames bias){
			TeamAIStats newStats = new TeamAIStats ();
			newStats.workerStartBoost = WorkerStartBoost;
			newStats.workerToFighterRatio = WorkerToFighterRatio;
			newStats.fighterToWorkerRatio = FighterToWorkerRatio;
			newStats.ArchersToSwordsmanRatio = AToS;
			newStats.SwordsmanToArchersRatio = SToA;
			newStats.AggressiveBias = bias;
			_teams.Push (newStats);
		}

		public BuildingStats BuildingHome {
			get {
				return _buildingHome;
			}
		}
		public BuildingStats BuildingArchery {
			get {
				return _buildingArchery;
			}
		}
		public BuildingStats BuildingBarraks {
			get {
				return _buildingBarraks;
			}
		}

		private BuildingStats CreateBuildingStats(string imageName, string BitmapName, IMoveUnitStats UnitToGenerate, float CreationTime, int buildingHealth ){
			BuildingStats NewBuilding = new BuildingStats ();
		
			NewBuilding.image = VisibleUnit.AddBitmap (imageName, BitmapName);

			NewBuilding.SetToSpawn (UnitToGenerate);
			NewBuilding.CreationTime = CreationTime;
			NewBuilding.health = buildingHealth;
			return NewBuilding;
		}

		private WorkerStats CreateWorker(int Cost, float Speed, int MaxHealth, int ResourceCarryLimit, string imageName, string BitmapName){
			WorkerStats NewWorker = new WorkerStats ();
			NewWorker.UT = UnitTypes.Worker;
			NewWorker.cost = Cost;
			NewWorker.speed = Speed;
			NewWorker.maxHealth = MaxHealth;
			NewWorker.spawnType = typeof (Worker);
			NewWorker.ResourceCarryAmount = ResourceCarryLimit;
			NewWorker.range = 1f;
			NewWorker.image = VisibleUnit.AddBitmap(imageName, BitmapName);

			return NewWorker;
		}

		private AggressiveMoveUnitStats CreateAggressive(int Cost, float Speed, int MaxHealth, int Damage, float Range, float ReloadTime, string imageName, string BitmapName, UnitTypes ThisAggressiveType)
		{
			AggressiveMoveUnitStats NewAggressive = new AggressiveMoveUnitStats ();
			NewAggressive.UT = ThisAggressiveType;
			NewAggressive.cost = Cost;
			NewAggressive.speed = Speed;
			NewAggressive.maxHealth = MaxHealth;
			NewAggressive.spawnType = typeof (Aggressive);
			NewAggressive.damage = Damage;
			NewAggressive.range = Range;
			NewAggressive.reloadTime = ReloadTime;
			NewAggressive.image = VisibleUnit.AddBitmap (imageName, BitmapName);

			return NewAggressive;
		}
	}
}

