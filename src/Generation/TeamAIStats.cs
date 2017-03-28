using System;
using SwinGameSDK;

namespace RTS
{
	public struct TeamAIStats
	{
		//intelligence
		public int workerStartBoost;
		public BiasNames AggressiveBias;

		//worker to fighter ratio
		public int workerToFighterRatio;
		public int fighterToWorkerRatio;

		//archer to swordsman ratio
		public int SwordsmanToArchersRatio;
		public int ArchersToSwordsmanRatio;
	}

	public enum BiasNames{
		Swordsman, Archers
	}
}