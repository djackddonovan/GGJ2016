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
	List<KeyValuePair<string, string>> swapsThisNight = new List<KeyValuePair<string, string>> ();

	[SerializeField]
	RectTransform warningPanel;
	[SerializeField]
	Text swapCounter;

	IEnumerator warningCoroutine = null;

	[SerializeField]
	Color hoverColor = Color.white;
	[SerializeField]
	Color selectedColor = Color.green;

	[SerializeField]
	string hoverSound = "SwapHover";
	[SerializeField]
	string initialSelectSound = "SwapSelect";
	[SerializeField]
	string failSound = "SwapFail";
	[SerializeField]
	string swapSound = "SwapDone";

	SoundManager soundMgr;

	[SerializeField]
	AudioSource soundSrc;

	void Awake () {
		if (nightCam == null)
			Debug.LogError ("Night cam not set on swapper");
		if (warningPanel == null)
			Debug.LogWarning ("WarningPanel not set on swapper");
		if (swapCounter == null)
			Debug.LogWarning ("SwapCounter not set on swapper");;
		if (soundSrc == null)
			Debug.LogWarning ("AudioSource swapper");

		soundMgr = GetComponent<SoundManager> ();
	}

	void Update () {
		UpdateHovered ();

		if (Input.GetButtonDown ("Swap")) {
			if (hovered != null && swapsThisNight.Count >= maxSwapPerNight)
				GiveWarning ("You have already used all of your swaps for this night.");
			else {
				if (initial != null)
					initial.OnStopHover ();
				initial = hovered;
				if (initial != null) {
					initial.OnSelectInitial (selectedColor);
					soundMgr.PlaySound (soundSrc, initialSelectSound);
				}
			}
		}

		if (Input.GetButtonUp ("Swap")) {
			if (initial != null && hovered != null && initial != hovered) {

				if (HasSwapBeenDoneThisNight (initial, hovered)) {
					GiveWarning ("You have already swapped those two items this night.");
					initial.OnStopHover ();
					soundMgr.PlaySound (soundSrc, failSound);
				} else {
					initial.Swap (hovered);
					swapsThisNight.Add (new KeyValuePair<string, string> (initial.currentItem.name, hovered.currentItem.name));
					swapCounter.text = "Swap done: " + swapsThisNight.Count + " / " + maxSwapPerNight;

					hovered.OnHover (hoverColor);
					initial.OnStopHover ();
					initial = null;
					soundMgr.PlaySound (soundSrc, swapSound);
				}
			} else if (initial != null) {
				initial.OnStopHover ();
				if (initial == GetHoveredSlot ()) {
					hovered = initial;
					hovered.OnHover (hoverColor);
				}
				initial = null;
				soundMgr.PlaySound (soundSrc, failSound);
			}
		}
	}

	void UpdateHovered () {
		var newHovered = GetHoveredSlot ();

		if (newHovered != hovered) {
			if (hovered != null && hovered != initial)
				hovered.OnStopHover ();

			if (newHovered != null &&
			    (initial == null || initial.CanSwap (newHovered))) {
				newHovered.OnHover (hoverColor);
				soundMgr.PlaySound (soundSrc, hoverSound);
			}

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
		string name1 = slot1.currentItem.name;
		string name2 = slot2.currentItem.name;

		return swapsThisNight.Exists (p => p.Key == name1 && p.Value == name2 || p.Key == name2 && p.Value == name1);
	}

	public void FinishNight () {
		if (TryStartDay ())
			GameObject.FindWithTag ("Managers").GetComponent<TimeLine> ().StartDay ();
	}

}
