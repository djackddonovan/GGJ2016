using UnityEngine;
using System.Collections;

public class ItemSlot : MonoBehaviour {

	public Item currentItem { get; private set; }

	[SerializeField]
	string _slotName;
	public string slotName {
		get { return _slotName; }
		set { _slotName = value; }
	}

	void Awake () {
		currentItem = GetComponentInChildren<Item> ();
	}

	public void Swap (ItemSlot other) {
		if (!CanSwap (other))
			return;

		var swapper = other.currentItem;
		other.currentItem = currentItem;
		currentItem = swapper;

		RestoreItem ();
		other.RestoreItem ();
	}

	public void RestoreItem () {
		currentItem.transform.SetParent (transform);
		currentItem.transform.localPosition = currentItem.zeroPosition;
		currentItem.transform.localRotation = currentItem.zeroRotation;
	}

	public bool CanSwap (ItemSlot other) {
		return currentItem.category == other.currentItem.category;
	}

	public void OnHover (Color color) {
		SetOutlineColor (color);
	}

	public void OnStopHover () {
		SetOutlineColor (Color.clear);
	}

	public void OnSelectInitial (Color color) {
		SetOutlineColor (color);
	}

	void SetOutlineColor (Color color) {
		var meshRdrs = GetComponentsInChildren<MeshRenderer> ();
		if (meshRdrs.Length >= 2)
			meshRdrs[1].material.SetColor ("_Color", color);
	}

}
