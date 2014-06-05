package iSeconds.Domain;

import java.util.Date;
import java.util.List;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "DayInfo")
public class DayInfo extends Model {
	
	private IRepository repository = null;

	public DayInfo(Date date, long timelineId) {
		this.date = SqlUtils.formatToSqliteDate(date);
		this.timelineId = timelineId;
		this.defaultVideoId = -1;
	}
	
	public DayInfo() {}

	public void setRepository(IRepository repository) {
		this.repository = repository;
	}
	
	public List<MediaInfo> getVideos() {
		return this.repository.getMediasForDay(this);
	}
	
	public void addVideo(String url) {
		assert(repository != null); // you should bind a repository with SetRepository() method

        // TODO: ver se isso basta.. salvamos o dia apenas se ele tiver video
        this.repository.saveItem(this);

        MediaInfo media = new MediaInfo(this.getId(), url, new Date());
        this.repository.saveItem(media);

        this.defaultVideoId = media.getId();

			// TODO: ugly, salvando duas vezes. na primeira vez temos que salvar para associar um Id a esse dia (usado no MediaInfo depois)
			// na segunda vez temos que persistir o ChoosedMediaId que sera o id associado ao MediaInfo quando salvo no banco (dependencia ciclica..)
        this.repository.saveItem(this); 
	}
	
	public Date getDate() {
		try {
			return SqlUtils.parseDate(date);
		} catch (Exception e) {
			return null;
		}
	}
	
	public void setDate(Date d) {
		this.date = SqlUtils.formatToSqliteDate(d);
	}
	
	@Column(name = "TimelineId")
	public long timelineId;
	
	@Column(name = "Date")
	private String date;
	
	@Column(name = "DefaultVideoId")
	public long defaultVideoId;




}
