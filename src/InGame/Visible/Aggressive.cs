using System;
using SwinGameSDK;

namespace RTS
{
	public class Aggressive : MoveUnit
	{
		private AggressiveMoveUnitStats _unitStats;
		private Unit _target;
		private float _timer;
		private DeathListener _listener;

		public Aggressive (Point2D position, ref VirtualIntelligence Master, ref IMoveUnitStats AggressiveStats) : base(position, ref Master, ref AggressiveStats)
		{
			_listener = new DeathListener (diaog);
			_unitStats = (AggressiveMoveUnitStats)AggressiveStats;
		}

		public override void Process ()
		{
			if (_timer > 0f)
				_timer -= 1 / GameMain.CalculationsPerSecond;
			if (idle) {
				if (_target == null) {
					ChargeNewTarget ();
				} else {
					float distance = Map.getDistance (_target.Position, Position);
					if (distance <= _unitStats.range && _timer <= 0f) {
						_timer = _unitStats.reloadTime;
						_target.takeDamage (this, _unitStats.damage);
						SwinGame.DrawLine (_masterRef.TeamColor, Position.X, Position.Y-1f, _target.Position.X, _target.Position.Y+1f);
						//Attack line
						DrawAttackLine (5, _target.TrueScreenPositionGet);
					}
				}

			}
			base.Process ();
		}

		public override void DamageTaken (Unit Attacker)
		{
			ChargeTarget (Attacker, "DamageTaken");
		}

		public void ChargeNewTarget(){
			ChargeTarget(_masterRef.TheMap.findTarget (Position, _masterRef), "ChargeNewTarget");
		}
		public void diaog(ref int count){
			_target.RemoveFromListeners (_listener, this);
			ChargeNewTarget ();
			count++;
		}
		private void ChargeTarget (Unit NewTarget, string WhereFrom)
		{	
			if (NewTarget != _target) {
				if (_target != null){
					_target.RemoveFromListeners (_listener, this);
					if(_target is MoveUnit)
						((MoveUnit)_target).RemoveMoveListener(NewPath);
				}
				if (NewTarget != null) {
					_target = NewTarget;
					NewPath (_target.Position);
					idle = false;
					_target.addToListeners (_listener, this);
					Console.WriteLine (WhereFrom);
					if (_target is MoveUnit) {
						((MoveUnit)_target).addMoveListener (NewPath);
					}
				}
			}
		}

		public override void death ()
		{
			_target.RemoveFromListeners (_listener, this);
			if (_target is MoveUnit)
				((MoveUnit)_target).RemoveMoveListener (NewPath);
			base.death ();
		}
	}
}

