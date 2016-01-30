using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour {

	public void Play () {
		SceneManager.LoadScene (1);
	}

	public void Quit () {
		Application.Quit ();
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#endif
	}

}
