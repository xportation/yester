package iSeconds.Droid;

import android.view.View;

public interface ItemHolderFactory<ItemType> {

	ItemHolder<ItemType> Build(View view);
}
