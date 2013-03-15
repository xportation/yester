using NUnit.Framework;
using System;

namespace iSeconds.Domain.Test
{
    [TestFixture()]
    class TimelineViewModelTest
    {
        User user = null;
        Timeline timeline = null;
        ISecondsDB repository = null;
        TimelineViewModel viewModel = null; 

        [SetUp()]
        public void Init()
        {
            repository = new ISecondsDB("testbase.db3");
            repository.DeleteAll<User>();
            user = new User("xuxa");
            repository.SaveItem(user);
            timeline = new Timeline("xou da xuxa", user.Id);
            repository.SaveTimeline(timeline);

            viewModel = new TimelineViewModel(timeline, repository);
        }

        [Test()]
        public void TestEmptyTimelineShouldHaveNoDays()
        {
            Assert.IsEmpty(viewModel.Days);
        }

        [Test()]
        public void TestOnCurrentDateChangedVisibleDaysChangesToo()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);

            Assert.That(viewModel.VisibleDays.Count, Is.EqualTo(42)); // fixed...
            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(new DateTime(2013, 2, 24)));
            Assert.That(viewModel.VisibleDays[viewModel.VisibleDays.Count-1].date, Is.EqualTo(new DateTime(2013, 4, 6)));
        }

        [Test()]
        public void TestJumpToNextMonthShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);
            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(new DateTime(2013, 2, 24)));

            viewModel.NextMonthCommand.Execute(null);
            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(new DateTime(2013, 3, 31)));
        }

        [Test()]
        public void TestJumpToPreviousMonthShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = new DateTime(2013, 3, 12);
            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(new DateTime(2013, 2, 24)));

            viewModel.PreviousMonthCommand.Execute(null);
            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(new DateTime(2013, 1, 27)));
        }

        [Test()]
        public void TestGoToTodayShouldChangeVisibleDays()
        {
            viewModel.CurrentDate = DateTime.Today;
            DateTime firstDayOfThisMonth = viewModel.VisibleDays[0].date;

            viewModel.PreviousMonthCommand.Execute(null);
            viewModel.PreviousMonthCommand.Execute(null);
            viewModel.GoToTodayCommand.Execute(null);

            Assert.That(viewModel.VisibleDays[0].date, Is.EqualTo(firstDayOfThisMonth));
        }

        [Test()]
        public void TestOnNewDay()
        {
            //TimelineViewModel viewModel = new TimelineViewModel(timeline, repository);

            //viewModel.AddVideoAt.Execute(DateTime.Today, "video.path");
        }
    }
}
