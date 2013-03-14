using NUnit.Framework;
using System;
using iSeconds.Domain;
using System.Reflection;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class UserTest
	{
		[Test()]
		public void TestUserShouldHaveTimelines()
		{
			User user = new User();
			user.CreateTimeline("my life");			
			Assert.AreEqual(1, user.GetTimelineCount());

			user.CreateTimeline("my daughter's life");
			Assert.AreEqual(2, user.GetTimelineCount());
		}

		[Test()]
		public void TestUserNotifiesOnNewTimelines()
		{

			bool wasCalled = false;
			User user = new User();
			user.OnNewTimeline += (object source, GenericEventArgs<Timeline> e) => {
				Assert.AreEqual("my life", e.Value.Name);
				wasCalled = true;
			};
			user.CreateTimeline("my life");

			Assert.IsTrue(wasCalled);
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

