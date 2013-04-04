using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class DayOptionsViewModel : ViewModel
	{
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

		private DayInfo model = null;
		
		public DayInfo Model 
		{
			get { return model; }
		}

		public DayOptionsViewModel(DayInfo model)
		{
			this.model = model;
			this.Init();
		}

		public void Init ()
		{
			IList<MediaInfo> videos = this.model.GetVideos();

			for (int i = 0; i < videos.Count; ++i )
			{
				MediaInfo media = videos[i];

				this.videos.Add(new VideoItem(media));				

				if (this.model.DefaultVideoId == media.Id)
					this.CheckedVideo = i;
			}
		}
	}

}

