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

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
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

/*
 * TODO:
 * resolver countdown (esta ficando sempre um segundo a mais)
 * orientation do video resultante esta errado
 * fazer com que a camera ja esteja iniciada ao começar a fazer o drop no fragment
 */
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
	    	if (item.thumbnail != null && item.thumbnail.length() > 0)
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
	private View rootView = null;
	private TextView toasTextView = null;
	private GridView listView;
	private ListItemsAdapter<DayItem> adapter;
	public TimelineFragment(){	
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		user = App.getUser(this);
		listView= (GridView) rootView.findViewById(R.id.timelineDays);		
		setupMonthViewer(listView);
		
		items = buildItems();
		adapter= new ListItemsAdapter<DayItem>(rootView.getContext(), items, 
    			new DayItemHolderFactory(), R.layout.item_timeline_day);
		
		listView.setAdapter(adapter);
		
		return rootView;
	}
	
	@Override
	public void onPause() {
		if (toast != null)
			toast.cancel();
		
		super.onPause();
	};

	public void updateItems() {
		items = buildItems();
		this.adapter.setItems(items);
		this.listView.invalidateViews();
		this.listView.invalidate();
		
	}
	private void setupMonthViewer(GridView listView) {
		buildToast();
		listView.setOnScrollListener(new OnScrollListener() {
			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {

				if (items != null && items.size() > 0) {
					SimpleDateFormat format = new SimpleDateFormat("MMMM, yyyy", Locale.US);
					String text = format.format(items.get(firstVisibleItem).date);
					toasTextView.setText(text);						
					toast.show();
				}
			}

			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {				
			}
			
		});
	}

	private void buildToast() {
		LayoutInflater inflater = this.getActivity().getLayoutInflater();
		ViewGroup toastLayout = (ViewGroup) rootView.findViewById(R.id.fragmentTimelineMonthToastLayout); 
		View layout = inflater.inflate(R.layout.fragment_timeline_month_toast, toastLayout);

		toasTextView = (TextView) layout.findViewById(R.id.fragmentTimelineMonthToastText);
		toasTextView.setText("");
		
		toast = new Toast(this.getActivity());
		toast.setDuration(Toast.LENGTH_SHORT);
		toast.setGravity(Gravity.TOP, 0, 80);
		toast.setView(layout);
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
		
		Collections.reverse(items);
		
		return items;
	}

	
}
