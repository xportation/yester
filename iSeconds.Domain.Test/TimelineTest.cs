using NUnit.Framework;
using System;

namespace iSeconds.Domain.Test
{
    [TestFixture()]
    public class TimelineTest
    {
        ISecondsDB repository = null;
        Timeline timeline = null;

        [SetUp()]
        public void Init()
        {
            repository = new ISecondsDB("testbase.db3");
            repository.DeleteAll<User>();
            repository.DeleteAll<Timeline>();
            repository.DeleteAll<DayInfo>();
            repository.DeleteAll<MediaInfo>();

            timeline = new Timeline("my life", 1);
            timeline.SetRepository(repository);

            //user = new User("test", repository);
            //repository.SaveItem(user);
        }

        [Test()]
        public void TestShouldBeAbleToAddAVideoInADate()
        {
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");

            Assert.That(timeline.GetVideosAt(new DateTime(2012, 1, 1)).Count, Is.EqualTo(1));
        }

        [Test()]
        public void TestShouldBeAbleToAddMoreThanAVideoInADate()
        {
            // o usuario a principio pode adicionar mais de um video no dia.
            // na hora de "commitar" para o "trunk" do timeline ele deve escolher apenas um.
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video2.mpeg");

            Assert.That(timeline.GetVideosAt(new DateTime(2012, 1, 1)).Count, Is.EqualTo(2));
        }

		[Test()]
		public void TestRetrieveVideosInARange()
		{
			DateTime date1 = new DateTime (2012, 1, 1);
			DateTime date2 = new DateTime (2012, 1, 2);

			timeline.AddVideoAt(date1, "sdcard/iseconds/video.mpeg");
			timeline.AddVideoAt(date2, "sdcard/iseconds/video2.mpeg");

			//Assert.That(timeline.GetVideosFromRange (date1, date2).Count, Is.EqualTo(2));
			Assert.That(timeline.GetVideosFromRange (new DateTime(2011,1,1), new DateTime(2013,1,1)).Count, Is.EqualTo(2));
		}

		[Test()]
		public void TestRetrieveVideosInARangeShouldIncludeDaysSearched()
		{
			DateTime date1 = new DateTime (2012, 1, 1);
			DateTime date2 = new DateTime (2012, 1, 2);

			timeline.AddVideoAt(date1, "sdcard/iseconds/video.mpeg");
			timeline.AddVideoAt(date2, "sdcard/iseconds/video2.mpeg");

			Assert.That(timeline.GetVideosFromRange (date1, date2).Count, Is.EqualTo(2));
			//Assert.That(timeline.GetVideosFromRange (new DateTime(2011,1,1), new DateTime(2013,1,1)).Count, Is.EqualTo(2));
		}




    }
}

