package iSeconds.Domain;

import java.util.Date;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "MediaInfo")
public class MediaInfo extends Model {

	public MediaInfo(long dayId, String url, Date date) {
		this.dayId = dayId;
		this.path = url;
//		this.timeOfDay = date;
	}
	
	public MediaInfo() {}

	public String getThumbnailPath() {
		if (path == null)
			return new String();
		
		return removeExtension(path) + ".png";
	}
	
	//TODO extrair isso.
	public String removeExtension(String filename) {
		int extensionIndex = filename.lastIndexOf(".");
	    if (extensionIndex == -1)
	        return filename;

	    return filename.substring(0, extensionIndex);
	}
	
	@Column(name = "DayId")
	public long dayId; 
	
	// TODO: ver o tamanho para essa string
	@Column(name = "Path")
	public String path;
	
	// TODO: ver como tratar issokkkk 
//	@Column(name = "TimeOfDay")
//	public TimeSpan timeOfDay { get; set; }

}
