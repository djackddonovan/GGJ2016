using UnityEngine;
using System.Collections.Generic;
using System;

public class Character : MonoBehaviour {

	public enum MadnessLevel {
		Calm = 0,
		Concerned = 1,
		Angry = 2,
		Mad = 3
	}

	[Serializable]
	class CharacterAction {
		public string actionName = "";

		public float startTime = 0f;
		public float endTime = 1f;
		public int node = 0;

		public string itemSlot = "None";
		public bool takeItemInHolder = true;
		public string animPlayed = "None";
		public string soundPlayed = "None";
	}

	public RailPath path;

	TimeLine timeLine;

	Animator anim;

	[SerializeField]
	List<CharacterAction> actions;

	#if UNITY_EDITOR
	[InspectorButton("UpdateOrder")]
	public bool updateOrder;
	#endif

	int currentActionIndice; //also design the next action to perform is none is currently being done
	bool actionStarted;

	float madness = 0f;

	SlotRecord slotRecord;

	[SerializeField]
	Transform itemHolder;

	[SerializeField, Tooltip("0: calm, 1: concerned, 2: angry, 3: mad")]
	Material[] materials;

	MadnessConsequencesTable csqTable;

	SoundManager soundMgr;
	AudioSource[] soundSrc;
	[SerializeField]
	string[] soundsMadnessConsequences = new string[4] { "None", "None", "None", "None" };

	void Awake () {
		UpdateOrder ();
		CheckActionValidity ();

		var managers = GameObject.FindWithTag ("Managers");
		timeLine = managers.GetComponent<TimeLine> ();
		slotRecord = managers.GetComponent<SlotRecord> ();
		csqTable = managers.GetComponent<MadnessConsequencesTable> ();
		soundMgr = managers.GetComponent<SoundManager> ();

		if (path == null)
			Debug.LogError (gameObject.name + " doesn't have a path");
		if (itemHolder == null)
			Debug.LogError (gameObject.name + " doesn't have an item holder");
		if (materials.Length < 4)
			Debug.LogWarning (gameObject.name + " doesn't have enough materials");
		if (soundsMadnessConsequences.Length < 4)
			Debug.LogWarning (gameObject.name + " doesn't have enough sounds");

		anim = GetComponent<Animator> ();
		soundSrc = GetComponents<AudioSource> ();
	}

	void Update () {
		if (timeLine.isDayTime)
			UpdateDay ();
	}

	void UpdateDay () {
		anim.SetInteger ("MadnessStatus", (int)madness);

		float currentTime = timeLine.dayTimeProgress;

		if (actions.Count == 0)
			transform.position = path.GetPositionAfterIndice (0, currentTime);
		else if (currentActionIndice < actions.Count) {
			CharacterAction currentAction = actions [currentActionIndice];
			CharacterAction previousAction = currentActionIndice > 0 ? actions [currentActionIndice - 1] : null;

			if (!actionStarted) {
				MoveTowardCurrentAction (currentTime, currentAction, previousAction);

				if (currentTime > currentAction.startTime)
					SetAction (currentAction, true);

			} else if (currentTime > currentAction.endTime) {
				SetAction (currentAction, false);
				++currentActionIndice;
			}
		}
	}

	void MoveTowardCurrentAction (float currentTime, CharacterAction current, CharacterAction previous) {
		if (current.node == 0) {
			transform.position = path.GetPositionAfterIndice(previous == null ? 0 : previous.node,
				Mathf.InverseLerp(previous == null ? 0f : previous.endTime, current.startTime, currentTime));

			return;
		}

		float moveStart = 0f;
		if (previous != null)
			moveStart = previous.endTime;

		float moveEnd = current.startTime;

		transform.position = path.GetPositionBetweenIndices(previous == null ? 0 : previous.node,
															current.node,
															Mathf.InverseLerp(moveStart, moveEnd, currentTime));
	}

	public void StartDayRoutine () {
		currentActionIndice = 0;
		actionStarted = false;
	}

	public void StartSleeping () {
		transform.position = path.GetStart ();
	}

	void CheckActionValidity () {
		var previousAction = actions [0];

		for (int i = 1; i < actions.Count; ++i) {
			var action = actions [i];

			if (i != 0 &&
				action.startTime < previousAction.endTime) {
				Debug.LogError (previousAction.actionName + " go past " + action.actionName + " starting time");
				return;
			}

			if (action.endTime > 1f) {
				Debug.LogError (action.actionName + " go past the end of day");
				return;
			}

			if (action.startTime <= 0f)
				Debug.LogWarning (action.actionName + " start before start of day");
			if (action.endTime - action.startTime <= 0f)
				Debug.LogWarning (action.actionName + " doesn't have a proper duration");

			previousAction = action;
		}
	}

	void UpdateOrder () {
		actions.Sort ((a, b) => a.startTime.CompareTo(b.startTime));
	}

	void SetAction (CharacterAction action, bool start) {
		#if UNITY_EDITOR
		if (start)
			print (gameObject.name + " starts to " + action.actionName);
		else
			print (gameObject.name + " stops to " + action.actionName);
		#endif

		if (start)
			anim.ResetTrigger ("Walk");
		else
			anim.SetTrigger ("Walk");

		if (action.takeItemInHolder && action.itemSlot != "None")
			SetItemSlotHolded (action.itemSlot, start);

		Item item = null;
		var slot = slotRecord.GetSlot (action.itemSlot);
		if (slot != null)
			item = slot.currentItem;

		if (start) {
			if (action.animPlayed != "None")
				anim.SetTrigger (action.animPlayed);
		}

		MadnessConsequencesTable.Severity severity = MadnessConsequencesTable.Severity.None;
		if (!start && item != null) {
			severity = csqTable.GetSwapSeverity (action.itemSlot, item.itemName);
			GainMadness (csqTable.SeverityToMadnessAmount (severity));
		}

		PlayActionSound (action, start, item, severity);

		actionStarted = start;
	}

	public MadnessLevel GetMadnessLevel () {
		if (madness < .3f)
			return MadnessLevel.Calm;
		if (madness < .6f)
			return MadnessLevel.Concerned;
		if (madness < 1f)
			return MadnessLevel.Angry;
		return MadnessLevel.Mad;
	}

	void SetItemSlotHolded (string slotName, bool use) {
		var slot = slotRecord.GetSlot (slotName);

		if (use) {
			var item = slot.currentItem;
			item.transform.SetParent (itemHolder);
			item.transform.localPosition = Vector3.zero;
		} else
			slot.RestoreItem ();
	}

	void GainMadness (float addedValue) {
		var previousLevel = GetMadnessLevel ();
		madness += addedValue;

		var newLevel = GetMadnessLevel ();
		if (previousLevel != newLevel)
			GetComponentInChildren<MeshRenderer> ().material = materials[(int)newLevel];
	}

	void PlayActionSound (CharacterAction action, bool atStart, Item item, MadnessConsequencesTable.Severity madnessSeverity) {
		if (atStart) {
			if (item != null &&
				item.OverrideSound (soundMgr, soundSrc [0], (action.endTime - action.startTime) / timeLine.speed))
				return;

			if (action.soundPlayed != "None")
				soundMgr.PlaySoundForDurtation (soundSrc [0], action.soundPlayed, (action.endTime - action.startTime) / timeLine.speed);
		} else {
			string madsound = soundsMadnessConsequences[(int)madnessSeverity];
			if (madsound != "None")
				soundMgr.PlaySound (soundSrc [1], madsound);
		}
	}

}