using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ItemSwapper : MonoBehaviour {

	[SerializeField]
	Camera nightCam;

	ItemSlot hovered = null;

	ItemSlot initial = null;

	public int maxSwapPerNight = 1;
	Dictionary<string, string> swapsThisNight = new Dictionary<string, string> ();

	[SerializeField]
	RectTransform warningPanel;
	[SerializeField]
	Text swapCounter;

	IEnumerator warningCoroutine = null;

	void Awake () {
		if (nightCam == null)
			Debug.LogError ("Night cam not set on swapper");
		if (warningPanel == null)
			Debug.LogWarning ("WarningPanel not set on swapper");
		if (swapCounter == null)
			Debug.LogWarning ("SwapCounter not set on swapper");
	}

	void Update () {
		UpdateHovered ();

		if (Input.GetButtonDown ("Swap")) {
			if (hovered != null && swapsThisNight.Count >= maxSwapPerNight)
				GiveWarning ("You have already used all of your swaps for this night.");
			else
				initial = hovered;
		}

		if (Input.GetButtonUp ("Swap") &&
		    initial != null &&
		    hovered != null &&
			initial != hovered &&
			initial.CanSwap (hovered)) {
			if (HasSwapBeenDoneThisNight (initial, hovered))
				GiveWarning ("You have already swapped those two items this night.");
			else {
				initial.Swap (hovered);
				swapsThisNight.Add (initial.slotName, hovered.slotName);
				swapCounter.text = "Swap done: " + swapsThisNight.Count + " / " + maxSwapPerNight;
			}
		}
	}

	void UpdateHovered () {
		var newHovered = GetHoveredSlot ();

		if (newHovered != hovered) {
			if (hovered != null)
				hovered.OnStopHover ();

			if (newHovered != null)
				newHovered.OnHover ();

			hovered = newHovered;
		}
	}

	public void Activate (bool value) {
		enabled = value;
		initial = null;
		swapsThisNight.Clear ();
		swapCounter.text = "Swap done: 0 / " + maxSwapPerNight;
	}

	ItemSlot GetHoveredSlot () {
		RaycastHit hit;

		Ray ray = nightCam.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast (ray, out hit)) {
			var item = hit.transform.GetComponentInParent<Item> ();

			if (item != null)
				return item.GetSlot ();
		}

		return null;
	}

	public bool TryStartDay () {
		if (swapsThisNight.Count >= maxSwapPerNight)
			return true;

		GiveWarning ("You have to use all of your swaps");
		return false;
	}

	void GiveWarning (string warning) {
		if (warningCoroutine != null)
			StopCoroutine (warningCoroutine);

		warningPanel.GetComponentInChildren<Text> ().text = warning;

		warningCoroutine = WarningCoroutine ();
		StartCoroutine (warningCoroutine);
	}

	IEnumerator WarningCoroutine () {
		CanvasGroup canvasGroup = warningPanel.GetComponentInChildren<CanvasGroup> ();

		float timer = 0f;
		while (timer < 2.5f) {
			yield return null;
			timer += Time.deltaTime;

			if (timer < 0.5f)
				canvasGroup.alpha = timer * 2f;
			else if (timer < 1.5f)
				canvasGroup.alpha = 1f;
			else
				canvasGroup.alpha = 2.5f - timer;
		}

		warningCoroutine = null;
	}

	bool HasSwapBeenDoneThisNight (ItemSlot slot1, ItemSlot slot2) {
		return swapsThisNight.ContainsKey (initial.slotName) &&
			swapsThisNight [initial.slotName] == hovered.slotName ||
			swapsThisNight.ContainsKey (hovered.slotName) &&
			swapsThisNight [hovered.slotName] == initial.slotName;
	}

	public void FinishNight () {
		if (TryStartDay ())
			GameObject.FindWithTag ("Managers").GetComponent<TimeLine> ().StartDay ();
	}

}
