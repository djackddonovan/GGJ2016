using UnityEngine;
using System;
using System.Collections.Generic;

public class MadnessConsequencesTable : MonoBehaviour {

	[SerializeField]
	float lowSeverityAmount = 0.008f;
	[SerializeField]
	float mediumSeverityAmount = 0.019f;
	[SerializeField]
	float highSeverityAmount = 0.035f;

	public enum Severity {
		None,
		Low,
		Medium,
		High
	}

	[Serializable]
	class SwapSeverity {
		public string slot = "None";
		public string item = "None";
		public Severity severity = Severity.None;
	}

	[SerializeField]
	List<SwapSeverity> consequences;

	public Severity GetSwapSeverity (string slot, string item) {
		if (slot == "None" || item == "None")
			return Severity.None;
		
		var slotConsequences = consequences.FindAll (c => c.slot == slot);
		if (slotConsequences.Count == 0)
			return Severity.None;

		var slotItemConsequence = slotConsequences.Find (c => c.item == item);
		if (slotItemConsequence == null)
			return Severity.None;
		return slotItemConsequence.severity;
	}

	public float GetSwapMadnessAmount (string slot, string item) {
		return SeverityToMadnessAmount (GetSwapSeverity (slot, item));
	}

	public float SeverityToMadnessAmount (Severity severity) {
		switch (severity) {
		case Severity.Low:
			return lowSeverityAmount;
		case Severity.Medium:
			return mediumSeverityAmount;
		case Severity.High:
			return highSeverityAmount;
		}
		return 0f;
	}

}
