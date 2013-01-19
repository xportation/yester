using NUnit.Framework;
using System;
using iSeconds.Domain;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class UserTest
	{
		[Test()]
		public void TestUserShouldHaveTimelines()
		{
			User user = new User();
			user.CreateTimeline();			
			Assert.AreEqual(1, user.GetTimelineCount());

			user.CreateTimeline();
			Assert.AreEqual(2, user.GetTimelineCount());
		}

		[Test()]
		public void TestUserNotifiesOnNewTimelines()
		{
			User user = new User();

			
			user.CreateTimeline();
		}

	}
}

