using UnityEngine;
using System.Collections;

public class MenuAudioManager : MonoBehaviour {

	[SerializeField]
	float audioBalance = 1f;

	AudioSource src;

	void Awake () {
		src = GetComponent<AudioSource> ();
	}

	void Update () {
		src.volume = audioBalance * SoundManager.volume;
	}
}
