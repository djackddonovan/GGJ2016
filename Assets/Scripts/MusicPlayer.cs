using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	AudioSource src;
	SoundManager mgr;

	void Awake () {
		src = GetComponent<AudioSource> ();
		mgr = GetComponent<SoundManager> ();
	}

	public void PlayMusic (string musicName) {
		mgr.PlaySound (src, musicName, false, true);
	}

}
