using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
	[Header("Properties")]
	public float delay = 1f;
	public Vector3 spawnPoint;
	public int level = 1;

	[Header("Required Objects")]
	public GameObject enemyPrefab;
	public GameObject levelText;

	private void Start() {
		levelText.GetComponent<Text>().text = "Level: " + level;
	}

	public void SpawnNewEnemy() {
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn() {
		yield return new WaitForSeconds(delay);
		GameObject enemyInstance = Instantiate(enemyPrefab);
		enemyInstance.transform.position = spawnPoint;
		level += 1;
		levelText.GetComponent<Text>().text = "Level: " + level;
	}
}
