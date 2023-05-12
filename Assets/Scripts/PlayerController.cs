using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpellingCaster.Stats;

public class PlayerController : MonoBehaviour
{
	// ANIMATIONS
	private string PLAYER_RUN_LEFT = "RunningLeft";
	private string PLAYER_RUN_RIGHT = "Running";
	private string PLAYER_IDLE = "Idle";
	private string PLAYER_DEATH_ANIM = "Death";

	[Header("Player Stats")]
	[SerializeField]
	public Stats stats;

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
	public Transform textRefPoint;

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
	public bool autoComplete = false;

	[SerializeField]
	[Range(0f, 1f)]
    private float autoThreshold = 5f;

	private TypeStatistics typeStat;

    void Start() {
		if (spellBank.Length < 1) Debug.LogWarning("Empty spell bank");
		if (slider == null) return;

        stats.hitpoints = stats.maxHitpoints;
		slider.maxValue = stats.maxHitpoints;
		slider.value = stats.hitpoints;

		energySlider.maxValue = stats.maxEnergy;
        energySlider.value = stats.energy;

        wordBank = GetSpell().Split(' ');

        currentWord = wordBank[UnityEngine.Random.Range(0, wordBank.Length)];
		
		SetCurrentWord();

		if (SystemInfo.deviceType == DeviceType.Handheld) {
            stats.speed *= 1.5f;
		}
	}

	public void IncreaseStats(Stats addStats) {
		stats.maxHitpoints += addStats.maxHitpoints;
        stats.hitpoints += (addStats.hitpoints * stats.maxHitpoints);

        stats.maxEnergy += addStats.maxEnergy;
        stats.energy += (addStats.energy * stats.maxEnergy);

		stats.damage += addStats.damage;
        stats.speed += addStats.speed;

		slider.maxValue = stats.maxHitpoints;
		energySlider.maxValue = stats.maxEnergy;

        slider.value = stats.hitpoints;
        energySlider.value = stats.energy;
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
			if (stats.energy > 0) {
				stats.energy -= drainSpeed * Time.deltaTime;
				energySlider.value = stats.energy;
			}

			// Handle overflow
			if (stats.energy < 0) {
				stats.energy = 0;

				// Disable PowerUp effect
                cloneGO.SetActive(false);
                autoComplete = false;
				projectile.GetComponent<PlayerProjectile>().isPoweredUp = false;
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

		stats.hitpoints -= damage;
		slider.value = stats.hitpoints;

		if (stats.hitpoints <= 0) {
			// Gameover
			rb.velocity = new Vector2(0, 0);
			isAlive = false;
			StartCoroutine(PlayDeathAnimation());
		}
	}

	private void TiltInput() {
		dirX = Input.acceleration.x * stats.speed * Time.deltaTime;
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
        PlayerProjectile proj = projectile.GetComponent<PlayerProjectile>();
		proj.damage = stats.damage;

        if (autoComplete)
			proj.isPoweredUp = true;
    }

	private void MoveLeft() {
		rb.velocity = new Vector3(-1f, 0, 0) * stats.speed * Time.deltaTime;
	}

	private void MoveRight() {
		rb.velocity = new Vector3(1f, 0, 0) * stats.speed * Time.deltaTime;
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

		float centerPos = -mesh.bounds.extents.x + textRefPoint.position.x;

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
		if (stats.energy < stats.maxEnergy && !autoComplete) {
			stats.energy += 10 * (typeStat.correct / (typeStat.correct + typeStat.mistakes));
			// Handle overflows
			if (stats.energy > stats.maxEnergy) {
				stats.energy = stats.maxEnergy;

				// Activate Animation when energy is max
                cloneGO.SetActive(true);
            }
            energySlider.value = stats.energy;
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