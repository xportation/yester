using System;

namespace iSeconds.Domain
{
	public interface IOptionsDialogService
	{
		void ShowModal(OptionsList options);

		/*Shows a message with only an ok button*/
		void ShowMessage(string msg, Action callback);
		void AskForConfirmation(string msg, Action userConfirmedCallback, Action userCanceledCallback);

		void AskForCompilationNameAndDescription(string defaultName, string defaultDescription, 
			Action<string, string> userConfirmedCallback, Action userCanceledCallback);

		void AskForTimelineNameAndDescription(string defaultName, string defaultDescription, 
			Action<string, string> userConfirmedCallback, Action userCanceledCallback);

		/// <summary>
		/// Shows the progress dialog.
		/// </summary>
		/// <param name="actionToPerforme">Action to performe. Be carefull if you need to update your GUI, 
		/// this is executed in separeted thread.</param>
		/// <param name="message">Message.</param>
		void ShowProgressDialog(Action actionToPerform, string message);
	}
}

