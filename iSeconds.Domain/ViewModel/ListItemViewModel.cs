using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace iSeconds.Domain
{
	public class ListItemViewModel {

		private string text = null;
		public Action Callback { get; set; }

		public event EventHandler<GenericEventArgs<bool>> OnCheckedChanged;

		private List<ListItemViewModel> children = new List<ListItemViewModel>();
		public List<ListItemViewModel> Children 
		{
			get { return children; }
		}

		public ListItemViewModel(string text, Action callback)
		{
			this.text = text;
			this.Callback = callback;
			this.Checked = false;
		}

		private bool isChecked;
		public bool Checked 
		{ 
			get
			{
				return isChecked;
			}
			set 
			{
				isChecked = value;
				if (OnCheckedChanged != null)
					OnCheckedChanged(this, new GenericEventArgs<bool>(isChecked));
			} 
		}

		public override string ToString()
		{
			return this.text;
		}
	}
}

