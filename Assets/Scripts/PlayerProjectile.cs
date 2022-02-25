using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
	public Rigidbody2D rb;
	public float projectileSpeed = 5f;
	public float expireTimer = 5f;
	void Start() {
		rb.AddForce(transform.up * projectileSpeed, ForceMode2D.Impulse);
		StartCoroutine(DestroyTimer());
	}

	IEnumerator DestroyTimer() {
		yield return new WaitForSeconds(expireTimer);
		Destroy(gameObject);
	}
}
