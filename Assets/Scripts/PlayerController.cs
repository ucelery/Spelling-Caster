using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D rb;
    public GameObject textObj;
    public GameObject projectile;

    private string[] fileLines;
    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;

    void Start() {
        // TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
        // Word Bank
        string readFromFilePath = Application.streamingAssetsPath + "/text.txt";
        fileLines = File.ReadAllLines(readFromFilePath).ToArray();

        currentWord = fileLines[Random.Range(0, fileLines.Length)];
        SetCurrentWord();
    }

    void Update() {
        Move();
        CheckInput();
    }

    private void Move() {
        if (Input.GetKey(KeyCode.LeftArrow))
            MoveLeft();
        else if (Input.GetKey(KeyCode.RightArrow))
            MoveRight();
        else
            rb.velocity = new Vector3(0, 0, 0);
    }

    private void CheckInput() {
        if (Input.anyKeyDown) {
            string pressed = Input.inputString;
            if (pressed.Length == 1)
                EnterLetter(pressed);
		}
	}

    private void Attack() {
        Instantiate(projectile, transform.position, transform.rotation);
    }

    private void MoveLeft() {
        rb.velocity = new Vector3(-1f, 0, 0) * speed * Time.deltaTime;
    }

    private void MoveRight() {
        rb.velocity = new Vector3(1f, 0, 0) * speed * Time.deltaTime;
    }

    private void SetCurrentWord() {
        currentWord = GetNewWord();
        SetRemainingWord(currentWord);
	}

    private void SetRemainingWord(string str) {
        remainingWord = str;
        textObj.GetComponent<TextMesh>().text = remainingWord;
    }

    private void EnterLetter(string typedLetter) {
        if (IsCorrectLetter(typedLetter)) {
            RemoveLetter();

            if (IsWordComplete()) {
                SetCurrentWord();
                Attack();
            }
		}
	}

    private bool IsCorrectLetter(string letter) {
        // If the key pressed is the first index of the word, return true
        return remainingWord.IndexOf(letter) == 0;
	}    

    private bool IsWordComplete() {
        return remainingWord.Length == 0;
	}

    private void RemoveLetter() {
        // remove first character
        string newStr = remainingWord.Remove(0, 1);
        SetRemainingWord(newStr);
    }

    private string GetNewWord() {
        return fileLines[Random.Range(0, fileLines.Length)];
    }

    private bool ValidMove() {
        if (transform.position.x > 0) return true;
        else if (transform.position.y < 6.5) return true;
        return false;
    }
}
