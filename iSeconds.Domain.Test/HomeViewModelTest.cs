using NUnit.Framework;
using System;
using iSeconds.Domain;
using System.ComponentModel;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class HomeViewModelTest
	{
		ISecondsDB repository = null; 
		User user = null;
        IMediaService mockMediaService = new MockMediaService();
        INavigator navigator = new INavigator();

		[SetUp()]
		public void Init()
		{
			repository = new ISecondsDB("testbase.db3");
			repository.DeleteAll<User> ();
			user = new User ("xuxa", repository);
			repository.SaveItem (user);
		}

		class MockHomeView
		{
		}

		[Test()]
		public void TestUserWithoutTimelineHasNoCurrentTimeline ()
		{
			HomeViewModel viewModel = new HomeViewModel (user, repository, mockMediaService, navigator);
            Assert.Null(viewModel.CurrentTimeline);
            Assert.IsEmpty(viewModel.Timelines);
		}

        [Test()]
        public void TestModelViewNotifiesOnNewTimeline()
        {
            HomeViewModel viewModel = new HomeViewModel(user, repository, mockMediaService, navigator);
            MockHomeView view = new MockHomeView();

            bool wasCalled = false;
            viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                Assert.AreEqual("CurrentTimeline", args.PropertyName);
                wasCalled = true;

            };

            repository.SaveTimeline(new Timeline("xou da xuxa", user.Id, repository));

            Assert.IsTrue(wasCalled);
        }

	}
}

