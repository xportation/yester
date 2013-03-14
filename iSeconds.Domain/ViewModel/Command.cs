using System;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class Command : ICommand
	{
		private Action<object> action;
		
		public Command(Action<object> action)
		{
			this.action = action;
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
		
		public event EventHandler CanExecuteChanged;
		
		public void Execute(object parameter)
		{
			action.Invoke(parameter);
		}
	}
	
	

}

