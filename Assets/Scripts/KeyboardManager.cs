using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour {
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Transform buttonWaveEffect;

    private Dictionary<string, RectTransform> buttonDictionary = new Dictionary<string, RectTransform>();

    [SerializeField]
    private List<RectTransform> buttonList = new List<RectTransform>();

    private void Start() {
        foreach (var button in buttonList) {
            buttonDictionary.Add(button.name.ToLower(), button);
        }
    }

    public void OnKeyClick(string key) {
        player.SendMessage("EnterLetter", key);

        if (key == "!") {
            if (player.energy >= player.maxEnergy)
                player.SendMessage("ActivatePowerUp");
        }

        if (player.autoComplete) {
            buttonWaveEffect.transform.position = buttonDictionary[key].transform.position;
            buttonWaveEffect.GetComponent<Animator>().Play("Button Wave", 0, 0);
        }
    }
}
