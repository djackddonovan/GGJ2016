using UnityEngine;
using System.Collections;

public class DayCamera : MonoBehaviour {

	[SerializeField]
	float depth = -.9f;

	Camera cam;

	void Awake () {
		cam = GetComponent<Camera> ();
		cam.depth = depth;
	}
	
	void Update () {
	
	}

	public void Activate () {
		gameObject.SetActive (true);
	}

	public void Deactivate () {
		gameObject.SetActive (false);
	}
}
