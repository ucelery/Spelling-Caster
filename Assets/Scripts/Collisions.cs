using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }
}
