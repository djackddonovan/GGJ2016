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

	public string actionSoundOverride = "None";

	[SerializeField]
	Category _category;
	public Category category { get { return _category; } }


	public ItemSlot GetSlot () {
		return GetComponentInParent<ItemSlot> ();
	}

	public bool OverrideSound (SoundManager soundMgr, AudioSource audioSrc, float length) {
		if (actionSoundOverride == "None")
			return false;

		soundMgr.PlaySoundForDurtation (audioSrc, actionSoundOverride, length);
		return true;
	}

}
