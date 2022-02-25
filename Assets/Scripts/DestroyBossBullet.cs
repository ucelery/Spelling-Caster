using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBossBullet : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject collision;
    public float dmg;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")) {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
            Destroy(gameObject);
            col.gameObject.GetComponent<PlayerController>().DamagePlayer(dmg);
        }
    }
}
