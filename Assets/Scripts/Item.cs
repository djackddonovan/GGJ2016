using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public enum Category {
		BigFurniture,
		SmallFurniture,
		Object,
		Wardrobe,
		Ground
	}

	[SerializeField]
	string _itemName;
	public string itemName {
		get { return _itemName; }
		set { _itemName = value; }
	}

	[SerializeField]
	Category _category;
	public Category category { get { return _category; } }


	public ItemSlot GetSlot () {
		return GetComponentInParent<ItemSlot> ();
	}

}
