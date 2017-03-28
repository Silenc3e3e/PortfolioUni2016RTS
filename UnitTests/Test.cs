using NUnit.Framework;
using System;
using RTS;
using SwinGameSDK;

namespace UnitTests
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestCase ()
		{
			Point2D start = new Point2D ();
			Point2D end = new Point2D ();
			start.X = 2;
			start.Y = 2;
			end.X = 4;
			end.Y = 4;
			//Assert.AreEqual (Map.getDistance(start,end), 2.82);
		}
	}
}

