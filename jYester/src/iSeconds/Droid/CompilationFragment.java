package iSeconds.Droid;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

public class CompilationFragment extends Fragment {

	private class CompilationItem {
		public String name;
		public String dateBegin;
		public String dateEnd;
		public String thumbnailPath;
		public String fileSize;
		public boolean isDone;
	}
		 
	class CompilationItemHolder implements ItemHolder<CompilationItem> {  
		private TextView name;  
		private TextView dateBegin;  
		private TextView dateEnd;  
		private ImageView thumbnail;
		private TextView fileSize;
		private ProgressBar progressBar;
		
		private AsyncImageLoader imageLoader;
		
	    public CompilationItemHolder(View base) {
	    	name = (TextView) base.findViewById(R.id.itemCompilationName);
	    	dateBegin = (TextView) base.findViewById(R.id.itemCompilationDateBegin);  
	    	dateEnd = (TextView) base.findViewById(R.id.itemCompilationDateEnd);  
	    	fileSize = (TextView) base.findViewById(R.id.itemCompilationSize);
	    	thumbnail = (ImageView) base.findViewById(R.id.itemCompilationThumbnail);
	    	progressBar = (ProgressBar) base.findViewById(R.id.itemCompilationSpin);
	    	
	    	imageLoader= new AsyncImageLoader();
	    } 
	    
	    public void Update(CompilationItem item) {
	    	name.setText(item.name);
	    	dateBegin.setText(item.dateBegin);
	    	dateEnd.setText(item.dateEnd);
	    	fileSize.setText(item.fileSize);
	    	if (item.thumbnailPath != null && !item.thumbnailPath.isEmpty())
	    		loadThumbnail(item.thumbnailPath);
	    	
	    	if (item.isDone)
	    		progressBar.setVisibility(View.GONE);
	    	else
	    		progressBar.setVisibility(View.VISIBLE);	    	
	    }

		private void loadThumbnail(String thumbnailPath) {
			imageLoader.load(thumbnailPath, thumbnail);
		}
	    
	}
	
	public class CompiationHolderFactory implements ItemHolderFactory<CompilationItem> {
		@Override
		public ItemHolder<CompilationItem> Build(View view) {
			return new CompilationItemHolder(view);
		}		
	}
	
	public CompilationFragment() {
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View rootView = inflater.inflate(R.layout.fragment_compilation,
				container, false);
		return rootView;
	}
}
