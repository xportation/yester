package iSeconds.Droid;

import iSeconds.Domain.IRepository;
import iSeconds.Domain.Media;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.TimeZone;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
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

	private class MediaItem {
		public String thumbnail;
		public Date date;
		
		//TODO [leonardo] temporario que nao pode ser para sempre :p (os dois)
		public String videoPath;
		public long mediaId;
		
		public String dateString() {
			DateFormat format= DateFormat.getDateInstance(DateFormat.SHORT);
//			DateFormat format = new SimpleDateFormat("E, dd", Locale.getDefault());
//			DateFormat format = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss", Locale.getDefault());
			format.setTimeZone(TimeZone.getTimeZone("UTC"));
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
		
//	private User user = null;
	private List<MediaItem> items = null;
	private IRepository repository = null;
	
	private Toast toast = null;	
	private View rootView = null;
	private TextView toasTextView = null;
	private GridView listView;
	private ListItemsAdapter<MediaItem> adapter;
	
	public TimelineFragment(){	
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		super.onCreateView(inflater, container, savedInstanceState);
		
		rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
//		user = App.getUser(this);
		repository = App.getRepository(this);
		listView= (GridView) rootView.findViewById(R.id.timelineDays);		
		setupMonthViewer(listView);
		
		items = buildItems();
		ListItemsAdapter<MediaItem> adapter= new ListItemsAdapter<MediaItem>(rootView.getContext(), items, 
    			new MediaItemHolderFactory(), R.layout.item_timeline_day);
		
		listView.setAdapter(adapter);
		
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
		List<Media> medias = repository.getAllMedias();
		List<MediaItem> items= new ArrayList<MediaItem>();
		for (Media media: medias) {
			MediaItem mediaItem= new MediaItem(); 
			mediaItem.thumbnail= media.getThumbnailPath();
			mediaItem.date= media.getDate();
			mediaItem.videoPath = media.getVideoPath();
			mediaItem.mediaId = media.getId();
			items.add(mediaItem);
		}
		
		Collections.sort(items, new Comparator<MediaItem>() {
			@Override
			public int compare(MediaItem item1, MediaItem item2) {
				return item1.date.compareTo(item2.date);
			}
			
		});
		
		Collections.reverse(items);		
		return items;
	}

	
}
