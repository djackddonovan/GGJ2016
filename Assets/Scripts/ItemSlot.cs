using UnityEngine;
using System.Collections;

public class ItemSlot : MonoBehaviour {

	Item currentItem;

	[SerializeField]
	string _slotName;
	public string slotName {
		get { return _slotName; }
		set { _slotName = value; }
	}

	void Awake () {
		currentItem = GetComponentInChildren<Item> ();
	}
	
	public string GetItemName () {
		if (currentItem == null)
			return "None";
		return currentItem.itemName;
	}

	public void Swap (ItemSlot other) {
		if (!CanSwap (other))
			return;

		var swapper = other.currentItem;
		other.currentItem = currentItem;
		currentItem = swapper;

		currentItem.transform.SetParent (transform, false);
		other.currentItem.transform.SetParent (other.transform, false);
	}

	public bool CanSwap (ItemSlot other) {
		return currentItem.category == other.currentItem.category;
	}

	public void OnHover () {

	}

	public void OnStopHover () {

	}
}
