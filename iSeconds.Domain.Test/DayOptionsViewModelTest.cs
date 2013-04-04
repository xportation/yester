using NUnit.Framework;
using System;

namespace iSeconds.Domain.Test
{
	[TestFixture()]
	public class DayOptionsViewModelTest
	{
		ISecondsDB repository;
		Timeline timeline;

		DayOptionsViewModel viewModel;
		DayInfo dayInfo;

		[SetUp()]
		public void Init()
		{
			repository = new ISecondsDB("testbase.db3");
			repository.Reset ();

			User user = new User("teste", repository);
			repository.SaveItem(user);

			timeline = user.CreateTimeline("timeline");

			DateTime date = new DateTime(1979, 1, 1);
			timeline.AddVideoAt(date, "videopath1");
			timeline.AddVideoAt(date, "videopath2");

			dayInfo = timeline.GetDayAt(date);
			
			viewModel = new DayOptionsViewModel(dayInfo);
		}

		[Test()]
		public void TestDayOptionsShouldShowAllVideosForADay ()
		{
			Assert.That(viewModel.Videos.Count, Is.EqualTo(2));
		}

		[Test()]
		public void TestCheckedVideoShouldReflectModel()
		{
			dayInfo.DefaultVideoId = dayInfo.GetMediaByPath("videopath2").Id;
			viewModel.Init();
			Assert.That(viewModel.CheckedVideo, Is.EqualTo(1));
		}

		[Test()]
		public void TestShouldAllowUserToSelectDefaultDayVideo()
		{
			viewModel.CheckVideoCommand.Execute(0);
			Assert.That(viewModel.CheckedVideo, Is.EqualTo(0));
			Assert.That(viewModel.Model.DefaultVideoId, Is.EqualTo(viewModel.Videos[0].Model.Id));

			viewModel.CheckVideoCommand.Execute(1);
			Assert.That(viewModel.CheckedVideo, Is.EqualTo(1));
			Assert.That(viewModel.Model.DefaultVideoId, Is.EqualTo(viewModel.Videos[1].Model.Id));
		}
	}
}



