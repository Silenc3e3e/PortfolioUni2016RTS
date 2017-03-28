using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SwinGameSDK;
using System.IO;
using System.Diagnostics;

namespace RTS
{
	public class Map
	{
		//private GameStats _gameStatsRef;
		//private List<IProcessor> _VIProcessRef;
		//private List<IProcessor> _BuildingProcessRef;
		//private List<IProcessor> _MoveUnitProcessRef;
		//private List<Unit> _DeahtListRef;
		//private List<MoveUnit> _MoveUnitMoveRef;
		//private List<VisibleUnit> _drawList;
		private Dictionary<Point2D, VisibleUnit> _grid;
		private List<Resource> _allResources;
		private List<Unit> _allUnits;
		private string _mapLocation;
		private static Color [] _TeamColors = new Color [] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Brown, Color.Pink, Color.Orange, Color.Violet, Color.Grey};


		public void RemoveFromGrid (VisibleUnit VU)
		{
			_grid.Remove (VU.Position);
		}

		public void AddToUnitList (Unit U)
		{
			_allUnits.Add (U);
		}
		public void RemoveFromUnitList (Unit U)
		{
			_allUnits.Remove (U);
		}
		//public void AddToDeathList(Unit U){
		//	if (!_DeahtListRef.Exists ((Unit obj) => obj == U))
		//		_DeahtListRef.Add (U);
		//}
		//public void RemoveFromBuildingList(IProcessor IP){
		//	_BuildingProcessRef.Remove (IP);
		//}

		public Map (//ref GameStats ThisGameStats,
					//ref List<IProcessor> VIProcess,
					//ref List<IProcessor> BuildingProcess,
					//ref List<IProcessor> MoveUnitProcess,
					//ref List<Unit> DeathList,
		   //         ref List<MoveUnit> MoveUnitMove,
		   //         ref List<VisibleUnit> drawList,
		            string mapName)
		{
			//_gameStatsRef = ThisGameStats;
			//_VIProcessRef = VIProcess;
			//_BuildingProcessRef = BuildingProcess;
			//_MoveUnitProcessRef = MoveUnitProcess;
			//_DeahtListRef = DeathList;
			//_MoveUnitMoveRef = MoveUnitMove;
			//_drawList = drawList;
			_grid = new Dictionary<Point2D, VisibleUnit> ();
			_allResources = new List<Resource> ();
			_mapLocation = Directory.GetCurrentDirectory () + "\\Resources\\RTS\\maps\\" + mapName;
			_mapLocation = Directory.GetCurrentDirectory()+ GameMain.MapsFolder + mapName;
			_allUnits = new List<Unit>();
		}

		public void GenerateLevel ()
		{
			//Make the wall's image
			Wall.SetImage (VisibleUnit.AddBitmap("wall.png","wallBitmap"));

			//Make the Resource's's image
			Resource.SetImage (VisibleUnit.AddBitmap ("Resource.png","resourceBitmap"));

			int NumOTeams = 0;
			StreamReader reader = new StreamReader (_mapLocation);
			try {
				string currentLine=reader.ReadLine();
				while(currentLine!=null){

					//create border
					if (currentLine.StartsWith ("d")) {
						string [] dimensions = currentLine.Split (new char [] { 'd', ' ' });
						int width = int.Parse (dimensions [1]);
						int height = int.Parse (dimensions [2]);

						for (int i = 0; i <= width; i++) {
							CreateWall (i, 0);
						}
						for (int i = 1; i <= height; i++) {
							CreateWall (0, i);
						}
						for (int i = 1; i <= width; i++) {
							CreateWall (i, height);
						}
						for (int i = 1; i <= (height - 1); i++) {
							CreateWall (width, i);
						}
					} else

					//fill/line
					if (currentLine.StartsWith ("f")) {
						string [] dimensions = currentLine.Split (new char [] { 'f', ' ' });
						int left = int.Parse (dimensions [1]);
						int right = int.Parse (dimensions [3]);
						int top = int.Parse (dimensions [2]);
						int bottom = int.Parse (dimensions [4]);
						//across
						for (int h = top; h <= bottom; h++) {
							//height
							for (int w = left; w <= right; w++) {
								CreateWall (w, h);
							}
						}
					} else

					//create wall
					if (currentLine.StartsWith ("w")) {
						string [] coordinates = currentLine.Split (new char [] { 'w', ' ' });
						CreateWall (float.Parse (coordinates [1]), float.Parse (coordinates [2]));
					} else

					//create team base
					if (currentLine.StartsWith ("t")) {
						string [] coordinates = currentLine.Split (new char [] { 't', ' ' });

						//home base
						Building HomeBase = CreateBuilding (ref GameMain.MasterGameStats._buildingHome, float.Parse (coordinates [1]), float.Parse (coordinates [2]));

						//Archery
						Building Archery = CreateBuilding (ref GameMain.MasterGameStats._buildingArchery, float.Parse (coordinates [3]), float.Parse (coordinates [4]));

						//Barraks
						Building Barraks = CreateBuilding (ref GameMain.MasterGameStats._buildingBarraks, float.Parse (coordinates [5]), float.Parse (coordinates [6]));

						Map thisMap = this;
						VirtualIntelligence teamIntelligence = new VirtualIntelligence (ref thisMap, HomeBase, Archery, Barraks, GameMain.MasterGameStats._teams.Pop(), _TeamColors[NumOTeams]);
						if (NumOTeams < _TeamColors.Length-1)
							NumOTeams++;
						else
							NumOTeams = 0;
						GameMain.BuildingsProcessList.Add (teamIntelligence);
						GameMain.BuildingsProcessList.Add (HomeBase);
						GameMain.BuildingsProcessList.Add (Archery);
						GameMain.BuildingsProcessList.Add (Barraks);
					} else

					//Create Resource
					if (currentLine.StartsWith ("r")) {
						string [] coordinates = currentLine.Split (new char [] { 'r', ' ' });
						CreateResource (float.Parse (coordinates [1]), float.Parse (coordinates [2]));
					}
					currentLine = reader.ReadLine ();
				}
			} finally {
				reader.Close ();
			}
		}

		private void CreateWall (float x, float y)
		{
			Point2D location = new Point2D ();
			location.X = x;
			location.Y = y;
			if (!_grid.ContainsKey(location)) {
				Wall NewWall = new Wall (location);
				_grid.Add (location, NewWall);
				GameMain.DrawList.Add (NewWall);
			}
		}
		private Building CreateBuilding (ref BuildingStats stats, float x, float y)
		{
			Point2D location = new Point2D ();
			location.X = x;
			location.Y = y;
			VirtualIntelligence masterAI = null;
			Building NewBuilding = new Building (ref stats,location, ref masterAI);
			GameMain.DrawList.Add (NewBuilding);
			_grid.Add (location, NewBuilding);
			//_allUnits.Add (NewBuilding);
			return NewBuilding;
		}
		private void CreateResource (float x, float y)
		{
			Point2D location = new Point2D ();
			location.X = x;
			location.Y = y;
			Resource NewResource = new Resource (location);
			GameMain.DrawList.Add (NewResource);
			_grid.Add (location, NewResource);
			_allResources.Add (NewResource);
		}

		public Resource FindResource (VirtualIntelligence TeamAI, Point2D FromPos)
		{
			Resource [] ResourceArray = _allResources.ToArray ();
			Resource current = ResourceArray[0];
			float currentDistance = getDistance (FromPos, current.Position);
			foreach (Resource R in ResourceArray) {
				float testDistance = getDistance (FromPos, R.Position);
				if (testDistance < currentDistance){
					current = R;
					currentDistance = testDistance;
				}
			}
			return current;
		}
		/// <summary>
		/// Finds path on the map. Note: Resource intensive
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="CurrentPos">Current position.</param>
		/// <param name="TargetsPos">Targets position.</param>
		public Stack<Point2D> FindPath (Point2D CurrentPos, Point2D TargetsPos, float distance = 1f)
		{
			//diagnostics
			Stopwatch pathFindTime = new Stopwatch ();
			pathFindTime.Start ();

			SortedList<float, Node> Nodes = new SortedList<float, Node> ();
			Dictionary<Point2D, Node> DNodes = new Dictionary<Point2D, Node> ();
			//OrderedDictionary ODNodes = new OrderedDictionary ();
			Dictionary<Point2D, Node> UsedNodes = new Dictionary<Point2D, Node> ();
			Node MyNode = new Node ();
			MyNode.location = CurrentPos;
			MyNode.from = CurrentPos;
			MyNode.distanceFrom = 0f;
			MyNode.distanceTo = getDistance (MyNode.location,TargetsPos);
			MyNode.totalDistance = MyNode.distanceFrom + MyNode.distanceTo;
			DNodes.Add (CurrentPos, MyNode);
			//ODNodes.Add (CurrentPos, MyNode);
			Stack<Point2D> theStack = new Stack<Point2D> ();
			Node N = DNodes[CurrentPos];
			bool unfound = true;
			while (unfound) {
				if (!(N.distanceTo <= distance) && !(N.location.X == TargetsPos.X && N.location.Y == TargetsPos.Y)) {
					Point2D SearchLocation = new Point2D ();
					SearchLocation.X = N.location.X + 1;
					SearchLocation.Y = N.location.Y;
					AddNode (SearchLocation, N.location, TargetsPos, ref DNodes, ref UsedNodes);
					SearchLocation.X = N.location.X - 1;
					SearchLocation.Y = N.location.Y;
					AddNode (SearchLocation, N.location, TargetsPos, ref DNodes, ref UsedNodes);
					SearchLocation.X = N.location.X;
					SearchLocation.Y = N.location.Y + 1;
					AddNode (SearchLocation, N.location, TargetsPos, ref DNodes, ref UsedNodes);
					SearchLocation.X = N.location.X;
					SearchLocation.Y = N.location.Y - 1;
					AddNode (SearchLocation, N.location, TargetsPos, ref DNodes, ref UsedNodes);
					UsedNodes.Add (N.location, N);
					DNodes.Remove (N.location);
					N = LowestDistanceNode(ref DNodes);
				} else {
					Point2D CurrentAddition = N.location;
					Node ParentNode = N;
					while (!CurrentAddition.Equals(CurrentPos)) {
						theStack.Push (CurrentAddition);
						ParentNode = UsedNodes[ParentNode.from];
						CurrentAddition = ParentNode.location;
					}
					pathFindTime.Stop ();
					TimeSpan theMain = GameMain.LongestPathCreationTime;
					if (pathFindTime.Elapsed > GameMain.LongestPathCreationTime)
						GameMain.LongestPathCreationTime = pathFindTime.Elapsed;
					return theStack;
					//unfound = false;
				}
			}
			pathFindTime.Stop ();
			if (pathFindTime.Elapsed > GameMain.LongestPathCreationTime)
				GameMain.LongestPathCreationTime = pathFindTime.Elapsed;
			return theStack;
		}

		private void AddNode (Point2D Search, Point2D From, Point2D target, ref Dictionary<Point2D,Node> DNodes, ref Dictionary<Point2D, Node> UsedNodes)
		{
			Node NU;
			VisibleUnit VU;
			if (!DNodes.TryGetValue (Search, out NU) && ( (!_grid.TryGetValue (Search, out VU) &&  !UsedNodes.TryGetValue (Search, out NU)) || Search.Equals (target) ) ) {
				Node newNode = new Node ();
				newNode.from = From;
				newNode.location = Search;
				newNode.distanceFrom = DNodes [From].distanceFrom + 1f;
				newNode.distanceTo = getDistance (newNode.location, target);
				newNode.totalDistance = newNode.distanceFrom + newNode.distanceTo;
				DNodes.Add (Search, newNode);
			}
		}
		private Node LowestDistanceNode (ref Dictionary<Point2D,Node> list)
		{
			Node closestNode = list.First().Value;
			foreach (Node N in list.Values) {
				float calcDistance = N.distanceFrom + N.distanceTo;
				if (calcDistance < closestNode.totalDistance) {
					closestNode.totalDistance = calcDistance;
					closestNode = N;
				}
			}
			return closestNode;
		}
		public static float getDistance (Point2D start, Point2D end)
		{
			float x =(float)Math.Pow (Math.Abs (start.X - end.X),2);
			float y = (float)Math.Pow (Math.Abs (start.Y - end.Y), 2);
			return (float)Math.Sqrt (x + y);
		}


		public Unit findTarget (Point2D Position, VirtualIntelligence SearchersTeam)
		{
			Unit NearestTarget = null;
			float distanceToTarget = 999999999f;
			foreach (Unit U in _allUnits) {
				if (U.Master != SearchersTeam){
					float dist = getDistance (Position, U.Position);
					if(dist < distanceToTarget) {
						NearestTarget = U;
						distanceToTarget = dist;
					}
				}
			}
			return NearestTarget;
		}

	}


	public struct Node
	{
		public Point2D location;
		public Point2D from;
		public float distanceFrom;
		public float distanceTo;
		public float totalDistance;
	}
}

