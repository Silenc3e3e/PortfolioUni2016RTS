using System;
using SwinGameSDK;

namespace RTS
{
	public class NumberReturn
	{
		public NumberReturn ()
		{
		}
		public float GetValue(float i, float x, float y, string FieldName){
			float ToReturn = i;
			SwinGame.DrawText (FieldName + ": "+i, Color.White, x - 96f, y);
			Button Increase = new Button (Color.White, x + 160, y, 64f, 32f, "Increase", Color.White);
			Button Decrease = new Button (Color.White, x - 192, y, 64f, 32f, "Decrease", Color.White);
			Increase.Draw ();
			Decrease.Draw ();
			if (SwinGame.MouseClicked (MouseButton.LeftButton)) {
				if (Increase.clicked ()) {
					return i + 0.25f;
				} else if (Decrease.clicked () && i>.025f) {
					return i - 0.25f;
				}
			}

			return i;
		}
		public int GetValue (int i, float x, float y, string FieldName){
			int ToReturn = i;
			SwinGame.DrawText (FieldName + ": " + i, Color.White, x - 96f, y);
			Button Increase = new Button (Color.White, x + 160, y, 64f, 32f, "Increase", Color.White);
			Button Decrease = new Button (Color.White, x - 192, y, 64f, 32f, "Decrease", Color.White);
			Increase.Draw ();
			Decrease.Draw ();
			if (SwinGame.MouseClicked (MouseButton.LeftButton)) {
				if (Increase.clicked ()) {
					return i+1;
				} else if (Decrease.clicked () && i > 1) {
					return i-1;
				}
			}
			return i;
		}
	}
}

