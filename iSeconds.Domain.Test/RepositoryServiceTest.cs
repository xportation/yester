//using NUnit.Framework;
//using System;
//using iSeconds.Domain;
//using System.Collections.Generic;
//
//namespace iSeconds
//{
//	[TestFixture()]
//	public class RepositoryServiceTest
//	{
//		UserService userService = null;
//		ISecondsDB persistence = null;
//		RepositoryService repository = null;
//
//		[SetUp()]
//		public void Init()
//		{
//			userService = new UserService();
//			persistence = new ISecondsDB ("testbase.db3");
//			persistence.DeleteAll<User> ();
//			repository = new RepositoryService(userService, persistence);
//		}
//
//		[Test()]
//		public void TestRepositoryListenToCurrentUser ()
//		{
//			userService.CurrentUser = new User ("xuxa");
//			Assert.AreEqual(1, persistence.GetUsers().Count);
//		}
//
//		[Test()]
//		public void TestUpdateSameUserShouldNotInsertTwiceInDb()
//		{
//			User user = new User ("pele");
//			userService.CurrentUser = user;
//			Assert.AreEqual(1, persistence.GetUsers().Count);
//
//			userService.CurrentUser = user;
//			Assert.AreEqual(1, persistence.GetUsers().Count);
//		}
//
//		[Test()]
//		public void TestDifferentUsersShouldBePersisted()
//		{
//			userService.CurrentUser = new User("pele");
//			Assert.AreEqual(1, persistence.GetUsers().Count);
//			
//			userService.CurrentUser = new User("romario");
//			Assert.AreEqual(2, persistence.GetUsers().Count);
//		}
//
//		[Test()]
//		public void TestShouldPersistUserName()
//		{
//			userService.CurrentUser = new User("pele");
//			Assert.AreEqual("pele", persistence.GetUsers()[0].Name);
//		}
//
//		[Test()]
//		public void TestUserReceivesUniqueIdOnPersistence()
//		{
//			User user = new User ("pele");
//			Assert.AreEqual(0, user.Id); // begins with id 0
//			userService.CurrentUser = user;
//			Assert.AreNotEqual(0, user.Id); // after inserted on db should receive id != 0
//		}
//
//		[Test()]
//		public void TestOnCreateATimelineRepositoryShouldPersist()
//		{
//			userService.CurrentUser = new User("pele");
//			userService.CurrentUser.CreateTimeline ("copa de 70");
//
//			User user = persistence.GetUser (userService.CurrentUser.Id);
//			Assert.AreEqual(1, user.GetTimelines ().Count);
//            Timeline timeline = user.GetTimelines()[0];
//            Assert.That(timeline.Name, Is.EqualTo("copa de 70"));
//		}
//
//        [Test()]
//        public void TestChangeTimelineName()
//        {
//            userService.CurrentUser = new User("pele");
//            userService.CurrentUser.CreateTimeline("copa de 70");
//
//            //Timeline timeline = persistence.GetTimeline(userService.CurrentUser.GetTimelines()[0].Id);
////            Assert.That(timeline.Name, Is.EqualTo("copa de 70"));
//
//
//        }
//
//        //[Test()]
//        //public void TestRemoveATimeline()
//        //{
//        //    userService.CurrentUser = new User("pele");
//        //    userService.CurrentUser.CreateTimeline("copa de 70");
//
//        //    User user = persistence.GetUser(userService.CurrentUser.Id);
//        //    Assert.AreEqual(1, user.GetTimelines().Count);
//
//        //    userService.CurrentUser.DeleteTimeline(0);
//        //}
//
//	}
//}
//
