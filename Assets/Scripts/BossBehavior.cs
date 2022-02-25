using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    // function variables
    public int bossHealth = 100;
    public int bossDamage = 10;
    private float timeBtwDamage = 1.5f;
    public GameObject projectile;
    public Rigidbody2D rb;
    // animated variables
    public Slider bossHealthBar;
    public bool isDead;

    // Update is called once per frame
    private void Update()
    {

        if (bossHealth <= 0)
        { // play death animation
        }

        // Breathing Phase
        // give players time to recover and attack
        if (timeBtwDamage > 0)
        {
            timeBtwDamage -= Time.deltaTime;
        }

        bossHealthBar.value = bossHealth;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        bossHealth -= bossDamage;
    }
}
