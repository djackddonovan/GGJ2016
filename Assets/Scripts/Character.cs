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
	class slotStatus {
		public string slot = "";
		public string item = "";
	}

	[Serializable]
	class animInfos {
		public string animPlayed = "None";
		public List<slotStatus> slotConditions = new List<slotStatus> ();

		public float addedMadness = 0f;
		public float addedMadnessCap = 0f;
	}

	[Serializable]
	class CharacterAction {
		public string actionName = "";

		public float startTime = 0f;
		public float endTime = 1f;
		public int node = 0;

		public string defaultAnim = "None";
		public List<animInfos> anims = new List<animInfos> ();
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

	float madnessStatus = 0f;

	void Awake () {
		UpdateOrder ();
		CheckActionValidity ();

		timeLine = GameObject.FindWithTag ("Managers").GetComponent<TimeLine> ();

		if (path == null)
			Debug.LogError (gameObject.name + " doesn't have a path");

		anim = GetComponent<Animator> ();
	}

	void Update () {
		if (timeLine.isDayTime)
			UpdateDay ();
	}

	void UpdateDay () {
		anim.SetInteger ("MadnessStatus", (int)madnessStatus);

		float currentTime = timeLine.dayTimeProgress;

		if (actions.Count == 0)
			transform.position = path.GetPositionAfterIndice (0, currentTime);
		else if (currentActionIndice < actions.Count) {
			CharacterAction currentAction = actions [currentActionIndice];
			CharacterAction previousAction = currentActionIndice > 0 ? actions [currentActionIndice - 1] : null;

			if (!actionStarted) {
				MoveTowardCurrentAction (currentTime, currentAction, previousAction);

				if (currentTime > currentAction.startTime)
					StartAction (currentAction);

			} else if (currentTime > currentAction.endTime) {
				EndAction (currentAction);
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

	void StartAction (CharacterAction action) {
		#if UNITY_EDITOR
		print (gameObject.name + " starts to " + action.actionName);
		#endif

		anim.SetBool ("Walking", false);

		actionStarted = true;
	}

	void EndAction (CharacterAction action) {
		#if UNITY_EDITOR
		print (gameObject.name + " stops to " + action.actionName);
		#endif		

		anim.SetBool ("Walking", true);

		actionStarted = false;
	}

	public MadnessLevel GetMadnessLevel () {
		if (madnessStatus < .3f)
			return MadnessLevel.Calm;
		if (madnessStatus < .6f)
			return MadnessLevel.Concerned;
		if (madnessStatus < .9f)
			return MadnessLevel.Angry;
		return MadnessLevel.Mad;
	}

}
