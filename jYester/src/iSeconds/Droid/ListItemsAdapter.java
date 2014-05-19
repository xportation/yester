package iSeconds.Droid;

import java.util.List;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;

public class ListItemsAdapter<ItemType> extends BaseAdapter {

	private int layoutId;
	private Context context;
	private List<ItemType> items;
	private ItemHolderFactory<ItemType> holderFactory;

	public ListItemsAdapter(Context context, List<ItemType> items, ItemHolderFactory<ItemType> holderFactory, int layoutId)
	{
		this.layoutId= layoutId;
		this.context= context;
		this.items= items;
		this.holderFactory= holderFactory;
	}
	
	public void setItems(List<ItemType> items) {
		this.items= items;
		this.notifyDataSetChanged();
	}
	
	@Override
	public int getCount() {
		if (items == null)
			return 0;

		return items.size();
	}

	@Override
	public Object getItem(int index) {
		return items.get(index);
	}

	@Override
	public long getItemId(int index) {
		return index;
	}

	@SuppressWarnings("unchecked")
	@Override
	public View getView(int index, View convertView, ViewGroup parent) {
		View view = convertView;
		ItemHolder<ItemType> itemHolder = null;
		if (convertView == null) {  
            LayoutInflater li = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);  
            view = li.inflate(layoutId, null);  
            itemHolder = holderFactory.Build(view);  
            view.setTag(itemHolder);  
		} else {  
			itemHolder = (ItemHolder<ItemType>) view.getTag();
		}  
		
		if (itemHolder != null)
			itemHolder.Update(items.get(index));
		
		return view;  
	}
}
