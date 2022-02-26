using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	[Header("BGM")]
	public GameObject bgm;
	public AudioClip endClip;

	[Header("Text Effects")]
	public GameObject prompt;
	public float fadeSpeed = 0.05f;
	public float fadeDelay = 0f;
	private float fadeValue = 1f;

	private bool fading = true;
	private float startDelay = 3.5f;

	private bool donePlaying = false;
	void Start()
	{

	}

	void Update()
	{
		if (fading)
		{
			fadeValue -= fadeSpeed;
			if (fadeValue <= -0.5)
				fading = false;
		}
		else
		{
			fadeValue += fadeSpeed;
			if (fadeValue > 2)
				fading = true;

		}

		prompt.GetComponent<CanvasGroup>().alpha = fadeValue;

		if (!bgm.GetComponent<AudioSource>().isPlaying && !donePlaying) {
			donePlaying = true;
			bgm.GetComponent<AudioSource>().clip = endClip;
			bgm.GetComponent<AudioSource>().Play();
		}
	}

	IEnumerator Starting()
	{
		yield return new WaitForSeconds(startDelay);
		// Change To Main Scene
	}
}
