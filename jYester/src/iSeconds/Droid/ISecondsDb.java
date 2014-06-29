package iSeconds.Droid;

import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.Media;
import iSeconds.Domain.Tag;

import java.util.List;

import android.content.Context;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;
import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;
import com.activeandroid.query.Select;

public class ISecondsDb implements IRepository {

	private Context context = null;
	private IPathService pathService = null;

	public ISecondsDb(Context context, IPathService pathService) {
		this.context = context;
		this.pathService = pathService;
	}

	@Table(name = "MediaTags")
	private class MediaTags extends Model {

		@Column(name = "MediaId")
		public long mediaId;

		@Column(name = "TagId")
		public long tagId;
	}
	
	@Override
	public void open() {

		Configuration config = new Configuration.Builder(context)
				.setDatabaseName(pathService.getDbPath())
				.addModelClass(Media.class)
				.addModelClass(Tag.class)
				.addModelClass(MediaTags.class)
				.create();
		ActiveAndroid.initialize(config);
		
		importLegacyDb();
	}
	
	private void importLegacyDb() {
		if (pathService.isLegacyDb()) {
			DatabaseConverter converter = new DatabaseConverter(
					context, pathService.getLegacyDbPath(), this);
			converter.Convert();
			pathService.turnLegacyDbDisabled();
		}
	}

	@Override
	public void addMediaTag(Media media) {
		ActiveAndroid.beginTransaction();
		try {
			for (int i= 0; i < media.tagsCount(); i++) {
				Tag tag= media.tagAt(i);
				
				MediaTags mediaTags = new MediaTags();
				mediaTags.mediaId = media.getId();
				mediaTags.tagId = tag.getId();
				mediaTags.save();
			}
			ActiveAndroid.setTransactionSuccessful();
		}
		finally {
			ActiveAndroid.endTransaction();
		}
	}

	@Override
	public List<Media> getAllMedias() {
		return new Select().from(Media.class).execute();
	}

}
