package iSeconds.Domain;

import java.util.List;

public interface IRepository {

	void open();
	
	void addMediaTag(Media media);

	List<Media> getAllMedias();

}
