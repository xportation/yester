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
using iSeconds.Domain;

namespace iSeconds.Droid
{
	class ISecondsListViewAdapter : BaseAdapter
	{
		private Activity context;
		private List<ListItemViewModel> viewModel;

		public ISecondsListViewAdapter(Activity context, List<ListItemViewModel> viewModel)
		{
			this.context = context;
			this.viewModel = viewModel;
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) 
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.TextViewItem, null);
				TextView textView = view.FindViewById<TextView>(Resource.Id.textItem);
				TextViewUtil.ChangeForDefaultFont(textView, context, 24f);
				textView.Text = viewModel [position].ToString();

				textView.Touch += (object sender, View.TouchEventArgs e) => {
					this.viewModel[position].Callback.Invoke();
				};
			}


			return view;
		}

		public override int Count
		{
			get { return viewModel.Count; }
		}
	}


}

