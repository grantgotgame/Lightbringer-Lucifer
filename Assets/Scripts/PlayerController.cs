using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float speedH = 5f;
    private float speedHMax = 10f;
    private float inputH;

    private float jumpForce = 1100f;
    private float speedVMax = 6f;

    private float dashForce = 2f;
    private float speedHMaxDash = 40f;

    public int stamina;
    public int staminaMax = 0;
    private int staminaMin = 0;
    private int staminaFromPowerup = 5;

    private float distanceFromWall = .75f;
    private float rotationFromWallX = 270f;
    private float rotationFromWallY = 0f;

    private Rigidbody playerRb;

    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Find player Rigidbody
        playerRb = GetComponent<Rigidbody>();

        // Find game manager script
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        inputH = Input.GetAxis("Horizontal");

        MovePlayer();
        Jump();

        LockPlayerPosition();

        // Check if player's rotation is correct, and fix it if not
        float marginOfError = .1f;
        if (playerRb.transform.localEulerAngles.x < rotationFromWallX - marginOfError ||
            playerRb.transform.localEulerAngles.x > rotationFromWallX + marginOfError)
        {
            LockPlayerRotation();
        }

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

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playerRb.transform.parent.transform.rotation.eulerAngles);
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

    // Set stamina to zero while in front of black panel
    private void OnTriggerStay(Collider other)
    {
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

    // When collecting a powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);

            // Increase max stamina
            staminaMax += staminaFromPowerup;
            stamina += staminaFromPowerup;
        }

        // Mark game as won when ceiling is reached
        if (gameManagerScript.gameHasStarted && other.gameObject.CompareTag("Ceiling"))
        {
            gameManagerScript.gameWon = true;
        }
    }

    // Move player left and right relative to parent (more if dashing)
    void MovePlayer()
    {
        //playerRb.AddForce(Vector3.right * inputH * 10f);
        if (Input.GetAxisRaw("Fire3") > 0 && stamina > staminaMin)
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
        if (Input.GetKey(KeyCode.Space) && stamina > staminaMin && playerRb.velocity.y < speedVMax)
        {
            playerRb.AddForce(Vector3.up * jumpForce * Time.deltaTime);
            DecreaseStamina();
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

    // Lock player's rotation relative to wall
    void LockPlayerRotation()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeAll;
        playerRb.transform.localEulerAngles = new Vector3(rotationFromWallX, rotationFromWallY);
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY;
    }

    // Decrease stamina toward minimum
    void DecreaseStamina()
    {
        if (stamina > staminaMin)
        {
            stamina--;
        }
    }
}
