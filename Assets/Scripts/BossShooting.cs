using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject BossBullet;

    public float bulletForce = 20f;
    private float timeBtwShots = 1.5f;
    // Update is called once per frame
    void Update()
    {

        if(timeBtwShots > 0)
        {
            timeBtwShots -= Time.deltaTime; 
        }
        else
        {
            Shoot();
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(BossBullet, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            timeBtwShots = 1.5f;
        }
    }
}
