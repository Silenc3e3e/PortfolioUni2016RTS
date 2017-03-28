using System;
using System.Collections.Generic;
using System.IO;
using SwinGameSDK;
using System.Diagnostics;

namespace RTS
{
	public class GameMain
	{
		public static List<VirtualIntelligence> VIProcessList;
		public static List<IProcessor> BuildingsProcessList;
		public static List<IProcessor> MoveUnitProcessList;
		public static List<Unit> DeathList;
		public static List<MoveUnit> MoveUnitList;
		public static List<VisibleUnit> DrawList;
		public static GameStats MasterGameStats;
		public static int ScreenRefreshRate;
		public static float CalculationsPerSecond;
		public const string WindowName = "The Great RTS";
		public static int VILeft;
		private static bool _Restart = true;

		public const string MapsFolder = "\\Resources\\RTS\\maps\\";
		private const string MapName = "\\map02.txt";

		//diagnostics
		public static TimeSpan LongestDeathItemSpan;
		public static string LongestDeathItemName;
		public static int NumOfInvokers;
		public static int NumTeamA;
		public static TimeSpan LongestMove;
		public static TimeSpan LongestPathCreationTime;

        public static void Main()
		{
			//Open the window
			StreamReader reader = new StreamReader (Directory.GetCurrentDirectory () + MapsFolder + MapName);
			int Width = 0;
			int Height = 0;
			try {
				string currentLine = reader.ReadLine ();
				while (currentLine != null) {
					if (currentLine.StartsWith ("d")) {
						string [] dimensions = currentLine.Split (new char [] { 'd', ' ' });
						Width = (int.Parse (dimensions [1]) + 1) * 32;
						Height = (int.Parse (dimensions [2]) + 1) * 32;
						break;
					} else
						currentLine = reader.ReadLine ();
				}
			} finally {
				reader.Close ();
			}
			SwinGame.OpenGraphicsWindow (WindowName, Width, Height);

			//EVERYTHING goes in here. So the game can be restarted
			while (_Restart && !SwinGame.WindowCloseRequested()) {
				_Restart = false;
				VILeft = 0;
				ScreenRefreshRate = 60;
				CalculationsPerSecond = (float)ScreenRefreshRate;



				//Run the window loop
				//First screen
				MasterGameStats = new GameStats (MapName);


				Button Next = new Button (Color.White, 300f, 300f, 200f, 100f, "Begin the game!", Color.White);

				bool clicked = false;
				while (!clicked) {
					if (SwinGame.WindowCloseRequested ())
						break;

					SwinGame.ProcessEvents ();
					if (SwinGame.MouseClicked (MouseButton.LeftButton))
						clicked = Next.clicked ();

					Next.Draw ();
					SwinGame.RefreshScreen (ScreenRefreshRate);
					SwinGame.ClearScreen (Color.Black);
				}






				//Game screen
				VIProcessList = new List<VirtualIntelligence> ();
				BuildingsProcessList = new List<IProcessor> ();
				MoveUnitProcessList = new List<IProcessor> ();
				DeathList = new List<Unit> ();
				MoveUnitList = new List<MoveUnit> ();
				DrawList = new List<VisibleUnit> ();

				Map gameMap = new Map (MapName);
				gameMap.GenerateLevel ();

				int VUdrawSize = DrawList.Count;

				//diagnostics
				TimeSpan longestDuration = TimeSpan.Zero;
				string longestItem = "";
				Stopwatch test = new Stopwatch ();
				test.Start ();
				LongestDeathItemSpan = TimeSpan.Zero;
				LongestDeathItemName = "";
				NumOfInvokers = 0;
				NumTeamA = 0;
				LongestMove = TimeSpan.Zero;
				LongestPathCreationTime = TimeSpan.Zero;

				while (false == SwinGame.WindowCloseRequested () && VILeft >= 2) {
					int THENUMB = VILeft;
					test.Stop ();
					Console.WriteLine ("PostDraw Process: " + test.Elapsed);
					Console.WriteLine ("Longest Duration: " + longestDuration + " Item: " + longestItem);
					Console.WriteLine ("Long   DeathItem: " + LongestDeathItemSpan + " Item: " + LongestDeathItemName + " Number Invokers " + NumOfInvokers);
					Console.WriteLine ("Number of Units : " + NumTeamA);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "Post draw";
					}
					test.Reset ();
					test.Start ();
					//AI process
					foreach (IProcessor IP in VIProcessList) {
						IP.Process ();
					}
					test.Stop ();
					Console.WriteLine ("VI Process:       " + test.Elapsed);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "VI Process";
					}
					test.Reset ();
					test.Start ();
					//Building process
					foreach (IProcessor IP in BuildingsProcessList) {
						IP.Process ();
					}
					test.Stop ();
					Console.WriteLine ("Building Process: " + test.Elapsed);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "Building Process";
					}
					test.Reset ();
					test.Start ();
					//Move units Process
					foreach (IProcessor IP in MoveUnitProcessList) {
						IP.Process ();
					}
					test.Stop ();
					Console.WriteLine ("MU Process:       " + test.Elapsed);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "MU Process";
					}
					test.Reset ();
					test.Start ();
					//Death of units
					if (DeathList.Count > 0) {
						Stopwatch DeathWatch = new Stopwatch ();
						DeathWatch.Start ();
						foreach (Unit U in DeathList) {
							U.death ();
						}
						DeathWatch.Stop ();
						Console.WriteLine ("Death Foreach:    " + test.Elapsed + "----");
						if (DeathWatch.Elapsed > longestDuration) {
							longestDuration = test.Elapsed;
							longestItem = "Death Foreach";
						}
						DeathWatch.Reset ();
						DeathWatch.Start ();
						DeathList = new List<Unit> ();
						DeathWatch.Stop ();
						Console.WriteLine ("Death NewList:    " + test.Elapsed + "----");
						if (DeathWatch.Elapsed > longestDuration) {
							longestDuration = test.Elapsed;
							longestItem = "Death NewList";
						}
					}
					test.Stop ();
					Console.WriteLine ("Death Process:    " + test.Elapsed);
					//if (test.Elapsed > longestDuration){
					//	longestDuration = test.Elapsed;
					//	longestItem = "Death Process";
					//}
					test.Reset ();
					test.Start ();
					//All units move
					foreach (MoveUnit U in MoveUnitList) {
						U.Move ();
					}
					test.Stop ();
					Console.WriteLine ("MU Move:          " + test.Elapsed);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "MU Move";
					}
					test.Reset ();
					test.Start ();
					//Draw everything
					foreach (VisibleUnit vu in DrawList) {
						vu.Draw ();
					}
					test.Stop ();
					Console.WriteLine ("Draw Process:     " + test.Elapsed);
					if (test.Elapsed > longestDuration) {
						longestDuration = test.Elapsed;
						longestItem = "Draw Process";
					}
					test.Reset ();
					test.Start ();
					SwinGame.DrawFramerate (0, 0);
					SwinGame.RefreshScreen (ScreenRefreshRate);
					SwinGame.ClearScreen (Color.White);
					SwinGame.ProcessEvents ();
				}

				//End sequence
				if (VILeft == 1 && false == SwinGame.WindowCloseRequested ()) {
					VirtualIntelligence [] Winner;
					string teamnumber = "I dunno";
					Color teamColor = Color.White;
					if (VIProcessList.Count >= 1) {
						Winner = VIProcessList.ToArray ();
					}
						
					//VirtualIntelligence VIWinner = Winner [0];
					Button Quit = new Button (Color.Red, SwinGame.WindowWidth (GameMain.WindowName) / 2f - 64f, 96f, 64f, 32f, "Quit", Color.Black);
					Button Restart = new Button (Color.LightGreen,SwinGame.WindowWidth (GameMain.WindowName) / 2f - 64f,162f,64f,32f,"Restart",Color.Black);
					while (false == SwinGame.WindowCloseRequested ()) {
						SwinGame.DrawRectangle (teamColor, SwinGame.WindowWidth (GameMain.WindowName) / 2f - 64f, 32f, 96f, 32f);
						SwinGame.DrawText ("TEAM " + teamnumber + " WINS!", Color.White, SwinGame.WindowWidth (GameMain.WindowName) / 2f - 64f, 64f);
						Quit.Draw ();
						Restart.Draw ();
						if (SwinGame.MouseClicked (MouseButton.LeftButton)) {
							if (Quit.clicked ())
								break;
							else if (Restart.clicked ()) {
								_Restart = true;
								break;
							}
						}
						SwinGame.RefreshScreen (ScreenRefreshRate);
						SwinGame.ClearScreen (Color.White);
						SwinGame.ProcessEvents ();
					}
				}
			}
		}
    }
}