using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
	public enum State {
		Idle, Breathing, Attacking, Damaged
	}

	public enum AttackPattern {
		Random, LeftRight, RightLeft, Follow
	}

	public State state;
	private State prevState;

	[Header("Boss Stats")]
	public int bossHealth = 100;
	public int bossDamage = 10;

	private float timeBtwDamage = 1.5f;

	[Header("Game Objects")]
	public GameObject projectile;
	public Rigidbody2D rb;
	private Transform player;

	// animated variables
	public Slider bossHealthBar;
	public bool isDead;

	[Header("Attacking")]
	public float atkDelay = 1f;
	public float atkDuration = 5f;
	public float minDamage;
	public float maxDamage;
	public float projectileForce;
	public float cooldown;
	public float bulletSpread = 0;

	bool isAttacking = false;
	bool canShoot = true;


	[Header("Idle")]
	public float idleDuration = 1f;
	bool isIdle;

	[Header("Breathing")]
	public float breathingDuration = 3f;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		ChangeState(State.Idle, idleDuration, State.Attacking);
	}

	private void Update() {
		HandleStates();

		if (bossHealth <= 0) { 
			// play death animation
		}

		// Breathing Phase
		// give players time to recover and attack
		if (timeBtwDamage > 0) {
			timeBtwDamage -= Time.deltaTime;
		}

		bossHealthBar.value = bossHealth;
	}

	private void HandleStates() {
		switch (state) {
			case State.Idle:
				break;
			case State.Attacking:
				if (canShoot)
					StartCoroutine(ShootPlayer());
				if (!isAttacking)
					StartCoroutine(AttackingDuration());
				break;
			case State.Breathing:
				// Do nothing
				break;
			case State.Damaged:
				// Do nothing
				break;
		}
	}

	public void ChangeState(State newState) {
		prevState = state;
		state = newState;
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
		canShoot = false;
		GameObject spell = Instantiate(projectile, transform.position, Quaternion.identity);
		Vector2 myPos = transform.position;
		Vector2 targetPos = player.position;

		var randomNumberX = Random.Range(-bulletSpread, bulletSpread);
		var randomNumberY = Random.Range(-bulletSpread, bulletSpread);
		Vector2 spread = new Vector2(randomNumberX, randomNumberY);
		Vector2 direction = ((targetPos - myPos) + spread).normalized;

		spell.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
		spell.GetComponent<DestroyBossBullet>().dmg = bossDamage;
		StartCoroutine(ShootPlayer());
		yield return new WaitForSeconds(atkDelay);
	}

	IEnumerator AttackingDuration() {
		isAttacking = true;
		yield return new WaitForSeconds(atkDuration);
		ChangeState(State.Breathing, breathingDuration, State.Attacking);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		bossHealth -= bossDamage;
	}
}
