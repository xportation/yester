using NUnit.Framework;
using System;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class TimelineTest
	{
		[Test()]
		public void TestShouldBeAbleToAddAVideoInADate()
		{
			Timeline timeline = new Timeline();
			timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");

			Assert.IsTrue(timeline.HasVideoAt(new DateTime(2012, 1, 1)));
		}

		[Test()]
		public void TestShouldBeAbleToAddMoreThanAVideoInADate()
		{
			Timeline timeline = new Timeline();
			timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");
			timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video2.mpeg");
			
			Assert.AreEqual(2, timeline.GetVideoCountAt(new DateTime(2012, 1, 1)));
		}
	}
}

