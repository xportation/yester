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
	public class ExpandableListAdapter : BaseExpandableListAdapter
	{
		/*private List<String> groups = null;
		private	List<List<string>> children = null;*/
		private Context context = null;

		public class ExpandableGroup
		{
			public string GroupName { get; set; }
			public bool Checked { get; set; }

			private List<ListItemViewModel> childs = new List<ListItemViewModel>();
			public List<ListItemViewModel> Childs { get { return childs; } }

			public ExpandableGroup(string groupName)
			{
				this.GroupName = groupName;
				this.Checked = false;
			}

			public void AddChild (string child) 
			{
				ListItemViewModel newItem = new ListItemViewModel (child, null);
				childs.Add (newItem);

		/*		newItem.Callback = () => {
					newItem.Checked = !newItem.Checked;
				};*/
			}
		}

		IList<ListItemViewModel> items = null;

		public ExpandableListAdapter(Context context, IList<ListItemViewModel> items) 
		{
			this.context = context;
			this.items = items;

			foreach (var group in this.items) {
				foreach (var child in group.Children) {
					child.OnCheckedChanged += (object sender, GenericEventArgs<bool> e) => {
						this.NotifyDataSetChanged();
					};
				}			
			}
		}

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return items [groupPosition].Children [childPosition].ToString();
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			return items [groupPosition].Children.Count;
		}

		private void onClick(object sender, EventArgs e, ListItemViewModel viewModel)
		{
			viewModel.Checked = !viewModel.Checked;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			ListItemViewModel content = items[groupPosition].Children[childPosition];

			CheckedTextView tv = null;
			// TODO: tratar o recycle aqui... nao consegui fazer um disconnect all na porra do evento Click
			//if (convertView == null) {

				convertView = View.Inflate(this.context, Resource.Layout.ExpandableGroupLayout, null);
				

			//	tv = convertView.FindViewById<CheckedTextView>(Resource.Id.tvGroup);
			//}

			tv = convertView.FindViewById<CheckedTextView>(Resource.Id.tvGroup);
			TextViewUtil.ChangeForDefaultFont(tv, context, 18f);
			tv.Text = content.ToString();
			tv.Checked = content.Checked;

			tv.Click += (object sender, EventArgs e) => {
				content.Checked = !content.Checked;
			};

			//tv.Click = onClick;
			//tv.Click += ApplyPartial(onClick, content);

			// Depending upon the child type, set the imageTextView01
			//tv.setCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);

			return convertView;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return items [groupPosition].ToString();
			//return groups[groupPosition];
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			ListItemViewModel group = items[groupPosition];
			if (convertView == null) {
				convertView = View.Inflate(this.context, Resource.Layout.ExpandableGroupLayout, null);
			}
			CheckedTextView tv = convertView.FindViewById<CheckedTextView>(Resource.Id.tvGroup);
			TextViewUtil.ChangeForDefaultFont(tv, context, 24f);
			tv.Text = group.ToString();
			//tv.Checked = group.Checked;

			//CheckBox cb = convertView.FindViewById<CheckBox>(Resource.Id.checkBox);
			/*cb.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => {

			};*/
			//cb.Text = "teste";
			//cb.Checked = true;

			//tv.Touch += (object sender, View.TouchEventArgs e) => { group.Checked = !group.Checked; };
			return convertView;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GroupCount {
			get {
				return this.items.Count;
				//return this.groups.Count;
			}
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}


/*		



		public void addItem(Vehicle vehicle) {
			if (!groups.contains(vehicle.getGroup())) {
				groups.add(vehicle.getGroup());
			}
			int index = groups.indexOf(vehicle.getGroup());
			if (children.size() < index + 1) {
				children.add(new ArrayList<Vehicle>());
			}
			children.get(index).add(vehicle);
		}


		@Override
			public Object getChild(int groupPosition, int childPosition) {
			return children.get(groupPosition).get(childPosition);
		}


		@Override
			public long getChildId(int groupPosition, int childPosition) {
			return childPosition;
		}

		// Return a child view. You can load your custom layout here.
		@Override
			public View getChildView(int groupPosition, int childPosition, boolean isLastChild,
			                         View convertView, ViewGroup parent) {
			Vehicle vehicle = (Vehicle) getChild(groupPosition, childPosition);
			if (convertView == null) {
				LayoutInflater infalInflater = (LayoutInflater) context
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
				convertView = infalInflater.inflate(R.layout.child_layout, null);
			}
			TextView tv = (TextView) convertView.findViewById(R.id.tvChild);
			tv.setText("   " + vehicle.getName());


			// Depending upon the child type, set the imageTextView01
			tv.setCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);

			return convertView;
		}


		@Override
			public int getChildrenCount(int groupPosition) {
			return children.get(groupPosition).size();
		}


		@Override
			public Object getGroup(int groupPosition) {
			return groups.get(groupPosition);
		}


		@Override
			public int getGroupCount() {
			return groups.size();
		}


		@Override
			public long getGroupId(int groupPosition) {
			return groupPosition;
		}


		// Return a group view. You can load your custom layout here.
		@Override
			public View getGroupView(int groupPosition, boolean isExpanded, View convertView,
			                         ViewGroup parent) {
			String group = (String) getGroup(groupPosition);
			if (convertView == null) {
				LayoutInflater infalInflater = (LayoutInflater) context
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
				convertView = infalInflater.inflate(R.layout.group_layout, null);
			}
			TextView tv = (TextView) convertView.findViewById(R.id.tvGroup);
			tv.setText(group);
			return convertView;
		}


		@Override
			public boolean hasStableIds() {
			return true;
		}


		@Override
			public boolean isChildSelectable(int arg0, int arg1) {
			return true;
		}*/
	}
}

