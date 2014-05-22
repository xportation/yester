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
		
	public TimelineFragment() {		
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		GridView listView= (GridView) rootView.findViewById(R.id.timelineDays);
		
		List<DayItem> items= buildItems();
		ListItemsAdapter<DayItem> adapter= new ListItemsAdapter<DayItem>(rootView.getContext(), items, 
    			new DayItemHolderFactory(), R.layout.item_timeline_day);
		
		listView.setAdapter(adapter);
		
		return rootView;
	}

	private List<DayItem> buildItems() {
		List<DayItem> items= new ArrayList<DayItem>();
		
		File file= new File(android.os.Environment.getExternalStorageDirectory() + "/Yester.Droid/Videos");
		if (file.exists()) {
			for (File image: file.listFiles()) {
				String path= image.getAbsolutePath();
				if (path.contains(".png")) {
					DayItem day= new DayItem(); 
					day.thumbnail= path;
					items.add(day);
				}
			}
		}
		
		return items;
	}	
}
