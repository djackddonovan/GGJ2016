using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

	void Awake () {
		isDayTime = true;
		dayTimeProgress = 0f;

		if (startWithDay)
			StartDay ();
		else
			StartNight ();
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

	public void StartNight () {
		dayTimeProgress = 0f;

		onDayEnd.Invoke ();
		isDayTime = false;
		onNightStart.Invoke ();
	}

	public void StartDay () {
		++dayPassed;

		onNightEnd.Invoke ();
		isDayTime = true;
		onDayStart.Invoke ();
	}

}
