using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iSeconds.Domain;

namespace iSeconds.Android.Test
{
	[TestClass()]
	public class UserTest
	{
		public UserTest ()
		{
		}

		[TestMethod()]
		public void TestUserShouldHaveTimelines()
		{
			User user = new User();
			user.CreateTimeline();

			Assert.AreEqual(1, user.GetTimelineCount());
		}
	}
}

