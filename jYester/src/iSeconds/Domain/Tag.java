package iSeconds.Domain;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "Tags")
public class Tag extends Model 
{
	@Column(name = "Name")
	private String name;

	public void setName(String name) {
		this.name = name;		
	}

}
