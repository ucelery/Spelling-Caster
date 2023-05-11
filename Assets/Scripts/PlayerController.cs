using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	// ANIMATIONS
	private string PLAYER_RUN_LEFT = "RunningLeft";
	private string PLAYER_RUN_RIGHT = "Running";
	private string PLAYER_IDLE = "Idle";
	private string PLAYER_DEATH_ANIM = "Death";

	[Header("Player Stats")]
	public float maxHitpoints = 100f;
	public float hitpoints;
	public float damage = 10f;
	public float speed = 10f;
	public float deathDelay = 1f;

	public bool isAlive = true;

	[Header("Required GameObjects")]
	public Rigidbody2D rb;
	public GameObject textObj;
	public GameObject projectile;
	public Slider slider;
	public Animator anim;

	// public GameObject debugX;

	[SerializeField]
	[TextArea]
	private string[] spellBank;

    private string[] wordBank;

    private string[] fileLines;
	private string remainingWord = string.Empty;
	private string currentWord = string.Empty;
    private int currWordIndex = 0;

	private float accuracy = 0;

    [Header("Misc")]
	public float slowdownFactor = 0.5f;

	TouchScreenKeyboard onScreenKb;
	private float dirX;

	void Start() {
		if (spellBank.Length < 1) Debug.LogWarning("Empty spell bank");

        hitpoints = maxHitpoints;
		slider.maxValue = maxHitpoints;
		slider.value = hitpoints;

        Debug.Log(wordBank);
        wordBank = GetSpell().Split(' ');
		Debug.Log(wordBank[0]);

        currentWord = wordBank[UnityEngine.Random.Range(0, wordBank.Length)];
		
		SetCurrentWord();

		if (SystemInfo.deviceType == DeviceType.Handheld) {
			speed *= 1.5f;
		}
	}

	void Update() {
		if (isAlive) {
			CheckInput();
			Animate();
		}
	}

	private void Animate() {
		if (SystemInfo.deviceType != DeviceType.Handheld) {
			if (rb.velocity.x >= 0) {
				anim.Play(PLAYER_RUN_RIGHT);
			}
			else if (rb.velocity.x < 0) {
				anim.Play(PLAYER_RUN_LEFT);
			}
			else {
				anim.Play(PLAYER_IDLE);
			}
		} else {
			if (Input.acceleration.x >= 0.5) {
				anim.Play(PLAYER_RUN_RIGHT);
			}
			else if (Input.acceleration.x <= -0.5) {
				anim.Play(PLAYER_RUN_LEFT);
			}
			else {
				anim.Play(PLAYER_IDLE);
			}
		}
	}

	private void FixedUpdate() {
		if (isAlive) {
			TiltInput();

			if (SystemInfo.deviceType != DeviceType.Handheld) {
				Move();
			}
		}
	}

	private string GetSpell() {
		string spell = spellBank[UnityEngine.Random.Range(0, spellBank.Length)];

        return spell;
	}

	public void DamagePlayer(float damage) {
		// Camera Shake
		StartCoroutine(Camera.main.GetComponent<CameraMotor>().Shake(0.2f, 0.03f));

		hitpoints -= damage;
		slider.value = hitpoints;

		if (hitpoints <= 0) {
			// Gameover
			rb.velocity = new Vector2(0, 0);
			isAlive = false;
			StartCoroutine(PlayDeathAnimation());
		}
	}

	private void TiltInput() {
		dirX = Input.acceleration.x * speed * Time.deltaTime;
		rb.velocity = new Vector2(dirX, 0);
		// debugX.GetComponent<TextMesh>().text = dirX.ToString();
	}

	private void Move() {
		if (Input.GetKey(KeyCode.LeftArrow))
			MoveLeft();
		else if (Input.GetKey(KeyCode.RightArrow))
			MoveRight();
		else
			rb.velocity = new Vector3(0, 0, 0);
	}

	private void CheckInput() {
		if (Input.anyKeyDown) {
			string pressed = Input.inputString;
			if (pressed.Length == 1)
				EnterLetter(pressed);
		}
	}

	private void Attack() {
		Instantiate(projectile, transform.position, transform.rotation);
		projectile.GetComponent<PlayerProjectile>().damage = damage;
	}

	private void MoveLeft() {
		rb.velocity = new Vector3(-1f, 0, 0) * speed * Time.deltaTime;
	}

	private void MoveRight() {
		rb.velocity = new Vector3(1f, 0, 0) * speed * Time.deltaTime;
	}

	private void SetCurrentWord() {
		currentWord = wordBank[currWordIndex];
		SetRemainingWord(currentWord);
	}

	private void SetRemainingWord(string str) {
		remainingWord = str;
		textObj.GetComponent<TextMesh>().text = remainingWord;
	}

	public void EnterLetter(string typedLetter) {
		if (!IsCorrectLetter(typedLetter)) {
			accuracy++;
            return;
		}

        RemoveLetter();
		if (IsWordComplete()) {
			currWordIndex++;

            Attack();

            if (IsSpellFinish()) {
                currWordIndex = 0;
				wordBank = GetSpell().Split(' ');
            }

			SetCurrentWord();
        }
    }

	private bool IsSpellFinish() {
        return currWordIndex >= wordBank.Length;
    }

	private bool IsCorrectLetter(string letter) {
		// If the key pressed is the first index of the word, return true
		return remainingWord.IndexOf(letter) == 0;
	}    

	private bool IsWordComplete() {
		return remainingWord.Length == 0;
	}

	private bool IsWordCorrect(string word) {
		return currentWord == word;
	}

	private void RemoveLetter() {
		// remove first character
		string newStr = remainingWord.Remove(0, 1);
		SetRemainingWord(newStr);
	}

	private bool ValidMove() {
		if (transform.position.x > 0) return true;
		else if (transform.position.y < 6.5) return true;
		return false;
	}

	IEnumerator PlayDeathAnimation() {
		yield return new WaitForSeconds(deathDelay);
		anim.Play(PLAYER_DEATH_ANIM);
		StartCoroutine(ChangeScene("GameOver", 1f));
	}

	IEnumerator ChangeScene(string name, float delay) {
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(name);
	}
}
