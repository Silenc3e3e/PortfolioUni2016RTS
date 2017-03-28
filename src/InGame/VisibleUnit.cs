using System;
using SwinGameSDK;
using System.IO;

namespace RTS
{
	public abstract class VisibleUnit
	{
		//public const string DirectoryLocation = Directory.GetCurrentDirectory ();
		public const string imageDirectory = "\\Resources\\RTS\\Images\\";

		private Sprite _sprite;
		private Point2D _position;
		private Point2D _screenPosition;
		private Point2D _teamColorPosition;
		protected Bitmap _image;
		private Color _drawTeam;

		//Attack visualization
		private LineSegment AttackLine;
		private int NumAttackFrames;

		public Color teamColor{
			set{
				_drawTeam = value;
				_drawTeam= SwinGame.RGBAColor(_drawTeam.R,_drawTeam.G,_drawTeam.B,(byte)100);
			}
		}

		public static Bitmap AddBitmap(string imageName, string BitmapName){
			string NewBitmapPath = Directory.GetCurrentDirectory () + imageDirectory + imageName;
			//NewBitmapPath = DirectoryLocation + imageDirectory + imageName;
			SwinGame.LoadBitmapNamed (BitmapName, NewBitmapPath);
			return SwinGame.BitmapNamed (BitmapName);
		}

		public VisibleUnit (ref Bitmap image, Point2D position)
		{
			_image = image;
			_sprite = SwinGame.CreateSprite (image);
			//_screenPosition = new Point2D ();
			Position = position;
			_sprite.X = _screenPosition.X;
			_sprite.Y = _screenPosition.Y;
			NumAttackFrames = 0;
		}

		public void Draw ()
		{
			SwinGame.DrawSprite (_sprite/*, Position*/);
			//_image.Dispose ();
			//_image.Draw (_screenPosition.X, _screenPosition.Y);
			if(_drawTeam!=null){
				SwinGame.FillCircle (_drawTeam,_teamColorPosition,16);
			}
			if(NumAttackFrames>0){
				NumAttackFrames--;
				SwinGame.DrawLine (_drawTeam, AttackLine);
			}
		}

		protected void DrawAttackLine(int NumFrames, Point2D TargetTrueScreenPosition){
			NumAttackFrames = NumFrames;
			LineSegment LS = new LineSegment ();
			Point2D start = new Point2D ();
			Point2D end = new Point2D ();
			start.X = _screenPosition.X+16;
			start.Y = _screenPosition.Y + 15;
			end.X = TargetTrueScreenPosition.X+ 16;
			end.Y = TargetTrueScreenPosition.Y + 17;
			LS.StartPoint = start;
			LS.EndPoint = end;
			AttackLine = LS;
		}

		public Point2D Position{
			get{
				return _position;
			}
			set {
				_position = value;
				ScreenPosition = value;
			}
		}
		protected Point2D ScreenPosition{
			set{
				_screenPosition.X = value.X * 32;
				_screenPosition.Y = value.Y * 32;
				_teamColorPosition.X = _screenPosition.X + 16;
				_teamColorPosition.Y = _screenPosition.Y + 16;
				_sprite.Position = _screenPosition;
			}
		}
		public Point2D TrueScreenPositionGet{
			get{
				return _screenPosition;
			}
		}
	}
}