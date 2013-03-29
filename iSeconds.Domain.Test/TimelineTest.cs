using NUnit.Framework;
using System;

namespace iSeconds.Domain.Test
{
    [TestFixture()]
    public class TimelineTest
    {
        ISecondsDB repository = null;

        [SetUp()]
        public void Init()
        {
            repository = new ISecondsDB("testbase.db3");
            repository.DeleteAll<User>();
            repository.DeleteAll<Timeline>();
            repository.DeleteAll<DayInfo>();
            repository.DeleteAll<MediaInfo>();

            //user = new User("test", repository);
            //repository.SaveItem(user);
        }

        [Test()]
        public void TestShouldBeAbleToAddAVideoInADate()
        {
            Timeline timeline = new Timeline("my life", 1, repository);
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");

            Assert.That(timeline.GetVideosAt(new DateTime(2012, 1, 1)).Count, Is.EqualTo(1));
        }

        [Test()]
        public void TestShouldBeAbleToAddMoreThanAVideoInADate()
        {
            // o usuario a principio pode adicionar mais de um video no dia.
            // na hora de "commitar" para o "trunk" do timeline ele deve escolher apenas um.
            Timeline timeline = new Timeline("my life", 1, repository);
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video.mpeg");
            timeline.AddVideoAt(new DateTime(2012, 1, 1), "sdcard/iseconds/video2.mpeg");

            Assert.That(timeline.GetVideosAt(new DateTime(2012, 1, 1)).Count, Is.EqualTo(2));
        }
    }
}

