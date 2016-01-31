using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TimeLine : MonoBehaviour {

	public float dayTimeProgress { get; private set; }
	public bool isDayTime { get; private set; }
	int dayPassed = 0;

	[Tooltip("In progress per second")]
	public float speed = 0.025f;

	[SerializeField]
	bool startWithDay = true;

	[SerializeField]
	Slider timeSlider;

	public UnityEvent onDayStart;
	public UnityEvent onDayEnd;

	public UnityEvent onNightStart;
	public UnityEvent onNightEnd;

	[SerializeField]
	Text dayHeader;
	[SerializeField]
	Text nightHeader;

	public float fadingTime = 2f;
	[SerializeField]
	Image fadePanel;

	bool inTransition = false;

	void Awake () {
		isDayTime = true;
		dayTimeProgress = 0f;
	}

	void Start () {
		if (startWithDay)
			StartDay (false, false);
		else
			StartNight (false);
	}

	void Update () {
		if (!isDayTime)
			return;

		dayTimeProgress += Time.deltaTime * speed;

		if (dayTimeProgress > 1f)
			StartNight ();
		else if (timeSlider != null)
			timeSlider.value = dayTimeProgress;
	}

	public void StartNight (bool doFade = true) {
		if (inTransition)
			return;
		
		StartCoroutine (StartNightCoroutine (doFade));
	}

	public void StartDay (bool doFade = true, bool checkSwap = true) {
		if (checkSwap && !GetComponent<ItemSwapper> ().TryStartDay () || inTransition)
			return;
		
		StartCoroutine (StartDayCoroutine (doFade));
	}

	public void StartDayButton () {
		StartDay ();
	}

	IEnumerator StartNightCoroutine (bool doFade) {
		inTransition = true;

		if (doFade)
			yield return StartCoroutine (FadeToBlack ());

		onDayEnd.Invoke ();
		dayTimeProgress = 0f;
		isDayTime = false;
		onNightStart.Invoke ();

		if (nightHeader != null)
			nightHeader.text = "Night " + dayPassed;

		if (doFade)
			yield return StartCoroutine (FadeFromBlack ());

		inTransition = false;
	}

	IEnumerator StartDayCoroutine (bool doFade) {
		inTransition = true;

		if (doFade)
			yield return StartCoroutine (FadeToBlack ());

		onNightEnd.Invoke ();
		++dayPassed;
		isDayTime = true;
		onDayStart.Invoke ();

		if (dayHeader != null)
			dayHeader.text = "Morning " + dayPassed;

		if (doFade)
			yield return StartCoroutine (FadeFromBlack ());

		inTransition = false;
	}

	IEnumerator FadeToBlack () {
		if (fadePanel != null) {
			float halfFadeTime = fadingTime * .5f;

			float timer = halfFadeTime;

			while (timer > 0f) {
				yield return null;
				fadePanel.color = new Color (0f, 0f, 0f, 1f - timer / halfFadeTime);
				timer -= Time.deltaTime;
			}
		}
	}

	IEnumerator FadeFromBlack () {
		if (fadePanel != null) {
			float halfFadeTime = fadingTime * .5f;

			float timer = halfFadeTime;

			while (timer > 0f) {
				yield return null;
				fadePanel.color = new Color (0f, 0f, 0f, timer / halfFadeTime);
				timer -= Time.deltaTime;
			}
		}
	}

}
