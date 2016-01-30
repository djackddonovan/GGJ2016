using UnityEngine;

public class ItemSwapper : MonoBehaviour {

	[SerializeField]
	Camera nightCam;

	ItemSlot hovered = null;

	ItemSlot initial = null;

	void Awake () {
		if (nightCam == null)
			Debug.LogError ("Night cam not set on swapper");
	}

	void Update () {
		if (Time.timeScale == 0f)
			return;

		UpdateHovered ();

		if (Input.GetButtonDown ("Swap"))
			initial = hovered;

		if (Input.GetButtonUp ("Swap") &&
			initial != null &&
		    hovered != null &&
		    initial != hovered)
			initial.Swap (hovered);
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

	void OnDisable () {
		initial = null;
	}

	void OnEnable () {
		initial = null;
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

}
