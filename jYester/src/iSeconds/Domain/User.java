package iSeconds.Domain;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "User")
public class User extends Model { 
    @Column(name = "Name")
    public String name;
}

