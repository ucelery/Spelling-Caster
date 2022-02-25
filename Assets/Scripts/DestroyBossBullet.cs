using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBossBullet : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject collision;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        Destroy(gameObject);
        collision.gameObject.GetComponent<PlayerController>().DamagePlayer(10f);
    }
}
