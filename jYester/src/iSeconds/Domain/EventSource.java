package iSeconds.Domain;

import java.util.ArrayList;
import java.util.List;

public class EventSource {
	
	public interface EventSourceListener {
		void handleEvent(Object sender, Object args);
	}
	
	private List<EventSourceListener> listeners = new ArrayList<EventSourceListener>();
	
	public void addListener(EventSourceListener eventSourceListener) {
		this.listeners.add(eventSourceListener);
	}
	
	public void notify(Object sender, Object args) {
		for (EventSourceListener l : listeners) {
			l.handleEvent(sender, args);
		}
	}
	
	

}
