using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour {
    [SerializeField]
    private PlayerController player;
        
    public void OnKeyClick(string key) {
        player.SendMessage("EnterLetter", key);
    }
}
