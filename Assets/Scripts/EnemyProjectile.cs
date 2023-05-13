using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	public float dmg;
	public float projectileForce;
	public Vector2 direction;
	public Rigidbody2D rb;

	void FixedUpdate() {
		rb.velocity = direction * projectileForce;
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Player")) {
			col.gameObject.GetComponent<PlayerController>().DamagePlayer(dmg);
            Destroy(gameObject);
        }

		if (col.gameObject.CompareTag("Invisible Walls")) {
            Destroy(gameObject);
        }
    }
}
