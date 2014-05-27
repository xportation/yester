package iSeconds.Domain;

import java.util.List;

import iSeconds.Droid.ISecondsDb;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "User")
public class User extends Model {

	private IRepository repository = null;
	
	public EventSource onCurrentTimelineChanged = new EventSource();
	public EventSource onTimelineUpdated = new EventSource();

	public User(String name, IRepository repository) {
		this.name = name;
		this.currentTimelineId = -1;
		this.recordDuration = 3;
		this.tutorialShown = false;
		this.repository = repository;
	}

	public User() {
	}

	public Timeline createTimeline(String name, String description) {

		Timeline timeline = new Timeline(name, this.getId().intValue());
		timeline.description = description;
		timeline.setRepository(repository);

		repository.saveItem(timeline);

		// TODO: ver a property currentTimeline
		 if (currentTimelineId == -1)
			 this.setCurrentTimeline(timeline);

		return timeline;

	}

	public int getTimelineCount() {
		return getTimelines().size();
	}

	public List<Timeline> getTimelines() {
		return repository.getUserTimelines(this.getId().longValue());
	}

	public void deleteTimeline(Timeline timeline, boolean deleteVideos) {
		if (this.getId() == timeline.userId) { // ?
			if (deleteVideos)
				timeline.deleteAllVideos();

			repository.deleteTimeline(timeline);
		}

	}

	public Timeline getCurrentTimeline() {
		List<Timeline> timelines = getTimelines();
		for (Timeline timeline : timelines) {
			if (timeline.getId() == currentTimelineId)
				return timeline;
		}

		if (timelines.size() > 0) {
			currentTimelineId = timelines.get(0).getId().intValue();
			return timelines.get(0);
		}
		return null;
	}
	
	public void setCurrentTimeline(Timeline timeline) {
		
		if (timeline.getId() != currentTimelineId && timeline.userId == this.getId())
		{
			currentTimelineId = timeline.getId().intValue();
			repository.saveUser(this);

			// TODO: ver se precisa notificar ainda...
			onCurrentTimelineChanged.notify(this, timeline);
		} 	
	}

	public boolean updateTimeline(Timeline timeline) {
		if (timeline.userId != this.getId())
	         return false;
  
        repository.saveTimeline(timeline);
        
        onTimelineUpdated.notify(this, timeline);

        return true;
	}

	@Column(name = "Name")
	public String name;

	@Column(name = "CurrentTimelineId")
	public int currentTimelineId;

	@Column(name = "RecordDuration")
	public int recordDuration;

	@Column(name = "UsesOnlyDefaultVideo")
	public boolean usesOnlyDefaultVideo;

	@Column(name = "TutorialShown")
	public boolean tutorialShown;


}
