using UnityEngine;
using System.Collections.Generic;

public class SlotRecord : MonoBehaviour {

	class Entry {
		public Entry(string _entryName, ItemSlot _slot) {
			entryName = _entryName;
			slot = _slot;
		}

		public string entryName;
		public ItemSlot slot;
	}

	List<Entry> record;

	void Awake () {
		var allSlots = Object.FindObjectsOfType<ItemSlot> ();

		record = new List<Entry> ();
		foreach (var slot in allSlots)
			record.Add (new Entry(slot.slotName, slot));
	}

	public string GetSlotItemName (string slotName) {
		return record.Find (e => e.entryName == slotName).slot.GetItemName ();
	}

}
