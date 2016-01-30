using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

public class SlotRecord : MonoBehaviour {

	[Serializable]
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

	public ItemSlot GetSlot (string slotName) {
		var entry = record.Find (e => e.entryName == slotName);
		if (entry == null)
			return null;
		return entry.slot;
	}

}
