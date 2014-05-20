package iSeconds.Droid;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

public class TimelineFragment extends Fragment {

	private class DayItem {
		public String thumbnail;
	}
	
	class DayItemHolder implements ItemHolder<DayItem> {    
		private ImageView thumbnail;
		private AsyncImageLoader imageLoader;
				
	    public DayItemHolder(View base) {
	    	thumbnail = (ImageView) base.findViewById(R.id.itemTimelineDayImage);
	    	imageLoader= new AsyncImageLoader();
	    } 
	    
	    public void Update(DayItem item) {
	    	if (item.thumbnail != null && !item.thumbnail.isEmpty())
	    		loadThumbnail(item.thumbnail);
	    }

		private void loadThumbnail(String thumbnailPath) {
			imageLoader.load(thumbnailPath, thumbnail);
		}
	    
	}
	
	public class DayItemHolderFactory implements ItemHolderFactory<DayItem> {
		@Override
		public ItemHolder<DayItem> Build(View view) {
			return new DayItemHolder(view);
		}		
	}
	
	private class TimelineItem {
		public String title;
		public List<DayItem> days;
	}
	
	class TimelineItemHolder implements ItemHolder<TimelineItem> {  
		private TextView title;
		private TextView count;
		private GridView days;
		
		private ListItemsAdapter<DayItem> adapter;
		
	    public TimelineItemHolder(View base) {
	    	title= (TextView) base.findViewById(R.id.itemTimelineMonthYear);
	    	count= (TextView) base.findViewById(R.id.itemTimelineCount);
	    	days= (GridView) base.findViewById(R.id.itemTimelineDays);
	    	
	    	adapter= new ListItemsAdapter<DayItem>(base.getContext(), null, 
	    			new DayItemHolderFactory(), R.layout.item_timeline_day);
	    	days.setAdapter(adapter);
	    } 
	    
	    public void Update(TimelineItem item) {
	    	title.setText(item.title);
	    	count.setText(Integer.valueOf(item.days.size()).toString());
	    	
	    	adapter.setItems(item.days);
	    }	    
	}
	
	public class TimelineItemHolderFactory implements ItemHolderFactory<TimelineItem> {
		@Override
		public ItemHolder<TimelineItem> Build(View view) {
			return new TimelineItemHolder(view);
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
		ListItemsAdapter<TimelineItem> adapter= new ListItemsAdapter<TimelineItem>(
				rootView.getContext(), items, new TimelineItemHolderFactory(), R.layout.item_timeline);
		
		listView.setAdapter(adapter);
		
		return rootView;
	}

	private List<TimelineItem> buildItems() {
		List<TimelineItem> items= new ArrayList<TimelineItem>();
		
		int qtds[] = { 12, 23, 30, 31, 18, 25, 20, 12, 23, 30, 31, 18, 25, 20, 12, 23, 30, 31, 18, 25, 20, 12, 23, 30, 31, 18, 25, 20, 12, 23, 30, 31, 18, 25, 20 }; 
		File file= new File(android.os.Environment.getExternalStorageDirectory() + "/Yester.Droid/Videos");
		if (file.exists()) {
			int counter= 0;
			int index= 0;
			TimelineItem item= null;
			for (File image: file.listFiles()) {
				if (counter == 0) { 
					item= new TimelineItem();
					item.days= new ArrayList<DayItem>();
					item.title = "Coisa";					
				}
				
				String path= image.getAbsolutePath();
				if (path.contains(".png")) {
					DayItem day= new DayItem(); 
					day.thumbnail= path;
					item.days.add(day);
				
					counter++;
				}
				
				if (counter >= qtds[index]) {
					counter= 0;
					items.add(item);
					index++;
				}
			}
			
			if (counter > 0)
				items.add(item);
		}
		
		return items;
	}	
}
