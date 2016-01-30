using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class NightCamera : MonoBehaviour {

	[SerializeField]
	float depth = -1f;

	Camera cam;

	BlurOptimized blur;

	void Awake () {
		cam = GetComponent<Camera> ();
		cam.depth = depth;

		blur = GetComponent<BlurOptimized> ();
	}
	
	void Update () {
	
	}

	public void Activate () {
		blur.enabled = false;
	}

	public void Deactivate () {
		blur.enabled = true;
	}

}
