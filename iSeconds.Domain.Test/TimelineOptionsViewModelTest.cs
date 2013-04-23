using System;
using NUnit.Framework;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain.Test
{
	[TestFixture]
	public class TimelineOptionsViewModelTest
	{
		User user = null;
		ISecondsDB repository = null;
		INavigator navigator = new INavigator();
		TimelineOptionsViewModel viewModel = null;


		[SetUp]
		public void Init()
		{
			repository = new ISecondsDB("testbase.db3");
			repository.Reset();

			user = new User("Mano Bródi", repository);
			repository.SaveItem(user);

			viewModel = new TimelineOptionsViewModel(navigator, user, repository);
		}

		[Test]
		public void TestTimelineAtReturnNullIfOutOfRange()
		{
			Timeline timeline = viewModel.TimelineAt(0);
			Assert.IsNull(timeline);
		}

		[Test]
		public void TestCreateNewTimelineWhenDeleteTheLastExisting()
		{
			viewModel.AddTimeline("Bacana","Óióió ow");
			Assert.AreEqual(1, viewModel.TimelinesCount());

			var timeline = viewModel.TimelineAt(0);
			Assert.AreEqual("Bacana", timeline.Name);
			Assert.AreEqual("Óióió ow", timeline.Description);

			viewModel.DeleteTimeline(timeline);
			Assert.AreEqual(1, viewModel.TimelinesCount());

			timeline = viewModel.TimelineAt(0);
			Assert.AreNotEqual("Bacana", timeline.Name);
			Assert.AreNotEqual("Óióió ow", timeline.Description);
		}

		[Test]
		public void TestNotifyModelChangedWhenSaveDeleteOrChangeCurrentTimeline()
		{
			bool notified = false;
			viewModel.OnTimelineOptionsViewModelChanged += (sender, args) => notified = true;
			Assert.IsFalse(notified);

			viewModel.AddTimeline("1","11");
			Assert.IsTrue(notified);
			notified = false;

			viewModel.AddTimeline("2", "22");
			Assert.IsTrue(notified);
			notified = false;

			var timeline = viewModel.TimelineAt(0);
			viewModel.SetCurrentTimeline(timeline);
			Assert.IsFalse(notified);

			timeline = viewModel.TimelineAt(1);
			viewModel.SetCurrentTimeline(timeline);
			Assert.IsTrue(notified);
			notified = false;

			Assert.AreEqual(2, viewModel.TimelinesCount());
			viewModel.DeleteTimeline(timeline);
			Assert.IsTrue(notified);

			Assert.AreEqual(1, viewModel.TimelinesCount());
		}

		[Test]
		public void TestDeleteCurrentTimelineMustSelectTheFirst()
		{
			viewModel.AddTimeline("1", "11");
			viewModel.AddTimeline("2", "22");
			viewModel.AddTimeline("3", "33");
			var timeline = viewModel.TimelineAt(2);

			viewModel.SetCurrentTimeline(timeline);
			viewModel.DeleteTimeline(timeline);

			timeline = user.CurrentTimeline;
			Assert.AreEqual("1", timeline.Name);
			Assert.AreEqual("11", timeline.Description);
		}
	}
}