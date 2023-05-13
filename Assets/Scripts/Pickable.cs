using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using SpellingCaster.Stats;

public class Pickable : MonoBehaviour {
    [SerializeField]
    private Stats addStats;

    [SerializeField]
    private float speed = 10;

    private void Start() {
        // Scaling
        //addStats.damage += 0.3f * (Enemy.enemyLevel) / 3; // needs balancing
        //addStats.maxHitpoints += 0.3f * Enemy.enemyLevel; // needs balancing
        //addStats.maxEnergy += 0.5f * Enemy.enemyLevel; // needs balancing
    }

    private void FixedUpdate() {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, -3), speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Player")) {
            col.GetComponent<PlayerController>().IncreaseStats(addStats);

            Destroy(gameObject);
        }
    }
}

namespace SpellingCaster.Stats {
    [System.Serializable]
    public struct Stats {
        public float maxHitpoints;

        public float hitpoints;

        public float maxEnergy;

        public float energy;

        public float damage;
        public float speed;
    }
}