using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	bool isPaused {
		get {
			return Time.timeScale == 0f;
		}
	}

	[SerializeField]
	float quitTimerLimit = 3f;

	float quitTimer = 0f;

	[SerializeField]
	RectTransform pausePanel;

	void Update () {
		if (isPaused) {
			if (Input.GetButtonUp ("Pause"))
				SetPaused (false);

			if (Input.GetButton ("Pause"))
				quitTimer += Time.unscaledDeltaTime;
			
			if (quitTimer > quitTimerLimit) {
				SetPaused (false);
				SceneManager.LoadScene (0);
			}
		}
		else if (Input.GetButtonUp ("Pause"))
			SetPaused (true);
	}

	void SetPaused (bool value) {
		if (value)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;

		quitTimer = 0f;

		if (pausePanel != null)
			pausePanel.gameObject.SetActive (value);
	}

}
