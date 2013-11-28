using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Input;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain
{
	public class DayOptionsViewModel : ViewModel
	{
		private IMediaService mediaService = null;
		private DayInfo model = null;
		private INavigator navigator = null;
		private IOptionsDialogService optionsDialogService = null;
		private Timeline timeline = null;

		public DayInfo Model 
		{
			get { return model; }
		}

		public DayOptionsViewModel(Timeline timeline, DayInfo model, INavigator navigator, IMediaService mediaService
		                           , IOptionsDialogService optionsDialogService)
		{
			this.timeline = timeline;
			this.model = model;
			this.navigator = navigator;
			this.mediaService = mediaService;
			this.optionsDialogService = optionsDialogService;
			this.Init();
		}

		public void Init ()
		{
			this.videos.Clear ();

			IList<MediaInfo> videos = this.model.GetVideos();

			for (int i = 0; i < videos.Count; ++i )
			{
				MediaInfo media = videos[i];

				this.videos.Add(new VideoItem(media));				

				if (this.model.DefaultVideoId == media.Id)
					this.CheckedVideo = i;
			}

			Videos = this.videos; // fazemos isso para notificar mudança e view se atualizar
		}

		public class VideoItem
		{
			public VideoItem(MediaInfo model)
			{
				this.Model = model;
				this.Label = Path.GetFileNameWithoutExtension(model.Path);
			}

			public MediaInfo Model { get; set; }
			public string Label { get; set; }

			public event EventHandler<GenericEventArgs<bool>> OnCheckedChanged;

			private bool isChecked;
			public bool Checked 
			{ 
				get
				{
					return isChecked;
				}
				set 
				{
					isChecked = value;
					if (OnCheckedChanged != null)
						OnCheckedChanged(this, new GenericEventArgs<bool>(isChecked));
				} 
			}

		}

		private IList<VideoItem> videos = new List<VideoItem>();
		public IList<VideoItem> Videos
		{
			get
			{
				return videos;
			}
			set
			{
				this.SetField(ref videos, value, "Videos");
			}
		}

		private int checkedVideo;
		public int CheckedVideo { 
			get 
			{
				return checkedVideo;
			}
			set 
			{
				checkedVideo = value;
				for (int i = 0; i < videos.Count; ++i)
				{
					VideoItem item = videos[i];
					item.Checked = (i == checkedVideo);
				}
			} 
		}

		public ICommand CheckVideoCommand {
			get { 
				return new Command((object arg) => {
					CheckedVideo = (int)arg;
					this.model.SetDefaultVideo(this.videos[CheckedVideo].Model.Id);
				}); 
			}
		}


      public ICommand BackToHomeCommand
      {
         get { return new Command((object arg) => { navigator.NavigateBack(); }); }
      }

		public ICommand TakeVideoCommand
		{
			get { 
				return new Command ((object arg) => { 
					mediaService.TakeVideo(this.model.Date, (string videoPath) =>
					{
						this.model.AddVideo(videoPath);
						mediaService.SaveVideoThumbnail(model.GetDefaultThumbnail(), model.GetDefaultVideoPath());
						Init();
						//OnPropertyChanged("VideoListChanged");
					});

				});
			}
		}

		public ICommand PlayVideoCommand 
		{
			get {
				return new Command ((object arg) => {
					int selectedVideo = (int)arg;
					mediaService.PlayVideo(this.videos[selectedVideo].Model.Path);
				});
			}
		}

		public ICommand DeleteVideoCommand
		{
			get {
				return new Command ((object arg) => {
					int selectedVideo = (int)arg;

					optionsDialogService.AskForConfirmation(
						"Are you sure? This operation cannot be undone!",
						() => {
							this.timeline.DeleteVideoAt(model.Date, this.videos[selectedVideo].Model.Path);
							this.Init();
						}, // confirmcallback

						() => {} //cancelcallback
					);
					                                        
				});
			}
		}



		public class VideoOptionsList : OptionsList
		{
			public VideoOptionsList(DayOptionsViewModel viewModel, int selectedVideo)
			{
				AddEntry(new OptionsEntry("Set as default", () => { viewModel.CheckVideoCommand.Execute(selectedVideo); }));

				AddEntry(new OptionsEntry("Delete video", () => {
					viewModel.DeleteVideoCommand.Execute(selectedVideo);
				}));
			}
		}


		public ICommand ShowVideoOptionsCommand
		{
			get {
				return new Command ((object arg) => {
					int selectedVideo = (int)arg;
					optionsDialogService.ShowModal(new VideoOptionsList(this, selectedVideo));
				});
			}
		}

	}

}

