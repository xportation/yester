using iSeconds.Domain.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using iSeconds.Domain.Framework;


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

            this.videoPath = model != null ? model.GetThumbnail() : "";
        }

        public Day PresentationInfo
        {
            get;
            set;
        }

        public DayInfo Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        private string videoPath = string.Empty;
        public string VideoPath
        {
            get { return videoPath; }
            set
            {
                this.SetField(ref videoPath, value, "VideoPath");
            }
        }
        
        public string VideoThumbnailPath
        {
            get
            {
                return getThumbnailPath(videoPath);
            }
        }

        private string getThumbnailPath(string videoPath)
        {
            if (videoPath.Length == 0)
                return "";

            return System.IO.Path.GetDirectoryName(videoPath) + "/" + System.IO.Path.GetFileNameWithoutExtension(videoPath) + ".png";
        }

        public ICommand PlayVideoCommand
        {
            get
            {
                return new Command((object arg) => {
                    mediaService.PlayVideo(this.videoPath);
                });                
            }
        }

        public ICommand RecordVideoCommand
        {
            get
            {
                return new Command((object arg) =>
                {
                    mediaService.TakeVideo(this.model.Date, (string videoPath) =>
                    {
                        mediaService.SaveVideoThumbnail(this.getThumbnailPath(videoPath), videoPath);
                        this.AddVideoCommand.Execute(videoPath);
                    });
                });
            }
        }

        public ICommand DayClickedCommand
        {
            get
            {
                return new Command((object arg) => {

                    if (this.VideoPath == "")
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

        public class DayOptionsList
        {
            public class DayOptionsEntry
            {
                public DayOptionsEntry(string name, Action callback)
                {
                    this.Name = name;
                    this.Callback = callback;
                }

                public string Name { get; set; }
                public Action Callback { get; set; }
            }

            List<DayOptionsEntry> entries = new List<DayOptionsEntry>();
            public List<DayOptionsEntry> OptionsEntries
            {
                get
                {
                    return entries;
                }
            }

            public ICommand DayEntryClicked
            {
                get
                {
                    return new Command((object arg) => {
                        int index = (int)arg;
                        entries[index].Callback.Invoke();
                    });
                }
            }


            public DayOptionsList(DayViewModel viewModel)
            {
                entries.Add(new DayOptionsEntry("Record video", () => 
                {
                    viewModel.RecordVideoCommand.Execute(null);
                }));

                entries.Add(new DayOptionsEntry("Options", () =>
                {
                    Args args = new Args();
                    args.Put("Day", viewModel.Model.Date.Day.ToString());
                    args.Put("Month", viewModel.Model.Date.Month.ToString());
                    args.Put("Year", viewModel.Model.Date.Year.ToString());
                    args.Put("TimelineId", viewModel.Model.TimelineId.ToString());

                    viewModel.navigator.NavigateTo("day_options", args);
                }));
            }
        }

       private InteractionRequest<DayOptionsList> dayOptionsRequest = new InteractionRequest<DayOptionsList>();
	    
	    public InteractionRequest<DayOptionsList> DayOptionsRequest
        {
            get
            {
                return dayOptionsRequest;
            }
        }

        public ICommand DayOptionsCommand
        {
            get
            {
                return new Command((object arg) => {

                    DayOptionsRequest.Raise(new DayOptionsList(this));
                });
            }
        }

        public ICommand AddVideoCommand
        {
            get
            {
                return new Command((object arg) => {
                    string videoPath = (string) arg;
                    this.model.AddVideo(videoPath);
                    VideoPath = videoPath;
                });
            }
        }

    }
}
