using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;
using TMPro;

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

    public float maxEnergy = 100f;
    public float energy = 0;

    public float damage = 10f;
	public float speed = 10f;
	public float deathDelay = 1f;

	public float drainSpeed = 10f;

    public bool isAlive = true;

	[Header("Required GameObjects")]
	public Rigidbody2D rb;
	public GameObject textObj;
	public GameObject projectile;
	public Slider slider;
    public Slider energySlider;
    public Animator anim;
	public GameObject cloneGO;
	public TextMesh backText;

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

	[SerializeField]
	private bool isInGame = true;
	public bool autoComplete = false;

	[SerializeField]
	[Range(0f, 1f)]
    private float autoThreshold = 5f;

	private TypeStatistics typeStat;

    void Start() {
		if (!isInGame) {
			textObj.SetActive(false);
			backText.gameObject.SetActive(false);
			return;
		}

		if (spellBank.Length < 1) Debug.LogWarning("Empty spell bank");

        hitpoints = maxHitpoints;
		slider.maxValue = maxHitpoints;
		slider.value = hitpoints;

		energySlider.maxValue = maxEnergy;
        energySlider.value = energy;

        wordBank = GetSpell().Split(' ');

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

        if (autoComplete) {
			if (energy > 0) {
				energy -= drainSpeed * Time.deltaTime;
				energySlider.value = energy;
			}

			// Handle overflow
			if (energy < 0) {
				energy = 0;

				// Disable PowerUp effect
                cloneGO.SetActive(false);
                autoComplete = false;
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

        backText.text = currentWord;

        CenterTexts();

        SetRemainingWord(currentWord);
    }

	private void SetRemainingWord(string str) {
		remainingWord = str;
		textObj.GetComponent<TextMesh>().text = currentWord.Substring(0, currentWord.Length - remainingWord.Length);
    }

	public void CenterTexts() {
        MeshRenderer mesh = backText.GetComponent<MeshRenderer>();

		float centerPos = -mesh.bounds.extents.x + transform.position.x;

        Debug.Log(mesh.bounds.ToString());

        textObj.transform.position = new Vector3(centerPos, textObj.transform.position.y, textObj.transform.position.z);
        backText.transform.position = new Vector3(centerPos, backText.transform.position.y, backText.transform.position.z);
    }

	public void EnterLetter(string typedLetter) {
		if (!IsCorrectLetter(typedLetter)) {
			typeStat.mistakes++;
            return;
		}

		typeStat.correct++;

        RemoveLetter();
		if (IsWordComplete()) {
			currWordIndex++;

			AddEnergy();

			typeStat.correct = 0;
			typeStat.mistakes = 0;

            Attack();

            if (IsSpellFinish()) {
                currWordIndex = 0;
				wordBank = GetSpell().Split(' ');
            }

			SetCurrentWord();
        }
    }

	private void AddEnergy() {
		if (energy < maxEnergy && !autoComplete) {
			energy += 10 * (typeStat.correct / (typeStat.correct + typeStat.mistakes));
			// Handle overflows
			if (energy > maxEnergy) {
				energy = maxEnergy;

				// Activate Animation when energy is max
                cloneGO.SetActive(true);
            }
            energySlider.value = energy;
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
		if (autoComplete)
			return remainingWord.Length <= currentWord.Length / 2;

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

	public void ActivatePowerUp() {
		autoComplete = true;
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

	private struct TypeStatistics {
		public float correct;
        public float mistakes;
    }
}