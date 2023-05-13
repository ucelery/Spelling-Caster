using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpellingCaster.Stats;

public class MainManager : MonoBehaviour
{
	[Header("Player Level Score")]
	public static TypeStatistics typeStats;

    [Header("Base Enemy Stats")]
	public float baseAtkDelay = 1f;
	public float baseProjForce = 5f;
	public float baseIdleDuration = 3f;

	[Header("Properties")]
	public float delay = 1f;

	[SerializeField]
	private Transform spawnPoint;
	public int level = 1;

	[Header("Required Objects")]
	public GameObject enemyPrefab;
	public GameObject levelText;

	private void Start() {
		levelText.GetComponent<Text>().text = "Level:\n" + level;
	}

	public void SpawnNewEnemy() {
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn() {
		yield return new WaitForSeconds(delay);

		Enemy.enemyLevel = level;
        GameObject enemyInstance = Instantiate(enemyPrefab);
		enemyInstance.transform.position = spawnPoint.position;
		level += 1;
		levelText.GetComponent<Text>().text = "Level: " + level;

		// // Scaling
		// if ((level % 2) == 0) {
		// 	baseAtkDelay -= 0.04f;
		// } else if ((level % 5) == 0) {
		// 	baseProjForce += 0.5f;
		// } else if ((level % 10) == 0) {
		// 	baseIdleDuration -= -0.3f;
		// }

		// if (baseAtkDelay < 0.3f) baseAtkDelay = 0.3f;
		// if (baseProjForce > 10f) baseProjForce = 10f;
		// if (baseIdleDuration < 0.5) baseIdleDuration = 0.5f;

		// enemyInstance.GetComponent<Enemy>().atkDelay = baseAtkDelay;
		// enemyInstance.GetComponent<Enemy>().projectileForce = baseProjForce;
		// enemyInstance.GetComponent<Enemy>().idleDuration = baseIdleDuration;

	}
}
