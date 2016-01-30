using UnityEngine;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour {

	[Serializable]
	class SoundEntry {
		public string name = "";
		public AudioClip clip = null;
		[Range(0f, 1f)]
		public float audioBalance = 1f;
	}

	class PlayedSoundInfo {
		public bool timed = false;
		public bool stayWithTimeScale = true;
		public float duration = 0f;
		public float smooth = 0.5f;
		public float balance = 1f;
	}

	[SerializeField]
	List<SoundEntry> soundArchive;

	static float volume = 1f;

	Dictionary<AudioSource, PlayedSoundInfo> playingSources = new Dictionary<AudioSource, PlayedSoundInfo> ();

	void Update () {
		List<AudioSource> removeList = new List<AudioSource> ();

		foreach (var pair in playingSources) {
			var src = pair.Key;
			var info = pair.Value;

			info.duration -= Time.deltaTime;

			if (!src.isPlaying ||
				info.timed && info.duration < 0f) {
				removeList.Add (src);
				src.clip = null;
				continue;
			} 

			if (info.stayWithTimeScale)
				src.pitch = Time.timeScale;
			
			src.volume = volume * info.balance;

			if (info.timed && info.duration < info.smooth)
				src.volume *= Mathf.InverseLerp (0f, info.smooth, info.duration);
		}

		foreach (var removedValue in removeList)
			playingSources.Remove (removedValue);
	}

	public void PlaySound (AudioSource source, string soundName, bool affectedByPause = true, bool looped = false) {
		var entry = soundArchive.Find (e => e.name == soundName);
		if (entry == null || source == null)
			return;

		source.loop = looped;
		source.volume = entry.audioBalance * volume;
		source.clip = entry.clip;

		source.Play ();

		if (!playingSources.ContainsKey (source))
			playingSources.Add (source, new PlayedSoundInfo());

		playingSources [source].timed = false;
		playingSources [source].balance = entry.audioBalance;
		playingSources [source].stayWithTimeScale = affectedByPause;
	}

	public void PlaySoundForDurtation (AudioSource source, string soundName, float duration, float endSmooth = .5f) {
		var entry = soundArchive.Find (e => e.name == soundName);
		if (entry == null || source == null)
			return;

		print (duration);

		source.loop = true;
		source.volume = entry.audioBalance * volume;
		source.clip = entry.clip;

		source.Play ();

		if (!playingSources.ContainsKey (source))
			playingSources.Add (source, new PlayedSoundInfo());

		playingSources [source].timed = true;
		playingSources [source].balance = entry.audioBalance;
		playingSources [source].duration = duration;
		playingSources [source].smooth = endSmooth;
	}

}
