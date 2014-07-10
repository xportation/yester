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

import org.lucasr.smoothie.AsyncGridView;
import org.lucasr.smoothie.ItemManager;
import org.lucasr.smoothie.SimpleItemLoader;

import uk.co.senab.bitmapcache.BitmapLruCache;
import uk.co.senab.bitmapcache.CacheableBitmapDrawable;
import android.content.Context;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.TransitionDrawable;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.Adapter;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.BaseAdapter;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;

/*
 * TODO:
 * resolver countdown (esta ficando sempre um segundo a mais)
 * orientation do video resultante esta errado
 * fazer com que a camera ja esteja iniciada ao começar a fazer o drop no fragment
 */

public class TimelineFragment extends Fragment {
	
	private class MediaItem {
			
		public Date date;
		public String thumbnail;
		
		//TODO [leonardo] temporario que nao pode ser para sempre :p (os dois)
		public long mediaId;
		public String videoPath;
		
		public String dateString() {
			DateFormat format= DateFormat.getDateInstance(DateFormat.SHORT);
	//		DateFormat format = new SimpleDateFormat("E, dd", Locale.getDefault());
	//		DateFormat format = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss", Locale.getDefault());
			format.setTimeZone(TimeZone.getTimeZone("UTC"));
			return format.format(date);
		}
		
	}
	
	class MediaItemHolder {    
		private TextView date;
		private ImageView thumbnail;
		
	    public MediaItemHolder(View base, String dateText) {
	    	thumbnail = (ImageView) base.findViewById(R.id.itemTimelineDayImage);
	    	date = (TextView) base.findViewById(R.id.itemTimelineDayText);
	    	date.setText(dateText);
	    } 
	    
		public void setImageDrawable(Drawable drawable) {
			thumbnail.setImageDrawable(drawable);			
		}

	}
	
	public class MediaAdapter extends BaseAdapter {
		private final Context context;
		private final List<MediaItem> items;

		public MediaAdapter(Context context, List<MediaItem> items) {
			this.items = items;
			this.context = context;
		}

		@Override
		public int getCount() {
		    if (items == null) {
		        return 0;
		    }

		    return items.size();
		}

		@Override
		public String getItem(int position) {
			return items.get(position).thumbnail;
		}

		@Override
		public long getItemId(int position) {
			return position;
		}

		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			if (convertView == null) {
				convertView = LayoutInflater.from(context).inflate(R.layout.item_timeline_day, parent, false);
				
				MediaItemHolder holder = new MediaItemHolder(convertView, items.get(position).dateString());
		        holder.setImageDrawable(null);
		        
				convertView.setTag(holder);
			}		

			return convertView;
		}
	}
	
	public class MediaListLoader extends SimpleItemLoader<String, CacheableBitmapDrawable> {
	    final BitmapLruCache bitmapCache;

	    public MediaListLoader(BitmapLruCache cache) {
	        bitmapCache = cache;
	    }

	    @Override
	    public CacheableBitmapDrawable loadItemFromMemory(String url) {
	        return bitmapCache.getFromMemoryCache(url);
	    }

	    @Override
	    public String getItemParams(Adapter adapter, int position) {
	        return (String) adapter.getItem(position);
	    }

	    @Override
	    public CacheableBitmapDrawable loadItem(String url) {
	        CacheableBitmapDrawable wrapper = bitmapCache.get(url);
	        if (wrapper == null) {
	            wrapper = bitmapCache.put(url, BitmapFactory.decodeFile(url));
	        }

	        return wrapper;
	    }

	    @Override
	    public void displayItem(View itemView, CacheableBitmapDrawable result, boolean fromMemory) {
	    	MediaItemHolder holder = (MediaItemHolder) itemView.getTag();

	        if (result == null)
	            return;

	        if (fromMemory) {
	            holder.setImageDrawable(result);
	        } else {
	            BitmapDrawable emptyDrawable = new BitmapDrawable(itemView.getResources());

	            TransitionDrawable fadeInDrawable =
	                    new TransitionDrawable(new Drawable[] { emptyDrawable, result });

	            holder.setImageDrawable(fadeInDrawable);
	            fadeInDrawable.startTransition(200);
	        }
	    }
	}
	
	private class LoadPatternsListTask extends AsyncTask<Void, Void, List<MediaItem>> {        
        @Override
        protected List<MediaItem> doInBackground(Void... params) {
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

       @Override
       protected void onPostExecute(List<MediaItem> items) {
    	   MediaAdapter adapter = new MediaAdapter(getActivity(), items);
           timelineGridView.setAdapter(adapter);
           setItems(items);
       }
   }
		
	private List<MediaItem> itemsCache = null;
	private IRepository repository = null;
		
	private View rootView = null;
    private BitmapLruCache cache; 
    private AsyncGridView timelineGridView;
	
	public TimelineFragment(){	
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		super.onCreateView(inflater, container, savedInstanceState);
		
		rootView = inflater.inflate(R.layout.fragment_timeline,
				container, false);
		
		repository = App.getRepository(this);
		timelineGridView= (AsyncGridView) rootView.findViewById(R.id.timelineDays);		

		BitmapLruCache.Builder bcBuilder = new BitmapLruCache.Builder(this.getActivity());
		bcBuilder.setMemoryCacheEnabled(true).setMemoryCacheMaxSizeUsingHeapSize(50f);
		cache = bcBuilder.build();
		
		MediaListLoader loader = new MediaListLoader(cache);

        ItemManager.Builder builder = new ItemManager.Builder(loader);
        builder.setPreloadItemsEnabled(true).setPreloadItemsCount(12);
        builder.setThreadPoolSize(6);

        timelineGridView.setItemManager(builder.build());
        
        setupTimelineActions(timelineGridView);
        updateItems();
		return rootView;
	}
	
	public void updateItems() {
		new LoadPatternsListTask().execute();		
	}
	
	private void setupTimelineActions(GridView listView) {
		listView.setOnScrollListener(new OnScrollListener() {
			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {

				if (itemsCache != null && itemsCache.size() > 0) {
					SimpleDateFormat format = new SimpleDateFormat("MMMM, yyyy", Locale.getDefault());
					String text = format.format(itemsCache.get(firstVisibleItem).date);
					ActionBar actionBar = ((ActionBarActivity)getActivity()).getSupportActionBar();
					actionBar.setTitle(text);
				}
			}

			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {				
			}
			
		});
		
		listView.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View view, final int itemIndex, long id) {
				IOptionCallback callback = new IOptionCallback() {

					@Override
					public void invoke(int index) {
						switch (index) {
							case 0: 
								Intent playerIntent = new Intent(getActivity(), PlayerActivity.class);
								playerIntent.putExtra("FileName", itemsCache.get(itemIndex).videoPath);
								startActivity(playerIntent);
								break;
							case 1: 
								Intent mediaIntent = new Intent(getActivity(), MediaActivity.class);
								mediaIntent.putExtra("MediaId", itemsCache.get(itemIndex).mediaId);
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

	
	private void setItems(List<MediaItem> items) {		
		itemsCache = items;
	}
}
