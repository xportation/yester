package iSeconds.Droid;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.AbsListView;
import android.widget.GridView;
import android.widget.ListAdapter;

public class UnscrollableGridView extends GridView 
{

	private int numColumns= 3;
	private int verticalSpacing= 1;
	private int horizontalSpacing= 1;

	public UnscrollableGridView(Context context) {
		super(context);
	}
	
	public UnscrollableGridView(Context context, AttributeSet attrs) {
		super(context, attrs);
	}
	
	public UnscrollableGridView(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	@Override
	public int getColumnWidth() {
		final int totalHorizontalSpacing = numColumns > 0 ? (numColumns - 1) * horizontalSpacing : 0;
	    return (getMeasuredWidth() - getPaddingLeft() - getPaddingRight() - totalHorizontalSpacing) / numColumns;
	}
	 
	@Override
	protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
	    // Sets the padding for this view.
	    super.onMeasure(widthMeasureSpec, heightMeasureSpec);
	 
	    final int measuredWidth = getMeasuredWidth();
	    final int childWidth = getColumnWidth();
	    int childHeight = 0;
	 
	    // If there's an adapter, use it to calculate the height of this view.
	    final ListAdapter adapter = getAdapter();
	    final int count;
	 
	    // There shouldn't be any inherent size (due to padding) if there are no child views.
	    if (adapter == null || (count = adapter.getCount()) == 0) {
	        setMeasuredDimension(0, 0);
	        return;
	    }
	 
	    // Get the first child from the adapter.
	    final View child = adapter.getView(0, null, this);
	    if (child != null) {
	        // Set a default LayoutParams on the child, if it doesn't have one on its own.
	        AbsListView.LayoutParams params = (AbsListView.LayoutParams) child.getLayoutParams();
	        if (params == null) {
	            params = new AbsListView.LayoutParams(AbsListView.LayoutParams.WRAP_CONTENT,
	                                                  AbsListView.LayoutParams.WRAP_CONTENT);
	            child.setLayoutParams(params);
	        }
	 
	        // Measure the exact width of the child, and the height based on the width.
	        // Note: the child takes care of calculating its height.
	        int childWidthSpec = MeasureSpec.makeMeasureSpec(childWidth, MeasureSpec.EXACTLY);
	        int childHeightSpec = MeasureSpec.makeMeasureSpec(0,  MeasureSpec.UNSPECIFIED);
	        child.measure(childWidthSpec, childHeightSpec);
	        childHeight = child.getMeasuredHeight();
	    }
	 
	    // Number of rows required to 'mTotal' items.
	    final int rows = (int) Math.ceil((double) count / numColumns);
	    final int childrenHeight = childHeight * rows;
	    final int totalVerticalSpacing = rows > 0 ? (rows - 1) * verticalSpacing : 0;
	 
	    // Total height of this view.
	    final int measuredHeight = childrenHeight + getPaddingTop() + getPaddingBottom() + totalVerticalSpacing;
	    setMeasuredDimension(measuredWidth, measuredHeight);
	}
}
