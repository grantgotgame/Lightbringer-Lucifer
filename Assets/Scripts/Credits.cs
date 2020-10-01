using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void August()
    {
        Application.OpenURL("https://soundcloud.com/anniejohnson/deamy-dreams-ft-chase-potter");
    }

    public void CreateWithCode()
    {
        Application.OpenURL("https://learn.unity.com/course/create-with-code-live-summer-2020");
    }

    public void Back()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
