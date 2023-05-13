using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClone : MonoBehaviour {
    private SpriteRenderer sr;

    [SerializeField]
    private float ghostDelay = 1f;

    [SerializeField]
    private GameObject clone;

    private bool isActive = false;

    public void MakeClones() {
        if (!isActive) {
            isActive = true;
            StartCoroutine(MakeClone());
        }
    }

    public void StopClones() {
        isActive = false;
    }

    public IEnumerator MakeClone() {
        yield return new WaitForSeconds(ghostDelay);
        // Spawn a clone based on current player sprite

        Sprite curSprite = GetComponent<SpriteRenderer>().sprite;
        GameObject cloneInstance = Instantiate(clone, transform.position, Quaternion.identity);
        cloneInstance.transform.parent = transform;
        cloneInstance.GetComponent<SpriteRenderer>().sprite = curSprite;

        if (isActive)
            yield return MakeClone();

        yield break;
    }
}
