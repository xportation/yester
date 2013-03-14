using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace iSeconds.Domain
{
	public class ViewModel : INotifyPropertyChanged
	{
		// boiler-plate
		public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            //if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


		
	}
}

