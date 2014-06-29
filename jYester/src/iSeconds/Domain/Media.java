package iSeconds.Domain;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "Medias")
public class Media extends Model
{
	@Column(name = "Date")
	private Date date;
	
	@Column(name = "Path")
	private String path;
	
//	@Column(name = "Time")
//	private long time;

	private List<Tag> tags = new ArrayList<Tag>();
	
	public void setDate(Date date) {
		this.date = date;		
	}

//	public void setTime(long time) {
//		this.time = time;
//	}

	public void setPath(String path) {
		this.path = path;		
	}

	public void addTag(Tag tag) {
		tags.add(tag);
	}

	public int tagsCount() {
		return tags.size();
	}

	public Tag tagAt(int index) {
		if (index >=  tags.size())
			return null;
		
		return tags.get(index);
	}

	public String getThumbnailPath() {
		if (path == null)
			return new String();
		
		return removeExtension(path) + ".png";
	}
	
	//TODO extrair isso.
	private String removeExtension(String filename) {
		int extensionIndex = filename.lastIndexOf(".");
	    if (extensionIndex == -1)
	        return filename;

	    return filename.substring(0, extensionIndex);
	}
	
	public Date getDate() {
//		try {
//			return SqlUtils.parseDate(date);
//		} catch (Exception e) {
//			return null;
//		}
		return date;
	}

	public String getVideoPath() {
		return path;
	}
	
}
