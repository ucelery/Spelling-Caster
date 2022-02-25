using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	public enum State { 
		Idle, Attacking, Damaged
	}

	public enum AttackPattern {
		Random, LeftRight, RightLeft, AtTarget
	}

	private Transform player;

	[Header("Required Objects")]
	public Slider slider;
	public AudioSource audioSource;
	public GameObject spawner;

	public AudioClip deathClip;

	[Header("Enemy Stats")]
	public State state;
	State prevState;

	public float damage = 10f;
	public float maxHitpoints = 100f;
	public float hitpoints;

	public bool alive = true;
	public float destroyDelay = 2f;
	[Header("Idle")]
	public float idleDuration = 5f;

	bool isIdling = false;

	[Header("Attacking")]
	public GameObject projectile;
	public float atkDelay = 1f;
	public float atkDuration = 2f;
	public float projectileForce;
	public float cooldown;
	public float bulletSpread = 0;

	float xPosDir = -2.25f;
	bool isAttacking = false;

	void Start() {
		spawner = GameObject.Find("Game Manager"); // Bad Practice
		audioSource = gameObject.GetComponent<AudioSource>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		hitpoints = maxHitpoints;
		slider = GameObject.Find("Enemy HP Bar").GetComponent<Slider>(); // Bad Practice
		slider.value = hitpoints;
	}

	void FixedUpdate() {
		if (alive) {
			HandleStates();
		}
	}

	void HandleStates() {
		switch (state) {
			case State.Idle:
				if (!isIdling) {
					isIdling = true;
					StartCoroutine(IdleTimer());
				}
				break;
			case State.Attacking:
				if (!isAttacking) {
					AttackPattern randomPattern = (AttackPattern)Random.Range(0, 4);
					switch (randomPattern) {
						case AttackPattern.Random:
							bulletSpread = 2.5f;
							StartCoroutine(ShootPlayer());
							break;
						case AttackPattern.AtTarget:
							bulletSpread = 0f;
							StartCoroutine(ShootPlayer());
							break;
						case AttackPattern.LeftRight:
							bulletSpread = 0f;
							xPosDir = -2.25f;
							StartCoroutine(ShootLeftRight());
							break;
						case AttackPattern.RightLeft:
							bulletSpread = 0f;
							xPosDir = 2.5f;
							StartCoroutine(ShootRightLeft());
							break;
					}

					
					isAttacking = true;
				}
				break;
			case State.Damaged:
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
			Terminate();
		}
	}

	public void Terminate() {
		// death animation
		alive = false;
		audioSource.clip = deathClip;
		audioSource.Play();
		StartCoroutine(DestroyDelay(destroyDelay));
	}

	public void ChangeState(State newState, float duration, State nextState) {
		prevState = state;
		state = newState;
		StartCoroutine(StateTimer(duration, nextState));
	}

	IEnumerator StateTimer(float duration, State nextState) {
		yield return new WaitForSeconds(duration);
		state = nextState;
	}

	IEnumerator ShootPlayer() {
		GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
		Vector2 myPos = transform.position;
		Vector2 targetPos = player.position;

		var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
		var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
		Vector2 spread = new Vector2(randomNumberX, randomNumberY);
		Vector2 direction = ((targetPos - myPos) + spread).normalized;

		proj.GetComponent<EnemyProjectile>().dmg = damage;
		proj.GetComponent<EnemyProjectile>().direction = direction;
		proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;

		yield return new WaitForSeconds(atkDelay);
		if (state == State.Attacking)
			StartCoroutine(ShootPlayer());
	}

	IEnumerator IdleTimer() {
		yield return new WaitForSeconds(idleDuration);
		StopCoroutine(ShootPlayer());
		ChangeState(State.Attacking, atkDuration, State.Idle);
		isIdling = false;
		isAttacking = false;
	}

	IEnumerator ShootLeftRight() {
		GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
		Vector2 myPos = transform.position;
		Vector2 targetPos = new Vector2(xPosDir, -4);

		var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
		var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
		Vector2 spread = new Vector2(randomNumberX, randomNumberY);
		Vector2 direction = ((targetPos - myPos) + spread).normalized;

		proj.GetComponent<EnemyProjectile>().dmg = damage;
		proj.GetComponent<EnemyProjectile>().direction = direction;
		proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;
		yield return new WaitForSeconds(atkDelay);
		xPosDir += (atkDelay / atkDuration) * atkDuration;
		if (state == State.Attacking)
			StartCoroutine(ShootLeftRight());
	}

	IEnumerator ShootRightLeft() {
		GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
		Vector2 myPos = transform.position;
		Vector2 targetPos = new Vector2(xPosDir, -4);

		var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
		var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
		Vector2 spread = new Vector2(randomNumberX, randomNumberY);
		Vector2 direction = ((targetPos - myPos) + spread).normalized;

		proj.GetComponent<EnemyProjectile>().dmg = damage;
		proj.GetComponent<EnemyProjectile>().direction = direction;
		proj.GetComponent<EnemyProjectile>().projectileForce = projectileForce;
		yield return new WaitForSeconds(atkDelay);

		xPosDir -= (atkDelay / atkDuration) * atkDuration;
		if (state == State.Attacking)
			StartCoroutine(ShootRightLeft());
	}

	IEnumerator DestroyDelay(float delay) {
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
		spawner.GetComponent<MainManager>().SpawnNewEnemy();
	}
}
