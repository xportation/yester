using System;

namespace iSeconds.Domain
{
	public interface IOptionsDialogService
	{
		void ShowModal(OptionsList options);

		void AskForConfirmation(string msg, Action userConfirmedCallback, Action userCanceledCallback);

		void ShowTutorial(Action doneAction);
	}
}

