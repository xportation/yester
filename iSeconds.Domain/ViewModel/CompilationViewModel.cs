using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class CompilationViewModel 
	{
		private IMediaService mediaService = null;
		private User user = null;
		private IOptionsDialogService dialogService = null;
		private I18nService i18n = null;

		public class CompilationItemViewModel : ListItemViewModel
		{
			public string Name { get; set; }

			public string Description { get; set; }

			public string Path { get; set; }

			public string BeginDate { get; set; }

			public string EndDate { get; set; }

			public string ThumbnailPath { get; set; }

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

		public CompilationViewModel(User user, IMediaService mediaService, IOptionsDialogService dialogService, I18nService i18n) 
		{
			this.user = user;
			this.mediaService = mediaService;
			this.dialogService = dialogService;
			this.i18n = i18n;

			loadCompilations ();
		}

		private void loadCompilations() 
		{
			IList<Compilation> savedCompilations = user.GetCompilations ();

			// reverse iterating to show the last compilations first
			for (int i = savedCompilations.Count - 1; i >= 0; i--) {
				Compilation c = savedCompilations [i];

				string begin = ISecondsUtils.DateToString(c.Begin, false);
				string end = ISecondsUtils.DateToString(c.End, false);

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

		public class CompilationOptionsList : OptionsList
		{
			public CompilationOptionsList(CompilationViewModel viewModel, int selectedVideo, I18nService i18n)
			{
				AddEntry(new OptionsEntry(i18n.Msg("Share"), () => {}));
				AddEntry(new OptionsEntry(i18n.Msg("Edit compilation"), () => {}));
				AddEntry(new OptionsEntry(i18n.Msg("Delete compilation"), () => {
					viewModel.DeleteCompilationCommand.Execute(selectedVideo);
				}));

				AddEntry(new OptionsEntry(i18n.Msg("Cancel"), () => {}));
			}
		}

		public ICommand ShowCompilationOptionsCommand {
			get {
				return new Command ((object arg) => {
					int selectedCompilation = (int)arg;
					dialogService.ShowModal(new CompilationOptionsList(this, selectedCompilation, i18n));
				});
			}
		}

		public ICommand DeleteCompilationCommand {
			get {
				return new Command ((object arg) => {
					int selectedCompilation = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[selectedCompilation];
				});
			}
		}
	}
}

