package iSeconds.Droid;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;

public class TimelineItemView extends FrameLayout {
	private class TimelineItem {
		public String thumbnailPath;
	}
	
		 
	class TimelineItemHolder implements ItemHolder<TimelineItem> {    
		private ImageView thumbnail;		
		private ImageViewAsyncLoader imageLoader;
		
	    public TimelineItemHolder(View base) {
	    	thumbnail = (ImageView) base.findViewById(R.id.itemMonthDayImage);	    	
	    	imageLoader= new ImageViewAsyncLoader(thumbnail);
	    } 
	    
	    public void Update(TimelineItem item) {
	    	if (item.thumbnailPath != null && !item.thumbnailPath.isEmpty())
	    		loadThumbnail(item.thumbnailPath);	    	
	    }

		private void loadThumbnail(String thumbnailPath) {
			imageLoader.execute(thumbnailPath);
		}
	    
	}
	
	public class TimelineItemHolderFactory implements ItemHolderFactory<TimelineItem> {
		@Override
		public ItemHolder<TimelineItem> Build(View view) {
			return new TimelineItemHolder(view);
		}		
	}
	
	
	private TextView title;
	private GridView gridDays;
	
	private ListItemsAdapter<TimelineItem> adapter= null;

	public TimelineItemView(Context context) {
		super(context);
		
		initView(context);
	}
	
	public TimelineItemView(Context context, AttributeSet attrs) {
		super(context, attrs);
		
		initView(context);
	}

	private void initView(Context context) {
		View.inflate(context, R.layout.item_timeline, this);
		
		title= (TextView) findViewById(R.id.itemTimelineMonthYear);
		gridDays= (GridView) findViewById(R.id.itemTimelineDays);
		
		adapter= new ListItemsAdapter<TimelineItemView.TimelineItem>(
				this.getContext(), null, new TimelineItemHolderFactory(), R.layout.item_timeline);
		gridDays.setAdapter(adapter);
	}

	public void setTitle(String title)
	{
		this.title.setText(title);
	}
	
	public void setDays(List<String> days) {
		List<TimelineItem> monthItems= new ArrayList<TimelineItem>();
		for (String path: days) {
			TimelineItem item= new TimelineItem();
			item.thumbnailPath= path;
			monthItems.add(item);
		}

		adapter.setItems(monthItems);
	}
}
