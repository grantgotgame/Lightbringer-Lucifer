using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeManager : MonoBehaviour
{
    public float maxDistanceFromPlayer = 0.26f;

    private Vector3 lookDirection;

    private GameObject player;
    private GameObject target;
    public GameObject finalTarget;
    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        target = FindClosestPowerup();

        FindDirectionOfPowerup();

        BindMovement();        
    }

    public GameObject FindClosestPowerup()
    {
        // Find the nearest powerup
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Powerup");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        // Return final target if no more powerups exist
        if (closest == null)
        {
            closest = finalTarget;
        }

        // Fall back on camera if no final target exists
        if (closest == null)
        {
            closest = mainCamera;
        }

        return closest;
    }

    void FindDirectionOfPowerup()
    {
        lookDirection = (target.transform.position - transform.position).normalized;
    }

    void BindMovement()
    {
        // Orient relative to player
        transform.position = player.transform.position + (lookDirection * maxDistanceFromPlayer);
        transform.LookAt(target.transform.position);
    }
}
