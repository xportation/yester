using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace iSeconds.Domain
{
    public class DayViewModel : ViewModel
    {
        private DayInfo model = null;

        private IRepository repository = null;

        private IMediaService mediaService = null;
        

        public DayViewModel(DayInfo model, IRepository repository, IMediaService mediaService)
        {
            this.model = model;
            this.repository = repository;
            this.mediaService = mediaService;

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

        public ICommand DayClickedCommand
        {
            get
            {
                return new Command((object arg) => {

                    if (this.VideoPath == "")
                    {
                        mediaService.TakeVideo(this.model.Date, (string videoPath) =>
                        {
                            this.AddVideoCommand.Execute(videoPath);
                        });

                    }
                    else
                    {
                        mediaService.PlayVideo(this.videoPath);
                    }

                });
            }
        }

        public ICommand AddVideoCommand
        {
            get
            {
                return new Command((object arg) => {
                    string videoPath = (string) arg;
                    repository.SaveDay(this.model.TimelineId, this.model.Date, videoPath);
                    VideoPath = videoPath;
                });
            }
        }

    }
}
