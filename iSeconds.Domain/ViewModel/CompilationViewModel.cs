using System;
using System.Collections.Generic;
using System.Windows.Input;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain
{
	public class CompilationViewModel
	{
		private IMediaService mediaService = null;
		private User user = null;

		private IOptionsDialogService dialogService = null;
		private I18nService i18n = null;

		private INavigator navigator = null;

		public class CompilationItemViewModel : ListItemViewModel
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public string Description { get; set; }

			public string Path { get; set; }

			public string BeginDate { get; set; }

			public string EndDate { get; set; }

			public string ThumbnailPath { get; set; }

			public string CompilationSize { 
				get {
					return ISecondsUtils.FileSizeFormated(this.Path);
				} 
			}

			public Compilation Model {
				get;
				set;
			}

			public CompilationItemViewModel(int id, string name, string description, string path
				, string beginDate, string endDate, string thumbnail, Compilation model)
				: base("", null)
			{
				this.Id = id;
				this.Name = name;
				this.Description = description;
				this.Path = path;
				this.BeginDate = beginDate;
				this.EndDate = endDate;
				this.ThumbnailPath = thumbnail;
				this.Model = model;
			}
		}

		private List<ListItemViewModel> compilations = new List<ListItemViewModel>();
		public List<ListItemViewModel> Compilations 
		{
			get {
				return compilations;
			}
		}

		public CompilationViewModel(User user, IMediaService mediaService, IOptionsDialogService dialogService, 
			INavigator navigator, I18nService i18n) 
		{
			this.user = user;
			this.mediaService = mediaService;
			this.dialogService = dialogService;
			this.i18n = i18n;
			this.navigator = navigator;

			loadCompilations();
		}

		private void loadCompilations() 
		{
			IList<Compilation> savedCompilations = user.GetCompilations ();

			// reverse iterating to show the last compilations first
			compilations.Clear();
			for (int i = savedCompilations.Count - 1; i >= 0; i--) {
				Compilation c = savedCompilations [i];

				string begin = ISecondsUtils.DateToString(c.Begin, false);
				string end = ISecondsUtils.DateToString(c.End, false);

				compilations.Add (new CompilationItemViewModel(c.Id, c.Name, c.Description, c.Path, begin, end, c.ThumbnailPath, c));
			}
			notifyChanges();
		}


		public event EventHandler<GenericEventArgs<CompilationViewModel>> OnCompilationViewModelChanged;

		private void notifyChanges()
		{
			if (OnCompilationViewModelChanged != null)
				OnCompilationViewModelChanged(this, new GenericEventArgs<CompilationViewModel>(this));
		}

		public ICommand PlayVideoCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];

					Args args = new Args();
					args.Put("FileName", compilation.Path);
					args.Put("UsesController", "true");
					navigator.NavigateTo("single_shot_video_player", args);
				});

			}
		}


		public ICommand DeleteVideoCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];

					string msg = i18n.Msg("Are you sure? This operation cannot be undone.");
					dialogService.AskForConfirmation(msg, () => {
						user.DeleteCompilation(compilation.Model);
						loadCompilations();

					}
						, () => {} // cancel callback
					);

				});

			}
		}

		public ICommand ShowCompilationOptionsCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];

					OptionsList options = new OptionsList();
					options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Delete"), () => {
						DeleteVideoCommand.Execute(arg);
					}));
					options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Share"), () => {
						ShareCompilationCommand.Execute(arg);
					}));
					options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Edit compilation"), () => {
						EditCompilationCommand.Execute(arg);
					}));
					options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Cancel"), () => {}));
					dialogService.ShowModal(options);
				});
			}
		}

		public ICommand EditCompilationCommand {
			get {
				return new Command ((object arg) => {
					int selectedCompilation = (int)arg;
					CompilationItemViewModel compilationModel = (CompilationItemViewModel)compilations[selectedCompilation];
					dialogService.AskForCompilationNameAndDescription(compilationModel.Name, compilationModel.Description,
						(string name, string description) => {
							Compilation compilation = user.GetCompilationById(compilationModel.Id);
							compilation.Name = name;
							compilation.Description = description;

							//Updating cache
							compilationModel.Name = name;
							compilationModel.Description = description;

							user.UpdateCompilation(compilation);

							notifyChanges();
						}, 
						null);
				});
			}
		}

		public ICommand ShareCompilationCommand {
			get {
				return new Command ((object arg) => {
					int selectedCompilation = (int)arg;
					CompilationItemViewModel compilationModel = (CompilationItemViewModel)compilations[selectedCompilation];
					mediaService.ShareVideo(compilationModel.Path, i18n.Msg("Share via:"));
				});
			}
		}
	}
}

