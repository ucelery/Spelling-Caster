using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	public float dmg;
	public float projectileForce;
	public Vector2 direction;
	public float lifeSpan = 5f;

	public Rigidbody2D rb;

	private void Start() {
		StartCoroutine(SelfDie());
	}

	void FixedUpdate() {
		rb.velocity = direction * projectileForce;
	}

	IEnumerator SelfDie() {
		yield return new WaitForSeconds(lifeSpan);
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Player")) {
			col.gameObject.GetComponent<PlayerController>().DamagePlayer(dmg);
			Destroy(gameObject);
		}
	}
}
