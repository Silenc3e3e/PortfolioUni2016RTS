using System;
using System.Collections.Generic;
using SwinGameSDK;

namespace RTS
{
	public class GetNumbers
	{
		public GetNumbers ()
		{
		}

		public static object[] GetTheNumbers(string SectionName, string[] Names, object[] NTList){
			int count;
			if (Names.Length < NTList.Length)
				count = Names.Length;
			else
				count = NTList.Length;

			//object [] Values = new object [count];
			//for (int i = 0; i < count; i++) {
			//	if(NTList[i] is float){
			//		Values [i] = NTList[i];
			//	}else if (NTList [i] is int) {
			//		Values [i] = NTList [i];
			//	}
			//}

			bool Nexted = false;
			int screenWidthHalf = SwinGame.WindowWidth(GameMain.WindowName) / 2;
			Button Next = new Button (Color.White, screenWidthHalf-100f, count*32f+128f, 200f, 64f, "Next", Color.White);
			while (!Nexted) {
				SwinGame.ProcessEvents ();
				for (int i = 0; i < count; i++) {
					NumberReturn NR = new NumberReturn ();
					if (NTList [i] is float) {
						NTList [i] = NR.GetValue ((float)NTList [i], screenWidthHalf, 128f + (float)i * 32f, Names [i]);
					} else if (NTList[i] is int)
						NTList [i] = NR.GetValue ((int)NTList [i], screenWidthHalf, 128f + (float)i * 32f, Names [i]);
				}
				SwinGame.DrawText (SectionName, Color.White, screenWidthHalf - 64f, 32f);
				Next.Draw ();
				if (SwinGame.WindowCloseRequested ()) {
					Nexted = true;
					break;
				}
				if (SwinGame.MouseClicked (MouseButton.LeftButton))
					Nexted = Next.clicked ();

				SwinGame.RefreshScreen (GameMain.ScreenRefreshRate);
				SwinGame.ClearScreen (Color.Black);
			}
			return NTList;
		}
	}
	public enum NumberType{
		integer, floating
	}
}

