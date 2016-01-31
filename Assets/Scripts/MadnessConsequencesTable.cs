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
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3
	}

	[Serializable]
	class SwapSeverity {
		public SwapSeverity (string _slot, string _item, Severity _severity) {
			slot = _slot;
			item = _item;
			severity = _severity;
		}

		public string slot = "None";
		public string item = "None";
		public Severity severity = Severity.None;
	}

	List<SwapSeverity> consequences = new List<SwapSeverity> ();

	void Awake () {
		GenerateConsequences ();
	}

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

	string[][] namesInTables = new string[][] {
		new string[] { "BeignoireVide", "BeignoirePleine", "Canape", "Lit", "MeubleVaisseille" },
		new string[] { "PQ", "BrosseADent", "RougeALevre", "ArmeAFeu", "Cactus", "RailCoke", "Magazine", "Cigario", "TasseDeThe", "Banane", "Orange", "Reveil", "Pantoufle", "Livre", "Poele", "AssieteNouriture", "Assiete", "Balai", "Mixer", "TV" },
		new string[] { "WC", "Lavabo", "Fauteuil", "MeubleTV", "TableDeChevet", "EvierRobinet", "Four", "Chaise", "Chemine" },
		new string[] { "ArmoireSdb", "Bibliotheque", "Armoire", "Frigo", "Placard" },
		new string[] { "TapisDouche", "Tapis" }
	};

	int[][] severityTables = new int[][] { //each row are the item values of one slot
		new int[] { 0, 0, 1, 1, 1,
					0, 0, 1, 1, 1,
					2, 3, 0, 1, 2,
					1, 3, 1, 0, 2,
					1, 1, 1, 1, 0 },
		new int[] { 0, 2, 2, 3, 3, 2, 1, 2, 3, 2, 2, 1, 1, 1, 2, 2, 2, 2, 3, 2,
			 	    1, 0, 2, 3, 3, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 2, 3, 2,
				    1, 1, 0, 3, 3, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 2, 2, 1, 3, 2, 
				    1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				    1, 1, 1, 1, 0, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1,
				    2, 2, 2, 3, 3, 0, 1, 2, 2, 1, 1, 1, 1, 1, 2, 2, 2, 1, 3, 1,
				    1, 1, 1, 1, 3, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 1,
				    2, 2, 2, 3, 3, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 1, 
				    1, 1, 2, 3, 3, 2, 1, 2, 0, 1, 1, 1, 2, 1, 2, 1, 2, 2, 3, 2,
				    2, 1, 1, 3, 3, 2, 1, 2, 2, 0, 1, 2, 2, 2, 2, 1, 2, 2, 3, 2,
				    2, 1, 1, 2, 2, 3, 1, 2, 1, 1, 0, 1, 2, 2, 2, 1, 2, 2, 3, 2,
				    1, 1, 2, 2, 3, 2, 1, 2, 2, 2, 2, 0, 1, 1, 2, 2, 2, 1, 3, 1,
				    1, 1, 2, 3, 3, 2, 1, 2, 2, 2, 2, 1, 0, 1, 2, 2, 2, 1, 3, 2,
				    1, 1, 1, 3, 3, 2, 1, 2, 2, 1, 1, 1, 2, 0, 2, 2, 2, 1, 3, 1,
				    2, 1, 1, 3, 3, 3, 1, 3, 1, 1, 1, 1, 1, 1, 0, 1, 2, 1, 3, 2,
				    2, 1, 1, 3, 3, 3, 1, 2, 1, 1, 1, 1, 1, 1, 1, 0, 2, 1, 2, 2,
				    1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 1, 2, 2,
				    2, 2, 1, 3, 1, 3, 2, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 2,
				    2, 1, 1, 3, 3, 3, 1, 3, 1, 1, 1, 3, 3, 1, 3, 2, 2, 3, 0, 2,
				    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 3, 0},
		new int[] { 0, 1, 2, 2, 1, 1, 3, 1, 3,
				    1, 0, 1, 2, 1, 1, 2, 2, 2,
				    1, 1, 0, 2, 1, 1, 2, 1, 3,
				    2, 1, 2, 0, 1, 1, 2, 1, 1,
				    2, 1, 1, 1, 0, 1, 2, 1, 1,
				    1, 1, 2, 2, 1, 0, 1, 2, 2,
				    3, 2, 1, 3, 1, 2, 0, 2, 1,
				    2, 1, 1, 2, 1, 1, 1, 0, 3,
				    1, 1, 1, 1, 1, 1, 1, 1, 0 },
		new int[] { 0, 1, 1, 1, 1,
	  			    1, 0, 1, 1, 1,
				    1, 1, 0, 1, 1,
				    1, 1, 1, 0, 1,
				    1, 1, 1, 1, 0 },
		new int[] { 0, 1,
		 		    1, 0 }
	};

	void GenerateConsequences () {
		consequences.Clear ();

		for (int i = 0; i < severityTables.Length; ++i) {
			var namesInTable = namesInTables [i];
			var severityTable = severityTables [i];

			int namecount = namesInTables[i].Length;

			for (int j = 0; j < namecount; ++j) { //row
				for (int k = 0; k < namecount; ++k) { //column
					consequences.Add (new SwapSeverity (namesInTable[j], namesInTable[k], (Severity)severityTable[j * namecount + k]));
				}
			}
		}
	}

}
