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
			user.CreateTimeline("my life");
			Assert.AreEqual(1, user.GetTimelineCount());

			user.CreateTimeline("my daughter's life");
			Assert.AreEqual(2, user.GetTimelineCount());
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

