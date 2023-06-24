using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    private GameManager gameManagerScript;

    public Button godModeButton;

    public GameObject godModeLockedText;
    public GameObject godModeStartText;

    // Start is called before the first frame update
    void Start()
    {
        // Find game manager script
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Lock or unlock god mode button as appropriate
        if (gameManagerScript.godModeUnlocked)
        {
            godModeButton.interactable = true;
            godModeStartText.SetActive(true);
            godModeLockedText.SetActive(false);
        }
        else
        {
            godModeButton.interactable = false;
            godModeStartText.SetActive(false);
            godModeLockedText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Start new normal game when button is pressed
    public void NewGame()
    {
        PlayerPrefs.SetInt("God Mode Active", 0);
        SceneManager.LoadScene("Well");
    }

    // Open credits when button is pressed
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    // Start game in GOD MODE when button is pressed
    public void StartGodMode()
    {
        PlayerPrefs.SetInt("God Mode Active", 1);
        SceneManager.LoadScene("Well");
    }
}
