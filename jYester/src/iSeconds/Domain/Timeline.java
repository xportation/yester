package iSeconds.Domain;

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
	
	public void deleteAllVideos() {
		
	} 
	
	@Column(name = "Name")
	public String name;
	
	@Column(name = "UserId")
	public int userId;

	@Column(name = "Description")
	public String description;



}
