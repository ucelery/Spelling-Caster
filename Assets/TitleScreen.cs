using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	[Header("BGM")]
	public GameObject bgm;
	public AudioClip startingClip;

	[Header("Text Effects")]
	public GameObject prompt;
	public float fadeSpeed = 0.05f;
	public float fadeDelay = 0f;
	private float fadeValue = 1f;

	private bool fading = true;
	private bool starting = false;
	private float startDelay = 3.5f;
	void Start() {
		
	}

	void Update() {
		HandleInputs();
		FadePrompt();
	}

	void HandleInputs() {
		if (Input.touchCount > 0) {
			Debug.Log(Input.touchCount);
			StartCoroutine(Starting());
		}

		if (Input.GetMouseButtonDown(0)) {
			StartCoroutine(Starting());
		}
	}

	void FadePrompt() {
		if (fading) {
			fadeValue -= fadeSpeed;
			if (fadeValue <= -0.5)
				fading = false;
		}
		else {
			fadeValue += fadeSpeed;
			if (fadeValue > 2)
				fading = true;

		}


		prompt.GetComponent<CanvasGroup>().alpha = fadeValue;

		if (Input.anyKey && !starting) {
			starting = true;
			fadeSpeed = 0.19f;
			bgm.GetComponent<AudioSource>().clip = startingClip;
			bgm.GetComponent<AudioSource>().Play();
			StartCoroutine(Starting());
		}
	}

	IEnumerator Starting() {
		yield return new WaitForSeconds(startDelay);
		SceneManager.LoadScene("MainSceneSample");
	}
}
