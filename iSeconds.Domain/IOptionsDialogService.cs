using System;

namespace iSeconds.Domain
{
	public interface IOptionsDialogService
	{
		void ShowModal(OptionsList options);

		/*Shows a message with only an ok button*/
		void ShowMessage(string msg, Action callback);
		void AskForConfirmation(string msg, Action userConfirmedCallback, Action userCanceledCallback);

		void ShowTutorial(Action doneAction);

		void AskForCompilationNameAndDescription(string defaultName, string defaultDescription, 
			Action<string, string> userConfirmedCallback, Action userCanceledCallback);
	}
}

