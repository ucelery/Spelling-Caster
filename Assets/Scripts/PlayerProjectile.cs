using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
	public Rigidbody2D rb;
	public float projectileSpeed = 5f;
	public float damage;
	public bool isPoweredUp = false;

	void Start() {
        if (isPoweredUp) return;

		rb.AddForce(transform.up * projectileSpeed, ForceMode2D.Impulse);
	}

    private void Update() {
		if (!isPoweredUp) return;

		transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, 5.82f), projectileSpeed * Time.deltaTime);
    }

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Enemy")) {
			if (col.gameObject.GetComponent<Enemy>().state != Enemy.State.Dead) {
				col.gameObject.GetComponent<Enemy>().DamageEnemy(damage);
                Destroy(gameObject);
            }
		}

        if (col.gameObject.CompareTag("Invisible Walls")) {
            Destroy(gameObject);
        }
    }
}
