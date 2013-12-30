using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class CompilationViewModel 
	{
		private IMediaService mediaService = null;
		private User user = null;

		public class CompilationItemViewModel : ListItemViewModel
		{
			public string Name {
				get;
				set;
			}

			public string Description {
				get;
				set;
			}

			public string Path {
				get;
				set;
			}

			public string BeginDate {
				get;
				set;
			}

			public string EndDate {
				get;
				set;
			}

			public string ThumbnailPath {
				get;
				set;
			}

			public CompilationItemViewModel(string name, string description, string path
				, string beginDate, string endDate, string thumbnail)
				: base("", null)
			{
				this.Name = name;
				this.Description = description;
				this.Path = path;
				this.BeginDate = beginDate;
				this.EndDate = endDate;
				this.ThumbnailPath = thumbnail;
			}
		}

		private List<ListItemViewModel> compilations = new List<ListItemViewModel>();
		public List<ListItemViewModel> Compilations 
		{
			get {
				return compilations;
			}
		}

		public CompilationViewModel(User user, IMediaService mediaService) 
		{
			this.user = user;
			this.mediaService = mediaService;
			loadCompilations ();
		}

		private void loadCompilations() 
		{
			IList<Compilation> savedCompilations = user.GetCompilations ();
			foreach (Compilation c in savedCompilations) {

				string begin = c.Begin.ToString ();
				string end = c.End.ToString ();

				compilations.Add (new CompilationItemViewModel(c.Name, c.Description, c.Path, begin, end, c.ThumbnailPath));
			}

			//OnInvalidate(this, null);
		}

		public ICommand PlayVideoCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];
					mediaService.PlayVideo(compilation.Path);
				});

			}
		}

		public ICommand ShowVideoOptionsCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];
					// TODO: fazer as opçoes...
					//					mediaService.PlayVideo(compilation.Path);
				});
			}
		}

		public ICommand AddCompilationCommand {
			get {
				return new Command ((object arg) => {
					string path = (string)arg;
					Compilation compilation = new Compilation();
					compilation.Path = path;
					//compilation.Name = 
					user.AddCompilation(compilation);
					loadCompilations();
				});
			}
		}
	}
}

