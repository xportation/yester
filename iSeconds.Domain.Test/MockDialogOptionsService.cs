using System;

using iSeconds.Domain;

namespace iSeconds.Domain.Test
{
	public class MockOptionsDialogService : IOptionsDialogService
	{
		public void ShowModal(OptionsList options)
		{
		}

		public void AskForConfirmation (string msg, Action userConfirmedCallback, Action userCanceledCallback)
		{
			if (this.confirmationResult)
				userConfirmedCallback.Invoke ();
			else 
				userCanceledCallback.Invoke ();
		}

		public void SetConfirmationResult (bool result)
		{
			this.confirmationResult = result;
		}

		bool confirmationResult;
	}
}

