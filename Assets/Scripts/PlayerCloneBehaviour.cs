using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCloneBehaviour : MonoBehaviour {
    private Animator animator;
    private void Start() {
        animator = GetComponent<Animator>();
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
