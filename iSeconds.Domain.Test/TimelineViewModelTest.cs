﻿using iSeconds.Domain.Framework;
using NUnit.Framework;
using System;
using System.Globalization;

namespace iSeconds.Domain.Test
{
    [TestFixture()]
    class TimelineViewModelTest
    {
        User user = null;
        Timeline timeline = null;
        ISecondsDB repository = null;
        TimelineViewModel viewModel = null;

        MockMediaService mockMediaService = new MockMediaService();
        INavigator navigator = new INavigator();


        [SetUp()]
        public void Init()
        {
            repository = new ISecondsDB("testbase.db3");
				repository.Reset();

            user = new User("xuxa", repository);
            repository.SaveItem(user);
            timeline = new Timeline("xou da xuxa", user.Id);
            timeline.SetRepository(repository);
            repository.SaveTimeline(timeline);

            viewModel = new TimelineViewModel(user, repository, mockMediaService, navigator);
        }

        [Test()]
        public void TestOnCurrentDateChangedVisibleDaysChangesToo()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);

            Assert.That(viewModel.VisibleDays.Count, Is.EqualTo(42)); // fixed...
            assertDay(viewModel.VisibleDays[0], 24, false, false, true);
            assertDay(viewModel.VisibleDays[viewModel.VisibleDays.Count - 1], 6, false, false, true);
            assertDay(viewModel.VisibleDays[viewModel.VisibleDays.Count - 2], 5, false, false, false);
        }

        [Test()]
        public void TestJumpToNextMonthShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);
            assertDay(viewModel.VisibleDays[0], 24, false, false, true);

            viewModel.NextMonthCommand.Execute(null);
            assertDay(viewModel.VisibleDays[1], 1, true, false, false);
        }

        [Test()]
        public void TestJumpToPreviousMonthShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);
            assertDay(viewModel.VisibleDays[0], 24, false, false, true);

           viewModel.PreviousMonthCommand.Execute(null);
           assertDay(viewModel.VisibleDays[0], 27, false, false, true);
        }

       private void assertDay(DayViewModel day, int number, bool inCurrentMonth, bool isToday, bool isWeekend)
        {
           Assert.That(day.PresentationInfo.number, Is.EqualTo(number));
           Assert.That(day.PresentationInfo.inCurrentMonth, Is.EqualTo(inCurrentMonth));
           Assert.That(day.PresentationInfo.isToday, Is.EqualTo(isToday));
           Assert.That(day.PresentationInfo.isWeekend, Is.EqualTo(isWeekend));  
       }

       [Test()]
        public void TestGoToTodayShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = DateTime.Today;
            int firstDayVisible = viewModel.VisibleDays[0].PresentationInfo.number;

            viewModel.PreviousMonthCommand.Execute(null);
            viewModel.PreviousMonthCommand.Execute(null);
            viewModel.GoToTodayCommand.Execute(null);

            Assert.That(viewModel.VisibleDays[0].PresentationInfo.number, Is.EqualTo(firstDayVisible));
        }

        [Test()]
        public void TestOnMonthChangedShouldChangeMonthTitle()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);
            Assert.That(viewModel.CurrentMonthTitle, Is.EqualTo(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(3) + ", 2013"));
            
            viewModel.PreviousMonthCommand.Execute(null);
            // ok, month changed should change title
            Assert.That(viewModel.CurrentMonthTitle, Is.EqualTo(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(2) + ", 2013"));

            // but if we change only the day, without change month, should not change title
            viewModel.CurrentDate.AddDays(1);
            Assert.That(viewModel.CurrentMonthTitle, Is.EqualTo(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(2) + ", 2013"));
        }

        [Test()]
        public void TestTakeVideo()
        {
            // se ficar nesse design extrair esse teste para DayViewModelTest
            DayViewModel dayViewModel = viewModel.VisibleDays[0];
            dayViewModel.AddVideoCommand.Execute("video/path");

            Assert.That(dayViewModel.VideoThumbnailPath, Is.EqualTo("video/path.png"));

            DayInfo dayInfo = repository.GetDayInfoAt(dayViewModel.PresentationInfo.day, dayViewModel.Model.TimelineId);
            Assert.That(dayInfo.GetDefaultVideoPath(), Is.EqualTo("video/path"));
        }


		[Test()]
      public void TestClickOnADayShouldOpenTheDayOptions()
		{
			MockPresenter mockPresenter = new MockPresenter ();
			navigator.RegisterNavigation ("day_options", mockPresenter);

			DayViewModel dayViewModel = viewModel.VisibleDays [0];
            
			dayViewModel.DayClickedCommand.Execute (null);

			Assert.That (mockPresenter.wasShow);
		}

    }
}
