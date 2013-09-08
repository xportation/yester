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
								OnPropertyChanged("VideoRecorded");
                     });
               });
         }
      }

      public ICommand DayClickedCommand
      {
         get
         {
				return new Command ((object arg) => {

					Args args = new Args ();
					args.Put ("Day", Model.Date.Day.ToString ());
					args.Put ("Month", Model.Date.Month.ToString ());
					args.Put ("Year", Model.Date.Year.ToString ());
					args.Put ("TimelineId", Model.TimelineId.ToString ());

					navigator.NavigateTo ("day_options", args);
				});
         }
      }

      public ICommand AddVideoCommand
      {
         get
         {
            return new Command((object arg) =>
               {
                  string videoPath = (string) arg;
                  this.model.AddVideo(videoPath);
						OnPropertyChanged("VideoAdded");
               });
         }
      }
   }
}