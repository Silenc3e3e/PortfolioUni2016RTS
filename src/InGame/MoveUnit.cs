using System;
using System.Collections.Generic;
using SwinGameSDK;
using System.Diagnostics;

namespace RTS
{
	public delegate void MoveListener (Point2D NewLocation);

	public abstract class MoveUnit : Unit, IProcessor
	{
		protected Stack<Point2D> _currentPath;
		private Point2D _currentMovePoint;
		private IMoveUnitStats _stats;
		private float _currentTimer;
		private float xChange;
		private float yChange;
		private float _increaseRate;
		private event MoveListener _moved;

		public MoveUnit (Point2D position, ref VirtualIntelligence Master, ref IMoveUnitStats Stats) : base(Stats.Image,position,ref Master, Stats.MaxHealth)
		{
			_currentTimer = 1f;
			_currentMovePoint = position;
			_increaseRate = Stats.Speed/GameMain.CalculationsPerSecond;
			_stats = Stats;
			GameMain.DrawList.Add (this);
			GameMain.MoveUnitList.Add (this);
			GameMain.MoveUnitProcessList.Add (this);
			xChange = 0f;
			yChange = 0f;
		}

		public void Move ()
		{
			Stopwatch Move = new Stopwatch ();
			Stopwatch midPoint = new Stopwatch ();
			Stopwatch whileLoop = new Stopwatch ();
			Stopwatch invoke = new Stopwatch ();
			Stopwatch SmoothX = new Stopwatch ();
			Stopwatch SmoothY = new Stopwatch ();
			//midPoint.Start ();
			Move.Start ();
			whileLoop.Start ();

			_currentTimer += _increaseRate;
			//if ((Position.X != _currentMovePoint.X && Position.Y != _currentMovePoint.Y) || _currentPath.Count >0) { 
				while (_currentTimer >= 1f) {
				//next move point
					_currentTimer = _currentTimer - 1f;
					Position = _currentMovePoint;
					if (_currentPath != null && _currentPath.Count > 0) {
						_currentMovePoint = _currentPath.Pop ();
					} else {
						idle = true;
					}
					invoke.Start ();
					if (_moved != null) {
						_moved.Invoke (Position);
					}
					invoke.Stop ();
					midPoint.Start ();
					xChange = (Math.Abs (_currentMovePoint.X - Position.X));
					yChange = (Math.Abs (_currentMovePoint.Y - Position.Y));
					midPoint.Stop ();
				}
				whileLoop.Stop ();

			//Continue moving
			Point2D newPoint = Position;
			if (Position.X != _currentMovePoint.X) {
				SmoothX.Start ();

				//float TimerInverse =  _currentTimer;
				if (_currentMovePoint.X < Position.X)
					newPoint.X = Position.X - xChange * _currentTimer;
				else if (_currentMovePoint.X > Position.X)
					newPoint.X = Position.X + xChange * _currentTimer;
				else
					newPoint.X = Position.X;
				SmoothX.Stop ();
			}
			if (Position.Y != _currentMovePoint.Y) { 
				SmoothY.Start ();
				if (_currentMovePoint.Y < Position.Y)
					newPoint.Y = Position.Y - yChange * _currentTimer;
				else if (_currentMovePoint.Y > Position.Y)
					newPoint.Y = Position.Y + yChange * _currentTimer;
				else
					newPoint.Y = Position.Y;
				SmoothY.Stop ();
			}
			ScreenPosition = newPoint;
			//}

			Move.Stop ();
			if (Move.Elapsed > GameMain.LongestMove) {
				GameMain.LongestMove = Move.Elapsed;
				Console.WriteLine ("New Move length = " + Move.Elapsed + "   Mid point: "+midPoint.Elapsed);
				Console.WriteLine ("While loop = " + whileLoop.Elapsed);
				Console.WriteLine ("invoke = " + invoke.Elapsed);
				Console.WriteLine ("smoothx = " + SmoothX.Elapsed);
				Console.WriteLine ("smoothy = " + SmoothY.Elapsed);
			}
		}
		public void addMoveListener(MoveListener ML){
			_moved += ML;
		}
		public void RemoveMoveListener(MoveListener ML){
			_moved-= ML;
		}

		public void NewPath (Point2D NewLocation)
		{
			Stack<Point2D> ThePath = _masterRef.TheMap.FindPath (Position, NewLocation, _stats.Range);
			_currentPath = ThePath;
			if (_currentPath.Count > 0) {
				_currentMovePoint = _currentPath.Pop ();
				xChange = (Math.Abs (_currentMovePoint.X - Position.X));
				yChange = (Math.Abs (_currentMovePoint.Y - Position.Y));
			}
		}

		public virtual void Process ()
		{

		}

		public override void death ()
		{
			_masterRef.ReduceRegisterLessUnits (_stats.MoveUnitType);
			GameMain.MoveUnitList.Remove (this);
			List<IProcessor> store = GameMain.MoveUnitProcessList;
			GameMain.MoveUnitProcessList.Remove (this);
			store = GameMain.MoveUnitProcessList;
			base.death ();
		}
	}
}
