using iSeconds.Domain.Framework;
using System.Windows.Input;


namespace iSeconds.Domain
{
   public class DayViewModel : ViewModel
   {
      private DayInfo model = null;

      private IRepository repository = null;

      private IMediaService mediaService = null;

      private INavigator navigator = null;

      public DayViewModel(DayInfo model, IRepository repository, IMediaService mediaService, INavigator navigator)
      {
         this.model = model;
         this.repository = repository;
         this.mediaService = mediaService;
         this.navigator = navigator;
      }

      public Day PresentationInfo { get; set; }

      public DayInfo Model
      {
         get { return this.model; }
         set { this.model = value; }
      }

      public string VideoThumbnailPath
      {
			get { return model.GetDefaultThumbnail(); }
      }

      public ICommand PlayVideoCommand
      {
         get { return new Command((object arg) => { mediaService.PlayVideo(model.GetDefaultVideoPath()); }); }
      }

      public ICommand RecordVideoCommand
      {
         get
         {
            return new Command((object arg) =>
               {
                  mediaService.TakeVideo(this.model.Date, (string videoPath) =>
                     {
                        this.AddVideoCommand.Execute(videoPath);
								mediaService.SaveVideoThumbnail(model.GetDefaultThumbnail(), model.GetDefaultVideoPath());
                     });
               });
         }
      }

      public ICommand DayClickedCommand
      {
         get
         {
            return new Command((object arg) =>
               {
                  if (model.GetVideoCount() == 0)
                  {
                     RecordVideoCommand.Execute(null);
                  }
                  else
                  {
                     PlayVideoCommand.Execute(null);
                  }
               });
         }
      }

      #region Day Options

		public class DayOptionsList : OptionsList
      {
         public DayOptionsList(DayViewModel viewModel)
         {
            AddEntry(new OptionsEntry("Record video", () => { viewModel.RecordVideoCommand.Execute(null); }));

            AddEntry(new OptionsEntry("Options", () =>
               {
                  Args args = new Args();
                  args.Put("Day", viewModel.Model.Date.Day.ToString());
                  args.Put("Month", viewModel.Model.Date.Month.ToString());
                  args.Put("Year", viewModel.Model.Date.Year.ToString());
                  args.Put("TimelineId", viewModel.Model.TimelineId.ToString());

                  viewModel.navigator.NavigateTo("day_options", args);
               }));
         }

			public string[] ListNames()
			{
				string[] names= new string[OptionsEntries.Count];
				for (int i = 0; i < OptionsEntries.Count; i++)
					names[i] = OptionsEntries[i].Name;

				return names;
			}
      }

      private InteractionRequest<DayOptionsList> dayOptionsRequest = new InteractionRequest<DayOptionsList>();

      public InteractionRequest<DayOptionsList> DayOptionsRequest
      {
         get { return dayOptionsRequest; }
      }

      public ICommand DayOptionsCommand
      {
         get { return new Command((object arg) => { DayOptionsRequest.Raise(new DayOptionsList(this)); }); }
      }

      #endregion 

      public ICommand AddVideoCommand
      {
         get
         {
            return new Command((object arg) =>
               {
                  string videoPath = (string) arg;
                  this.model.AddVideo(videoPath);
               });
         }
      }
   }
}