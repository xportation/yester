package iSeconds.Droid;

import iSeconds.Domain.DayInfo;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.Media;
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

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.AbsListView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

public class TimelineFragment extends Fragment {

	private class MediaItem {
		public String thumbnail;
		public Date date;
		
		//TODO [leonardo] temporario que nao pode ser para sempre :p (os dois)
		public String videoPath;
		public long mediaId;
		
		public String dateString() {
			DateFormat format= DateFormat.getDateInstance(DateFormat.SHORT);
//			DateFormat format = new SimpleDateFormat("E, dd", Locale.getDefault());
			return format.format(date);
		}
	}
	
	class MediaItemHolder implements ItemHolder<MediaItem> {    
		private ImageView thumbnail;
		private TextView date;
		private AsyncImageLoader imageLoader;
				
	    public MediaItemHolder(View base) {
	    	thumbnail = (ImageView) base.findViewById(R.id.itemTimelineDayImage);
	    	date = (TextView) base.findViewById(R.id.itemTimelineDayText);
	    	imageLoader= new AsyncImageLoader();
	    } 
	    
	    public void Update(MediaItem item) {
	    	date.setText(item.dateString());
	    	if (item.thumbnail != null && item.thumbnail.length() > 0)
	    		loadThumbnail(item.thumbnail);
	    }

		private void loadThumbnail(String thumbnailPath) {
			imageLoader.load(thumbnailPath, thumbnail);
		}
	    
	}
	
	public class MediaItemHolderFactory implements ItemHolderFactory<MediaItem> {
		@Override
		public ItemHolder<MediaItem> Build(View view) {
			return new MediaItemHolder(view);
		}		
	}
		
	private User user = null;
	private List<MediaItem> items = null;
	private IRepository repository = null;
	
	private Toast toast = null;	
	private View rootView = null;
	private TextView toasTextView = null;
	private GridView listView;
		
	public TimelineFragment(){	
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		user = App.getUser(this);
		repository = App.getRepository(this);
		listView= (GridView) rootView.findViewById(R.id.timelineDays);		
		setupMonthViewer(listView);
		
		items = buildItems();
		Collections.reverse(items);
		ListItemsAdapter<MediaItem> adapter= new ListItemsAdapter<MediaItem>(rootView.getContext(), items, 
    			new MediaItemHolderFactory(), R.layout.item_timeline_day);
		
		listView.setAdapter(adapter);
		
//		user.onNewVideo.addListener(new EventSourceListener() {
//			
//			@Override
//			public void handleEvent(Object sender, Object args) {
//				TimelineFragment.this.listView.invalidateViews();
//				
//			}
//		});
		
		return rootView;
	}
	
	@Override
	public void onPause() {
		cancelToast();		
		super.onPause();
	}

	private void cancelToast() {
		if (toast != null)
			toast.cancel();
	};

	private void setupMonthViewer(GridView listView) {
		buildToast();
		listView.setOnScrollListener(new OnScrollListener() {
			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {

				if (items != null && items.size() > 0) {
					SimpleDateFormat format = new SimpleDateFormat("MMMM, yyyy", Locale.getDefault());
					String text = format.format(items.get(firstVisibleItem).date);
					toasTextView.setText(text);						
					toast.show();
				}
			}

			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {				
			}
			
		});
		
		listView.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View view, final int itemIndex, long id) {
				cancelToast();
				IOptionCallback callback = new IOptionCallback() {

					@Override
					public void invoke(int index) {
						switch (index) {
							case 0: 
								Intent playerIntent = new Intent(getActivity(), PlayerActivity.class);
								playerIntent.putExtra("FileName", items.get(itemIndex).videoPath);
								startActivity(playerIntent);
								break;
							case 1: 
								Intent mediaIntent = new Intent(getActivity(), MediaActivity.class);
								mediaIntent.putExtra("MediaId", items.get(itemIndex).mediaId);
								startActivity(mediaIntent);
								break;
						}
					}
					
				};
				
				OptionsList options = new OptionsList(callback);
				options.add("Play from here");				
				options.add("More...");				
				options.add("Delete");				
				options.add("Cancel");				
				OptionsDialog.ShowDialog(getActivity(), options);
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
		toast.setGravity(Gravity.TOP, 0, 70);
		toast.setView(layout);
	}

	private List<MediaItem> buildItems() {
//		Timeline timeline= user.getCurrentTimeline();
//		
//		List<MediaItem> items= new ArrayList<MediaItem>();
//		for (DayInfo day: timeline.getAllVideos()) {
//			for (MediaInfo media: day.getVideos()) {
//				MediaItem mediaItem= new MediaItem(); 
//				mediaItem.thumbnail= media.getThumbnailPath();
//				mediaItem.date= day.getDate();
//				mediaItem.videoPath = media.getVideoPath();
//				mediaItem.mediaId = media.getId();
//				items.add(mediaItem);
//			}
//		}
		List<MediaItem> items= new ArrayList<MediaItem>();
		List<Media> medias = repository.getAllMedias();
		for (Media media: medias) {
			MediaItem mediaItem= new MediaItem(); 
			mediaItem.thumbnail= media.getThumbnailPath();
			mediaItem.date= media.getDate();
			mediaItem.videoPath = media.getVideoPath();
			mediaItem.mediaId = media.getId();
			items.add(mediaItem);
		}
		return items;
	}
	
}
