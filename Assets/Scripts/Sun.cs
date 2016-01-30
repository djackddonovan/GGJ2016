using UnityEngine;

public class Sun : MonoBehaviour {

	float time = 0f;

	Light sunLight;

	TimeLine timeLine;

	[SerializeField]
	float dayStartAngle = 0f;
	[SerializeField]
	float dayStartEndAngle = 0f;

	[SerializeField]
	float nightStartAngle = 60f;

	[SerializeField]
	Color dayColor = Color.yellow;
	[SerializeField]
	Color nightColor = Color.yellow;

	void Awake () {
		sunLight = GetComponent<Light> ();

		timeLine = GameObject.FindWithTag ("Managers").GetComponent<TimeLine> ();

		if (timeLine == null)
			print ("WARNING: Sun: TimeLine not set");
	}

	void Start () {
		if (timeLine.isDayTime)
			time = timeLine.dayTimeProgress;
		else
			time = -1f;
	}

	void Update () {
		if (time >= 0f) {
			time = timeLine.dayTimeProgress;

			float angle = Mathf.Lerp (dayStartAngle, dayStartEndAngle, time);
			SetSunAngle (angle);
		}			
	}

	public void SetDay () {
		time = 0f;
		SetSunAngle (dayStartAngle);
		sunLight.color = dayColor;
	}

	public void SetNight () {
		time = -1f;
		SetSunAngle (nightStartAngle);
		sunLight.color = nightColor;
	}

	void SetSunAngle (float angle) {
		Quaternion quat = transform.rotation;

		var euler = quat.eulerAngles;
		euler.x = angle;
		euler.y = 320f;
		euler.z = 0f;
		quat.eulerAngles = euler;

		transform.rotation = quat;
	}

}
