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

        public IRepository repository = null;

        public DayViewModel(DayInfo model, IRepository repository)
        {
            this.model = model;
            this.repository = repository;

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
