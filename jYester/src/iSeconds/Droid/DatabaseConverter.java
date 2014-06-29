package iSeconds.Droid;

import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.Media;
import iSeconds.Domain.SqlUtils;
import iSeconds.Domain.Tag;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

public class DatabaseConverter {

	private class TimelineCompat {
		public long id;
		public String name;
		
		public String tagnific() {			
			return name.replaceAll(" ", "_").toLowerCase();
		}
	}
	
	private class DayInfoCompat {
		public long id;
		public String date;
	}
	
	private class MediaInfoCompat {
		public String path;
		public String date;
		public long timeOfDay; 
	}
	
	private class LegacyDb extends SQLiteOpenHelper {

		public LegacyDb(Context context, String databaseName) {
	        super(context, databaseName, null, 1);
	    }
		
		@Override
		public void onCreate(SQLiteDatabase db) {			
		}

		@Override
		public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		}
		
		public List<TimelineCompat> loadTimelines() {
			SQLiteDatabase db = this.getReadableDatabase();
			 
		    String selectQuery = "SELECT  * FROM Timeline";
		    Cursor c = db.rawQuery(selectQuery, null);
		 
		    List<TimelineCompat> timelines = new ArrayList<TimelineCompat>();
		    if (c == null)
		        return timelines;
		 
		    if (c.moveToFirst()) {
		        do {
		        	TimelineCompat timeline = new TimelineCompat();
		        	timeline.id = c.getLong(c.getColumnIndex("Id"));
		            timeline.name = c.getString(c.getColumnIndex("Name"));
		            
		            timelines.add(timeline);
		        } while (c.moveToNext());
		    }
		    
		    return timelines;
		}

		private List<DayInfoCompat> loadDays(TimelineCompat timeline) {
			SQLiteDatabase db = this.getReadableDatabase();
			 
		    String selectQuery = "SELECT  * FROM DayInfo WHERE TimelineId = " + Long.toString(timeline.id);
		    Cursor c = db.rawQuery(selectQuery, null);
			
			List<DayInfoCompat> days= new ArrayList<DayInfoCompat>();
			if (c == null)
		        return days;
		 
		    if (c.moveToFirst()) {
		        do {
		        	DayInfoCompat day = new DayInfoCompat();
		        	day.id = c.getLong(c.getColumnIndex("Id"));
		            day.date = c.getString(c.getColumnIndex("Date"));
		            
		            days.add(day);
		        } while (c.moveToNext());
		    }
		    
			return days;
		}
		
		public List<MediaInfoCompat> loadMediasFromTimeline(TimelineCompat timeline) {
			List<DayInfoCompat> days= loadDays(timeline);
			List<MediaInfoCompat> medias= new ArrayList<MediaInfoCompat>();
			
			for (DayInfoCompat day : days) {
				SQLiteDatabase db = this.getReadableDatabase();
				 
			    String selectQuery = "SELECT  * FROM MediaInfo WHERE DayId = " + Long.toString(day.id);
			    Cursor c = db.rawQuery(selectQuery, null);
							
			    if (c == null)
			        return medias;
			 
			    if (c.moveToFirst()) {
			        do {
			        	MediaInfoCompat media = new MediaInfoCompat();
			        	media.timeOfDay = c.getLong(c.getColumnIndex("TimeOfDay"));
			            media.path = c.getString(c.getColumnIndex("Path"));
			            media.date = day.date;
			            
			            medias.add(media);
			        } while (c.moveToNext());
			    }
			}
			
			return medias;
		}
	}
	
	private Context context = null;
	private LegacyDb legacyDb = null;
	private IRepository repository = null;
		
	public DatabaseConverter(Context context, String legacyDbPath, IRepository repository) {
		this.context = context;
		this.repository = repository;
		
		this.legacyDb = new LegacyDb(context, legacyDbPath);
	}
	
	public void Convert() {
		List<TimelineCompat> timelines = legacyDb.loadTimelines();
		for (TimelineCompat timeline: timelines) {
			convertMediaFromTimeline(timeline);
		}
	}

	private void convertMediaFromTimeline(TimelineCompat timeline) {
		List<MediaInfoCompat> medias = legacyDb.loadMediasFromTimeline(timeline);
		Tag tag = tagIt(timeline);
		
		ActiveAndroid.beginTransaction();
		try {
			for (MediaInfoCompat mediaInfo : medias) {
				Media media = new Media();
				media.setDate(toDate(mediaInfo.date, toMiliseconds(mediaInfo.timeOfDay)));
//				media.setTime(toMiliseconds(mediaInfo.timeOfDay));
				media.setPath(mediaInfo.path);
				media.save();
				
				media.addTag(tag);
				repository.addMediaTag(media);
			}
			ActiveAndroid.setTransactionSuccessful();
		}
		finally {
			ActiveAndroid.endTransaction();
		}
	}

	private Date toDate(String date, long timeMiliseconds) {
		Date dateTime;
		try {
			dateTime= SqlUtils.parseDate(date);
		} catch (Exception e) {
			return null;
		}
		
		Date time = new Date(timeMiliseconds);
		dateTime.setHours(time.getHours());
		dateTime.setMinutes(time.getMinutes());
		dateTime.setSeconds(time.getSeconds());
		return dateTime;
	}

	private long toMiliseconds(long ticks) {
		final long TicksPerMillisecond = 10000L;
		return ticks / TicksPerMillisecond;
	}

	private Tag tagIt(TimelineCompat timeline) {
		Tag tag = new Tag();
		tag.setName(timeline.tagnific());
		tag.save();
		return tag;
	}
}
