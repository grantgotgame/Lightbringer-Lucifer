using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float messageClearDelay = 15f;

    private TMP_Text storyText;

    public bool gameHasStarted = false;

    public bool gameWon = false;

    private float boostForce = 100f;

    public GameObject titleLetters;
    
    // Start is called before the first frame update
    void Start()
    {
        storyText = GameObject.Find("Story Text").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        storyText.text = powerupText;
        StartCoroutine(WaitAndClearMessage());
    }

    // Messages clear after a few seconds
    public IEnumerator WaitAndClearMessage()
    {
        yield return new WaitForSeconds(messageClearDelay);
        storyText.text = "";
    }
}
