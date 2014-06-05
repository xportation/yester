package iSeconds.Droid;

import iSeconds.Domain.DayInfo;
import iSeconds.Domain.MediaInfo;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.List;
import java.util.Locale;

import android.content.res.TypedArray;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

public class TimelineFragment extends Fragment {

	private class DayItem {
		public String thumbnail;
		public Date date;
		
		public String dateString() {
			DateFormat format= DateFormat.getDateInstance(DateFormat.SHORT);
			return format.format(date);
		}
	}
	
	class DayItemHolder implements ItemHolder<DayItem> {    
		private ImageView thumbnail;
		private TextView date;
		private AsyncImageLoader imageLoader;
				
	    public DayItemHolder(View base) {
	    	thumbnail = (ImageView) base.findViewById(R.id.itemTimelineDayImage);
	    	date = (TextView) base.findViewById(R.id.itemTimelineDayText);
	    	imageLoader= new AsyncImageLoader();
	    } 
	    
	    public void Update(DayItem item) {
	    	date.setText(item.dateString());
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
		
	private User user = null;
	private List<DayItem> items = null;
	
	private Toast toast = null;
	private String currentToastText = null;
		
	public TimelineFragment(){	
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		user = App.getUser(this);
		GridView listView= (GridView) rootView.findViewById(R.id.timelineDays);		
		setupMonthViewer(listView);
		
		items = buildItems();
		Collections.reverse(items);
		ListItemsAdapter<DayItem> adapter= new ListItemsAdapter<DayItem>(rootView.getContext(), items, 
    			new DayItemHolderFactory(), R.layout.item_timeline_day);
		
		listView.setAdapter(adapter);
		
		return rootView;
	}

	private void setupMonthViewer(GridView listView) {
		currentToastText = new String();
		toast = Toast.makeText(this.getActivity(), currentToastText, Toast.LENGTH_SHORT);			
		toast.setGravity(Gravity.TOP, 0, 100);
		listView.setOnScrollListener(new OnScrollListener() {
			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {

				if (items != null && items.size() > 0) {
					SimpleDateFormat format = new SimpleDateFormat("MMMM, yyyy", Locale.US);
					String text = format.format(items.get(firstVisibleItem).date);
					if (!currentToastText.equals(text)) {
						currentToastText = text;
						toast.setText(currentToastText);
						toast.cancel();
						toast.show();
					}
				}
			}

			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {				
			}
			
		});
	}

	private List<DayItem> buildItems() {
		Timeline timeline= user.getCurrentTimeline();
		
		List<DayItem> items= new ArrayList<DayItem>();
		for (DayInfo day: timeline.getAllVideos()) {
			for (MediaInfo media: day.getVideos()) {
				DayItem dayItem= new DayItem(); 
				dayItem.thumbnail= media.getThumbnailPath();
				dayItem.date= day.getDate();
				items.add(dayItem);
			}
		}
		
//		File file= new File(android.os.Environment.getExternalStorageDirectory() + "/Yester.Droid/Videos");
//		if (file.exists()) {
//			for (File image: file.listFiles()) {
//				String path= image.getAbsolutePath();
//				if (path.contains(".png")) {
//					DayItem day= new DayItem(); 
//					day.thumbnail= path;
//					items.add(day);
//				}
//			}
//		}
		
		return items;
	}
}
