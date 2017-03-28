using System;
using SwinGameSDK;

namespace RTS
{
	public class Wall : VisibleUnit
	{
		private static Bitmap _wallImage = SwinGame.CreateBitmap(32, 32);

		public static void SetImage (Bitmap image)
		{
			_wallImage = image;
		}

		public Wall (Point2D position) : base (ref _wallImage, position)
		{
		}
	}
}