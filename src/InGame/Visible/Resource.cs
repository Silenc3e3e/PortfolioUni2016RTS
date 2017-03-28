using System;
using SwinGameSDK;

namespace RTS
{
	public class Resource : VisibleUnit
	{
		private static Bitmap _resourceImage = SwinGame.CreateBitmap (32, 32);

		public static void SetImage (Bitmap image)
		{
			_resourceImage = image;
		}

		public Resource (Point2D position) : base(ref _resourceImage, position)
		{
		}
	}
}

