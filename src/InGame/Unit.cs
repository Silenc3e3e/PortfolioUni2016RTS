using System;
using SwinGameSDK;
using System.Diagnostics;
using System.Collections.Generic;

namespace RTS
{
	//int is diaog only
	public delegate void DeathListener (ref int count);

	public abstract class Unit : VisibleUnit
	{
		protected VirtualIntelligence _masterRef;
		protected bool idle;
		private int _health;
		private List<Aggressive> _deathListeningUnits;
		private bool _doomed;

		//death listener
		private event DeathListener Dead;
		public void addToListeners(DeathListener listener, Aggressive A){
			Dead += listener;
			if (!_deathListeningUnits.Exists ((Aggressive obj) => obj == A)){
				_deathListeningUnits.Add (A);
			}
		}
		public void RemoveFromListeners (DeathListener listener, Aggressive A){
			Dead -= listener;
			_deathListeningUnits.Remove (A);
		}


		public Unit (Bitmap image, Point2D position, ref VirtualIntelligence VIMaster, int Health) : base(ref image,position)
		{
			_deathListeningUnits = new List<Aggressive> ();
			idle = false;
			_doomed = false;
			_health = Health;
			if(VIMaster !=null){
				Master = VIMaster;
			}
		}

		//public void AddUnitToList(){
		//	if(_masterRef != null)
		//		Master.TheMap.AddToUnitList (this);
		//}

		public virtual void death ()
		{
			Stopwatch drawRemove = new Stopwatch ();
			drawRemove.Start ();
			GameMain.DrawList.Remove (this);
			drawRemove.Stop ();
			if(drawRemove.Elapsed> GameMain.LongestDeathItemSpan){
				GameMain.LongestDeathItemSpan = drawRemove.Elapsed;
				GameMain.LongestDeathItemName = "DrawRemove";
			}
			drawRemove.Reset ();
			drawRemove.Start ();
			_masterRef.TheMap.RemoveFromUnitList (this);
			drawRemove.Stop ();
			if (drawRemove.Elapsed > GameMain.LongestDeathItemSpan) {
				GameMain.LongestDeathItemSpan = drawRemove.Elapsed;
				GameMain.LongestDeathItemName = "UnitListRemove";
			}
			drawRemove.Reset ();
			drawRemove.Start ();
			int counter = 0;
			//if (Dead != null) {
			//	Dead.Invoke (ref counter);
			//}
			GameMain.NumTeamA--;
			//List<Aggressive> removal = new List<Aggressive>();
			Aggressive[] removal = _deathListeningUnits.ToArray();
			List<Point2D> locations = new List<Point2D> ();
			foreach(Aggressive A in removal){
				foreach (Point2D p in locations)
					if (p.Equals (A.Position))
						Console.WriteLine ("SECOND!");
				locations.Add (A.Position);
				A.ChargeNewTarget ();
				counter++;
			}
			_deathListeningUnits.Clear ();
			if (counter > GameMain.NumOfInvokers)
				GameMain.NumOfInvokers = counter;
			drawRemove.Stop ();
			if (drawRemove.Elapsed > GameMain.LongestDeathItemSpan) {
				GameMain.LongestDeathItemSpan = drawRemove.Elapsed;
				GameMain.LongestDeathItemName = "Dead Invoke";
			}
		}

		public void takeDamage(Unit Attacker, int Damage){
			_health -= Damage;
			DamageTaken (Attacker);
			if(_health <=0 && !_doomed){
				_doomed = true;
				GameMain.DeathList.Add (this);
			}else if(_doomed){
				Console.Write ("Not adding twice");
			}
		}
		public virtual void DamageTaken (Unit Attacker){}

		public VirtualIntelligence Master {
			get {
				return _masterRef;
			}
			set {
				_masterRef = value;
				teamColor = value.TeamColor;
				value.TheMap.AddToUnitList (this);
			}
		}
	}
}

