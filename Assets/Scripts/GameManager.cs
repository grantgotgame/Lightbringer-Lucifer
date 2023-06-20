using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float messageClearDelay = 15f;
    private float nextLetterDelay = 0.03f;

    private TMP_Text storyText;

    private string fullText;
    private string currentText;

    public bool gameHasStarted;
    public bool gameWon;

    public bool godModeUnlocked;
    public bool godModeActive;

    private float boostForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
        // initialize story text
        storyText = GameObject.Find("Story Text").GetComponent<TMP_Text>();

        // check whether god mode is unlocked
        godModeUnlocked = (PlayerPrefs.GetInt("God Mode Unlocked") != 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Player ascends when ceiling is reached
    private void OnTriggerEnter(Collider other)
    {
        if (gameHasStarted)
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * boostForce, ForceMode.Impulse);
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    // Update story text when powerup is collected
    public void UpdateStoryText(string powerupText)
    {
        StopAllCoroutines();
        fullText = powerupText;
        StartCoroutine(ShowText());
    }

    // Toggle GOD MODE (for use with button in Main Menu)
    public void ToggleGodMode()
    {
        if (godModeUnlocked)
        {
            godModeActive = !godModeActive;
        }
    }

    // Display one letter at a time
    private IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            currentText = fullText.Substring(0, i);
            storyText.text = currentText;
            yield return new WaitForSeconds(nextLetterDelay);
        }
        StartCoroutine(WaitAndClearMessage());
    }

    // Messages clear after a few seconds
    private IEnumerator WaitAndClearMessage()
    {
        yield return new WaitForSeconds(messageClearDelay);
        storyText.text = "";
    }
}
