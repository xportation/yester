package iSeconds.Domain;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "Timeline")
public class Timeline extends Model {
	
	private IRepository repository = null;
	
	public Timeline(String name, int userId) {
		this.name = name;
		this.userId = userId;
	}
	public Timeline() {}
	
	public void setRepository(IRepository repository) {
		this.repository = repository;
	}
	
	public void addVideoAt(Date date, String url) {
		
		assert(repository != null);

		DayInfo day = repository.getDayInfoAt(date, this.getId());
		day.addVideo(url);
	}
    
	public void deleteAllVideos() {
		
	} 
	
	public List<MediaInfo> getVideosAt(Date date) {
		assert(repository != null);

		DayInfo day = repository.getDayInfoAt(date, this.getId());
		return day.getVideos();
	}
	
	public List<DayInfo> getDays() {
		return repository.getAllDays(this.getId());
	}

//	public List<MediaInfo> getAllVideos() {
//		assert(repository != null);
//		
//		List<MediaInfo> medias= new ArrayList<MediaInfo>();
//		List<DayInfo> days= repository.getDays(this.getId());
//		for (DayInfo day: days) {
//			medias.addAll(day.getVideos());
//		}
//		
//		return medias;
//	}
	
	public List<DayInfo> getAllVideos() {
		assert(repository != null);		
		return repository.getDays(this.getId());
	}
	
	@Column(name = "Name")
	public String name;
	
	@Column(name = "UserId")
	public int userId;

	@Column(name = "Description")
	public String description;


}
