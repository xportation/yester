package iSeconds.Droid;

import java.io.File;
import java.io.FilenameFilter;
import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ListView;

public class TimelineFragment extends Fragment {

	private class TimelineItem {
		public String title;
		public List<String> days;
	}
	
	class TimelineItemHolder implements ItemHolder<TimelineItem> {  
		private TimelineItemView itemView;
		
	    public TimelineItemHolder(View base) {
	    	itemView = new TimelineItemView(base.getContext());
	    } 
	    
	    public void Update(TimelineItem item) {
	    	itemView.setTitle(item.title);
	    	itemView.setDays(item.days);	    	
	    }	    
	}
	
	public class TimelineItemsAdapter extends BaseAdapter {

		private Context context;
		private List<TimelineItem> items;

		public TimelineItemsAdapter(Context context, List<TimelineItem> items)
		{
			this.context= context;
			this.items= items;
		}
		
		@Override
		public int getCount() {			
			return items.size();
		}

		@Override
		public Object getItem(int index) {
			return items.get(index);
		}

		@Override
		public long getItemId(int index) {
			return index;
		}

		@Override
		public View getView(int index, View convertView, ViewGroup parent) {
			View view = convertView;
			TimelineItemHolder itemHolder = null;
			if (convertView == null) {  
	            view = new TimelineItemView(context);  
	            itemHolder = new TimelineItemHolder(view);  
	            view.setTag(itemHolder);  
			} else {  
				itemHolder = (TimelineItemHolder) view.getTag();
			}  
			
			if (itemHolder != null)
				itemHolder.Update(items.get(index));
			
			return view;  
		}
	}

	
	public TimelineFragment() {		
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		ListView listView= (ListView) rootView.findViewById(R.id.timelineDays);  
		
		List<TimelineItem> items= buildItems();
		TimelineItemsAdapter adapter= new TimelineItemsAdapter(rootView.getContext(), items);
		listView.setAdapter(adapter);
		
		return rootView;
	}

	private List<TimelineItem> buildItems() {
		List<TimelineItem> items= new ArrayList<TimelineItem>();
		
		File file= new File(android.os.Environment.getExternalStorageDirectory() + "/Yester.Droid/Videos");
		if (file.exists()) {
			int counter= 0;
			TimelineItem item= null;
			for (File image: file.listFiles()) {
				if (counter == 0) { 
					item= new TimelineItem();
					item.days= new ArrayList<String>();
					item.title = "Coisa";
					items.add(item);
				}
				
				String path= image.getAbsolutePath();
				if (path.contains(".png")) {
					item.days.add(path);
				
					counter++;
				}
				
				if (counter >= 10)
					counter= 0;
			}
		}
		
		return items;
	}	
}
