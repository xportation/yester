using NUnit.Framework;
using System;
using iSeconds.Domain;
using System.Reflection;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class UserTest
	{
        ISecondsDB repository = null;
        User user = null;

        [SetUp()]
        public void Init()
        {
            repository = new ISecondsDB("testbase.db3");
            repository.DeleteAll<User>();
            user = new User("test", repository);
            repository.SaveItem(user);
        }

		[Test()]
		public void TestUserShouldHaveTimelines()
		{
         user.CreateTimeline("my life", "timeline_description");
			Assert.AreEqual(1, user.GetTimelineCount());

         user.CreateTimeline("my daughter's life", "timeline_description");
			Assert.AreEqual(2, user.GetTimelineCount());
		}

		[Test]
		public void TestDeleteTimeline()
		{
			Timeline timelineCreated= user.CreateTimeline("my life", "timeline_description");
			Assert.AreEqual(1, user.GetTimelineCount());

			user.DeleteTimeline(timelineCreated);
			Assert.AreEqual(0, user.GetTimelineCount());
		}

		[Test]
		public void TestOnlyDeleteTimelineWithSameUserId()
		{
			var user2 = new User("test2", repository);
			repository.SaveItem(user2);

			Timeline timelineUser1 = user.CreateTimeline("1", "1");
			Assert.AreEqual(1, user.GetTimelineCount());

			Timeline timelineUser2 = user2.CreateTimeline("2", "2");
			Assert.AreEqual(1, user2.GetTimelineCount());

			user.DeleteTimeline(timelineUser2);
			user2.DeleteTimeline(timelineUser1);

			Assert.AreEqual(1, user.GetTimelineCount());
			Assert.AreEqual(1, user2.GetTimelineCount());

			user.DeleteTimeline(timelineUser1);
			user2.DeleteTimeline(timelineUser2);

			Assert.AreEqual(0, user.GetTimelineCount());
			Assert.AreEqual(0, user2.GetTimelineCount());
		}

		[Test]
		public void TestNewTimelineWillBeCurrentOnlyIfHasNoTimelinesBefore()
		{
			Assert.AreEqual(0, user.GetTimelineCount());
			Timeline timelineCreated= user.CreateTimeline("my life", "timeline_description");
			Assert.AreEqual(timelineCreated.Id,user.CurrentTimeline.Id);

			Timeline newTimelineCreated = user.CreateTimeline("my life2", "timeline_description2");
			Assert.AreEqual(timelineCreated.Id, user.CurrentTimeline.Id);
			Assert.AreNotEqual(newTimelineCreated.Id, user.CurrentTimeline.Id);
		}

		[Test]
		public void TestNotifyCurrentTimelineChangedOnlyIfTimelineWasChanged()
		{
			bool notified = false;
			user.OnCurrentTimelineChanged += (sender, args) => notified = true; 

			Timeline timelineCreated = user.CreateTimeline("my life", "timeline_description");
			Assert.AreEqual(true, notified);
			notified = false;

			user.CurrentTimeline = timelineCreated;
			Assert.AreEqual(false, notified);

			Timeline newTimelineCreated = user.CreateTimeline("my life2", "timeline_description2");
			Assert.AreEqual(false, notified);

			user.CurrentTimeline = newTimelineCreated;
			Assert.AreEqual(true, notified);
			notified = false;

			var user2 = new User("test2", repository);
			repository.SaveItem(user2);
			Timeline timelineUser2 = user2.CreateTimeline("2", "2");

			user.CurrentTimeline = timelineUser2;
			Assert.AreEqual(false, notified);
		}

		[Test]
		public void TestNotifyWhenTimelineWasUpdated()
		{
			bool notified = false;
			user.OnTimelineUpdated += (sender, args) => notified = true; 

			Timeline timelineToUpdate = user.CreateTimeline("my life", "timeline_description");
			Assert.AreEqual(false, notified);

			timelineToUpdate.Name = "Nome lindo";
			user.UpdateTimeline(timelineToUpdate);
			Assert.AreEqual(true, notified);
		}

		public static void Main()
		{
			NUnit.ConsoleRunner.Runner.Main(new string[]
			{
				Assembly.GetExecutingAssembly().Location, 
			});
		}

	}



}

