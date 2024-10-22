﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float speedH = 5f;
    private float speedHMax = 10f;
    private float inputH;

    private float jumpForce = 22f;
    private float airControlForce = 2.5f;
    private float speedVMax = 6f;

    private float dashForce = 2f;
    private float speedHMaxDash = 40f;

    public int stamina;
    public int staminaMax = 0;
    private int staminaMin = 0;
    private int staminaMargin = 1;
    private int staminaFromPowerup = 5;
    public Slider staminaSlider;

    private float distanceFromWall = .75f;
    private float rotationFromWallX = 270f;
    private float rotationFromWallY = 0f;

    private AudioSource playerAudio;
    public AudioClip sharpInhale;
    public AudioClip sharpExhale;
    public AudioClip softInhale;
    public AudioClip softExhale;
    public AudioClip ting;

    private bool canPlaySound = true;
    private float soundTimerLength = .5f;

    private Rigidbody playerRb;

    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        playerRb = GetComponent<Rigidbody>();
        playerAudio = GetComponent<AudioSource>();
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        MovePlayer();
        Jump();
        LockPlayerPosition();
        LockPlayerRotation();
        EndGameConditions();
        UpdateStaminaBar();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Move player left and right relative to parent (more if dashing)
    void MovePlayer()
    {
        inputH = Input.GetAxis("Horizontal");
        if ((Input.GetAxisRaw("Fire3") > 0 || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) &&
            stamina > staminaMin)
        {
            playerRb.maxAngularVelocity = speedHMaxDash;
            playerRb.AddRelativeTorque(Vector3.forward * speedH * inputH * dashForce);
            DecreaseStamina();
        }
        else
        {
            playerRb.maxAngularVelocity = speedHMax;
            playerRb.AddRelativeTorque(Vector3.forward * speedH * inputH);
        }
    }

    // Allow player to arise (jump) when spacebar is held
    void Jump()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) &&
            stamina > staminaMin && playerRb.velocity.y < speedVMax)
        {
            playerRb.AddForce(Vector3.up * jumpForce);
            DecreaseStamina();
            AirControl();
        }
    }

    // Allow player to move left and right relative to the wall while in the air
    void AirControl()
    {
        if (playerRb.transform.parent.parent.transform.rotation.eulerAngles.y == 0)
        {
            playerRb.AddForce(Vector3.right * inputH * airControlForce);
        }

        if (playerRb.transform.parent.parent.transform.rotation.eulerAngles.y == 90)
        {
            playerRb.AddForce(Vector3.back * inputH * airControlForce);
        }

        if (playerRb.transform.parent.parent.transform.rotation.eulerAngles.y == 180)
        {
            playerRb.AddForce(Vector3.left * inputH * airControlForce);
        }

        if (playerRb.transform.parent.parent.transform.rotation.eulerAngles.y == 270)
        {
            playerRb.AddForce(Vector3.forward * inputH * airControlForce);
        }
    }

    // Lock player's position relative to wall
    void LockPlayerPosition()
    {
        if (playerRb.transform.localPosition.y != distanceFromWall)
        {
            playerRb.transform.localPosition = new Vector3(playerRb.transform.localPosition.x,
                distanceFromWall, playerRb.transform.localPosition.z);
        }
    }

    // Check if player's rotation relative to wall is correct, and fix it if not
    void LockPlayerRotation()
    {
        float marginOfError = 0.1f;
        if (playerRb.transform.localEulerAngles.x < rotationFromWallX - marginOfError ||
            playerRb.transform.localEulerAngles.x > rotationFromWallX + marginOfError)
        {
            playerRb.constraints = RigidbodyConstraints.FreezeAll;
            playerRb.transform.localEulerAngles = new Vector3(rotationFromWallX, rotationFromWallY);
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY;
        }
    }

    void EndGameConditions()
    {
        if (SceneManager.GetActiveScene().name == "Well" && gameManagerScript.gameHasStarted)
        {
            // Infinitely increase stamina when player reaches ceiling
            if (gameManagerScript.gameWon)
            {
                IncreaseMaxStamina(1);
            }

            // Load main menu when player beats game and reaches a certain height
            float winHeight = 500f;
            if (playerRb.transform.position.y > winHeight)
            {
                gameManagerScript.EndGame();
            }
        }
    }

    // Set stamina slider values and size
    void UpdateStaminaBar()
    {
        staminaSlider.maxValue = staminaMax;
        staminaSlider.value = stamina;
        staminaSlider.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, staminaMax * 3);
    }

    // Check whether audio can be played and countdown time if not
    IEnumerator PlaySoundDelayed(AudioClip sound)
    {
        if (canPlaySound)
        {
            playerAudio.clip = sound;
            playerAudio.Play();
            canPlaySound = false;
            yield return new WaitForSeconds(soundTimerLength);
            canPlaySound = true;
        }
    }

    // Decrease stamina toward minimum
    void DecreaseStamina()
    {
        if (stamina > staminaMin && !gameManagerScript.godModeActive)
        {
            stamina--;
            //StartCoroutine(PlaySoundDelayed(softExhale));
        }
    }

    // Increase stamina toward maximum
    void IncreaseStamina(int amountToIncrease)
    {
        if (stamina < staminaMax)
        {
            stamina += amountToIncrease;
            if (stamina < staminaMax - staminaMargin)
            {
                //StartCoroutine(PlaySoundDelayed(softInhale));
            }
        }
    }

    // Increase maximum stamina and set stamina to match
    void IncreaseMaxStamina(int amount)
    {
        staminaMax += amount;
        IncreaseStamina(amount);
        //StartCoroutine(PlaySoundDelayed(softInhale));
    }

    private void OnTriggerStay(Collider other)
    {
        // Set stamina to zero while in front of dark panel
        if (other.gameObject.CompareTag("Dark") && !gameManagerScript.godModeActive)
        {
            stamina = staminaMin;
        }

        // Set stamina to max while in front of light panel
        if (other.gameObject.CompareTag("Light"))
        {
            stamina = staminaMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Play audio when triggering light panel
        if (other.gameObject.CompareTag("Light") && stamina < staminaMax - staminaMargin)
        {
            playerAudio.clip = sharpInhale;
            playerAudio.Play();
        }

        // Play audio when triggering dark panel
        if (other.gameObject.CompareTag("Dark") && stamina > staminaMin + staminaMargin &&
            !gameManagerScript.godModeActive)
        {
            playerAudio.clip = sharpExhale;
            playerAudio.Play();
        }

        // When collecting a powerup, play sound, increase max stamina, and destroy powerup
        if (other.gameObject.CompareTag("Powerup"))
        {
            //AudioSource.PlayClipAtPoint(ting, other.transform.position);
            playerAudio.PlayOneShot(ting, .5f);
            IncreaseMaxStamina(staminaFromPowerup);
            Destroy(other.gameObject);
        }

        // Mark game as won when ceiling is reached
        if (gameManagerScript.gameHasStarted && other.gameObject.CompareTag("Ceiling"))
        {
            gameManagerScript.gameWon = true;
            PlayerPrefs.SetInt("God Mode Unlocked", 1);
        }
    }

    // Recharge stamina when player is touching a platform
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" && stamina < staminaMax)
        {
            IncreaseStamina(1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Start game when player hits a platform
        if (SceneManager.GetActiveScene().name == "Well")
        {
            if (!gameManagerScript.gameHasStarted && collision.gameObject.tag == "Platform")
            {
                gameManagerScript.StartGame();
            }

            // Set player parent and rotation relative to wall on collision
            if (collision.gameObject.CompareTag("Wall"))
            {
                playerRb.transform.parent = collision.gameObject.transform.GetChild(0);
                LockPlayerRotation();
            }
        }
    }
}
