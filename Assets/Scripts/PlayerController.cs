using System.Collections;
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
    private int staminaFromPowerup = 5;
    private Slider staminaSlider;

    private float distanceFromWall = .75f;
    private float rotationFromWallX = 270f;
    private float rotationFromWallY = 0f;

    private AudioSource playerAudio;
    public AudioClip sharpInhale;
    public AudioClip sharpExhale;
    public AudioClip softInhale;
    public AudioClip softExhale;

    private Rigidbody playerRb;

    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        playerRb = GetComponent<Rigidbody>();
        staminaSlider = GetComponentInChildren<Slider>();
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
        float marginOfError = .1f;
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
                staminaMax++;
                stamina = staminaMax;
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

    // Decrease stamina toward minimum
    void DecreaseStamina()
    {
        if (stamina > staminaMin)
        {
            stamina--;
            if (!playerAudio.isPlaying)
            {
                playerAudio.clip = softExhale;
                playerAudio.Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Set stamina to zero while in front of black panel
        if (other.gameObject.CompareTag("Dark"))
        {
            stamina = staminaMin;
        }

        // Set stamina to max while in front of white panel
        if (other.gameObject.CompareTag("Light"))
        {
            stamina = staminaMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Play audio and when triggering light panel
        if (other.gameObject.CompareTag("Light") && stamina < staminaMax)
        {
            playerAudio.clip = sharpInhale;
            playerAudio.Play();
        }

        // Play audio and set bool when triggering dark panel
        if (other.gameObject.CompareTag("Dark") && stamina > staminaMin)
        {
            playerAudio.clip = sharpExhale;
            playerAudio.Play();
        }

        // When collecting a powerup, destroy powerup and increase max stamina
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            staminaMax += staminaFromPowerup;
            stamina += staminaFromPowerup;
        }

        // Mark game as won when ceiling is reached
        if (gameManagerScript.gameHasStarted && other.gameObject.CompareTag("Ceiling"))
        {
            gameManagerScript.gameWon = true;
        }
    }

    // Recharge stamina when player is touching a platform
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" && stamina < staminaMax)
        {
            stamina++;
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
