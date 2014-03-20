using System;
using System.Collections.Generic;
using System.Windows.Input;
using iSeconds.Domain.Framework;
using System.IO;

namespace iSeconds.Domain
{
	public class CompilationViewModel
	{
		private IMediaService mediaService = null;
		private User user = null;

		private IOptionsDialogService dialogService = null;
		private I18nService i18n = null;

		private INavigator navigator = null;
		private IRepository repository = null;

		private EventHandler<GenericEventArgs<Compilation>> compilationChangedHandler;

		public class CompilationItemViewModel : ListItemViewModel
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public string Description { get; set; }

			public string Path { get; set; }

			public string LockPath { get; set; }

			public string BeginDate { get; set; }

			public string EndDate { get; set; }

			public string ThumbnailPath { get; set; }

			public bool Done { get; set; }

			public string CompilationSize { 
				get {
					return ISecondsUtils.FileSizeFormated(this.Path);
				} 
			}

			public Compilation Model {
				get;
				set;
			}

			public CompilationItemViewModel(Compilation model, string beginDate, string endDate)
				: base("", null)
			{
				this.Id = model.Id;
				this.Name = model.Name;
				this.Description = model.Description;
				this.Path = model.Path;
				this.LockPath = model.LockPath();
				this.BeginDate = beginDate;
				this.EndDate = endDate;
				this.ThumbnailPath = model.ThumbnailPath;
				this.Model = model;
				this.Done = model.Done;
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
			INavigator navigator, I18nService i18n, IRepository repository) 
		{
			this.user = user;
			this.mediaService = mediaService;
			this.dialogService = dialogService;
			this.i18n = i18n;
			this.navigator = navigator;
			this.repository = repository;

			compilationChangedHandler = new EventHandler<GenericEventArgs<Compilation>>((sender, e) => this.loadCompilations());

			this.repository.OnSaveCompilation += compilationChangedHandler;
			this.repository.OnDeleteCompilation += compilationChangedHandler;

			checkCompilationsAreDoneByThumbnails();
			loadCompilations();
		}

		public void Disconnect()
		{
			this.repository.OnSaveCompilation -= compilationChangedHandler;
			this.repository.OnDeleteCompilation -= compilationChangedHandler;
		}

		private void loadCompilations() 
		{
			IList<Compilation> savedCompilations = user.GetCompilations ();

			// reverse iterating to show the last compilations first
			compilations.Clear();
			for (int i = savedCompilations.Count - 1; i >= 0; i--) {
				Compilation compilation = savedCompilations [i];

				string begin = ISecondsUtils.DateToString(compilation.Begin, false);
				string end = ISecondsUtils.DateToString(compilation.End, false);

				compilations.Add (new CompilationItemViewModel(compilation, begin, end));
			}
			notifyChanges();
		}

		private void checkCompilationsAreDoneByThumbnails()
		{
			IList<Compilation> compilations = user.GetCompilations();
			foreach (Compilation compilation in compilations) {
				if (!compilation.Done && File.Exists(compilation.ThumbnailPath)) {
					compilation.Done = true;
					user.UpdateCompilation(compilation);
				}
			}
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
						dialogService.ShowProgressDialog(() => {
							//repository notifies on compilation delete
							user.DeleteCompilation(compilation.Model);
						}, i18n.Msg("Please wait..."));
					}, null);
				});

			}
		}

		public ICommand ShowCompilationOptionsCommand {
			get {
				return new Command ((object arg) => {
					int pos = (int)arg;
					CompilationItemViewModel compilation = (CompilationItemViewModel)compilations[pos];

					if (!compilation.Done) {
						if (ISecondsUtils.IsFileLocked(compilation.Path+".lock")) {
							dialogService.ShowMessage(i18n.Msg("Please, wait for the compilation to finish"), null);
							return;
						}
					} 

					OptionsList options = new OptionsList();
               if (compilation.Done) {
                  options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Play"), () => {
                     PlayVideoCommand.Execute(arg);
                  }));
                  options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Edit compilation"), () => {
                     EditCompilationCommand.Execute(arg);
                  }));
                  options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Share"), () => {
                     ShareCompilationCommand.Execute(arg);
                  }));
               }
					options.AddEntry(new OptionsList.OptionsEntry(i18n.Msg("Delete"), () => {
						DeleteVideoCommand.Execute(arg);
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

							//repository notifies on compilation update
							user.UpdateCompilation(compilation);
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

