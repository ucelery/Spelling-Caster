using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
	public static int enemyLevel = 1;

    private string CASTING = "Attack";
	private string IDLE = "Idle";
    private string DEATH = "Dead";

    public enum State { 
		Idle, Attacking, Dead
	}

	public enum AttackPattern {
		Random, LeftRight, RightLeft, AtTarget, NONE
	}

	[System.Serializable]
    public struct Drops {
		public GameObject hpRecovery;
        public GameObject energyRecovery;
        public GameObject hpAmp;
        public GameObject energyAmp;
        public GameObject damageAmp;
    }

	[SerializeField]
	private Drops drops;

	private Transform player;

	[Header("Required Objects")]
	public Slider slider;
	public AudioSource audioSource;
	public GameObject spawner;
	public GameObject projectilePoint;
	public Animator anim;

	public AudioClip deathClip;

	[Header("Enemy Stats")]
	public State state;
	State prevState;

	public float damage = 10f;
	public float maxHitpoints = 100f;
	public float hitpoints;

	public float destroyDelay = 2f;
	[Header("Idle")]
	public float idleDuration = 5f;

	bool isIdling = false;

	[Header("Attacking")]
	public GameObject projectile;
	public float atkDelay = 1f;
    public float atkSpeed = 1f;
    public float atkDuration = 2f;
    public float projectileForce;
	public float cooldown;
	public float bulletSpread = 0;

	public float maxLeftXPos = -3.20f;
	public float maxRightXPos = 3.20f;

	float xPosDir;

	private Coroutine stateRoutine = null;
	private AttackPattern randomPattern = AttackPattern.NONE;

    void Start() {
		InitializeValues();

		spawner = GameObject.Find("Game Manager"); // Bad Practice
		audioSource = gameObject.GetComponent<AudioSource>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		hitpoints = maxHitpoints;
		slider = GameObject.Find("Enemy HP Bar").GetComponent<Slider>(); // Bad Practice
		slider.value = hitpoints;

		anim.speed = atkSpeed;
	}

	void InitializeValues() {
		for (int i = 0; i < enemyLevel; i++) {
			// Scaling per level
			maxHitpoints += 1; // HP
			atkDelay -= 0.04f;
            atkSpeed += 0.04f;
			projectileForce += 0.025f;
			idleDuration -= 0.01f;
        }
	}


	void FixedUpdate() {
		if (player.GetComponent<PlayerController>().isAlive) {
			HandleStates();
		}
	}

	void HandleStates() {
		switch (state) {
			case State.Idle:
				anim.Play(IDLE);
				randomPattern = AttackPattern.NONE;

                if (stateRoutine == null)
					stateRoutine = StartCoroutine(IdleTimer(idleDuration));
				break;
            case State.Attacking:
                anim.Play(CASTING);

				if (stateRoutine == null)
					stateRoutine = StartCoroutine(AttackTimer(atkDuration));
                break;
            case State.Dead:
                anim.Play(DEATH);

				StopAllCoroutines();
				stateRoutine = null;
                break;
        }
	}

	public void ChangeState(State newState) {
		prevState = state;
		state = newState;
	}

	public void DamageEnemy(float dmgSource) {
		hitpoints -= dmgSource;
		slider.value = hitpoints;

		audioSource.clip = deathClip;
		audioSource.Play();

		if (hitpoints <= 0) {
			ChangeState(State.Dead);
		}
	}

	// Used by the animation
	public void Terminate() {
		SpawnDrops();
        Destroy(gameObject);
        spawner.GetComponent<MainManager>().SpawnNewEnemy();
    }

	public void ShootPlayer() {
		if (state == State.Dead || state != State.Attacking) return;

		if (stateRoutine == null)
            stateRoutine = StartCoroutine(AttackTimer(atkDuration));

        if (randomPattern == AttackPattern.NONE) {
			randomPattern = (AttackPattern)Random.Range(0, 4);
			xPosDir = GetInitialXDirection(randomPattern);
        }

        switch (randomPattern) {
            case AttackPattern.Random:
                bulletSpread = 2.5f;
				// StartCoroutine(ShootPlayer());
				TargetPlayerPattern();
                break;
            case AttackPattern.AtTarget:
                bulletSpread = 0f;
                // StartCoroutine(ShootPlayer());
                TargetPlayerPattern();
                break;
            case AttackPattern.LeftRight:
                bulletSpread = 0f;

				// StartCoroutine(ShootLeftRight());
				LeftRightPattern();
                break;
            case AttackPattern.RightLeft:
                bulletSpread = 0f;

				// StartCoroutine(ShootRightLeft());
				RightLeftPattern();
                break;
        }
    }

	private float GetInitialXDirection(AttackPattern attackPattern) {
        float xTargetDir = 0f;

		Debug.Log(attackPattern);

		if (attackPattern == AttackPattern.LeftRight)
			xTargetDir = maxLeftXPos;
        else if (attackPattern == AttackPattern.RightLeft)
            xTargetDir = maxRightXPos;

		Debug.Log(maxLeftXPos);
		Debug.Log(xPosDir);
        return xTargetDir;
	}

	private void SpawnDrops() {
		// Removed due to OP scaling
        //if ((enemyLevel % 6) == 0) {
        //    // DAMAGE AMP
        //    Instantiate(drops.damageAmp, projectilePoint.transform.position, Quaternion.identity);
        //} else if ((enemyLevel % 3) == 0) {
        //    // MAX HP UP
        //    Instantiate(drops.hpAmp, projectilePoint.transform.position, Quaternion.identity);
        //} else if ((enemyLevel % 5) == 0) {
        //    // MAX ENERGY UP
        //    Instantiate(drops.energyAmp, projectilePoint.transform.position, Quaternion.identity);
        //} 

        if ((enemyLevel % 2) == 0) {
            // HP RECOVERY
            Instantiate(drops.hpRecovery, projectilePoint.transform.position, Quaternion.identity);
        } else {
            // ENERGY RECOVERY
            Instantiate(drops.energyRecovery, projectilePoint.transform.position, Quaternion.identity);
        }
    }

	#region [ Attack Patterns ]
	public void RightLeftPattern() {
        GameObject proj = Instantiate(projectile, projectilePoint.transform.position, Quaternion.identity);
        Vector2 myPos = projectilePoint.transform.position;
        Vector2 targetPos = new Vector2(xPosDir, -1.25f);

        var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
        var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
        Vector2 spread = new Vector2(randomNumberX, randomNumberY);
        Vector2 direction = ((targetPos - myPos) + spread).normalized;

        proj.GetComponent<EnemyProjectile>().dmg = damage;
        proj.GetComponent<EnemyProjectile>().direction = direction;
        proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;

		xPosDir -= atkSpeed + (-0.04f * enemyLevel);

        if (xPosDir < maxLeftXPos) {
            randomPattern = AttackPattern.LeftRight;
        }
    }

	public void LeftRightPattern() {
        GameObject proj = Instantiate(projectile, projectilePoint.transform.position, Quaternion.identity);
        Vector2 myPos = projectilePoint.transform.position;
        Vector2 targetPos = new Vector2(xPosDir, -1.25f);

        var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
        var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
        Vector2 spread = new Vector2(randomNumberX, randomNumberY);
        Vector2 direction = ((targetPos - myPos) + spread).normalized;

        proj.GetComponent<EnemyProjectile>().dmg = damage;
        proj.GetComponent<EnemyProjectile>().direction = direction;
        proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;

        xPosDir += atkSpeed + (-0.04f * enemyLevel);

		if (xPosDir > maxRightXPos) {
			randomPattern = AttackPattern.RightLeft;
		}
    }

	public void TargetPlayerPattern() {
        GameObject proj = Instantiate(projectile, projectilePoint.transform.position, Quaternion.identity);
        Vector2 myPos = projectilePoint.transform.position;
        Vector2 targetPos = player.position;

        var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
        var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
        Vector2 spread = new Vector2(randomNumberX, randomNumberY);
        Vector2 direction = ((targetPos - myPos) + spread).normalized;

        proj.GetComponent<EnemyProjectile>().dmg = damage;
        proj.GetComponent<EnemyProjectile>().direction = direction;
        proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;
    }
    #endregion

	IEnumerator AttackTimer(float delay) {
		yield return new WaitForSeconds(delay);
		ChangeState(State.Idle);
		stateRoutine = null;
    }

	IEnumerator IdleTimer(float delay) {
        yield return new WaitForSeconds(delay);
        ChangeState(State.Attacking);
        stateRoutine = null;
    }
}
