﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameObject explosionEffect;
    private GameController gameController;
    public float minMoveDistance;
    public float maxMoveDistance;
    public float minX, maxX, minZ, maxZ;
    public float speed;
    public float startWaitTime;
    public float minStopTime;
    public float maxStopTime;
    private float pauseTime;
    private bool isStopped;
    public bool isRotating;
    private Quaternion newRotation;

    private Vector3 newPosition; // destination point

    void Start()
    {
        // Only for cube prefab
        if (isRotating == true)
        {
            GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 5;
        }

        newPosition = transform.position;
        isStopped = true;
        // Start movement after the first wait time
        StartCoroutine(WaitAndSetNewPosition(startWaitTime));
    }

    void Update()
    {
        if (isStopped == false)
            Move();
    }

    int PickPlusOrMinusByRandom()
    {
        if (Random.Range(0, 2) == 1)
            return 1;
        else
            return -1;
    }

    void SetRandomPosition()
    {
        float randomDistance;
        float newX, newZ;
        Vector3 curPos = transform.position;

        // Set new Random X point
        randomDistance = Random.Range(minMoveDistance, maxMoveDistance) * PickPlusOrMinusByRandom();
        newX = curPos.x + randomDistance;
        if (newX < minX || newX > maxX)
            newX = curPos.x - randomDistance;

        // Set new Random Z point
        randomDistance = Random.Range(minMoveDistance, maxMoveDistance) * PickPlusOrMinusByRandom();
        newZ = curPos.z + randomDistance;
        if (newZ < minZ || newZ > maxZ)
            newZ = curPos.z - randomDistance;

        Vector3 newPos = new Vector3(newX, 0, newZ);

        newRotation = Quaternion.LookRotation(newPos - transform.position);
        newPosition = newPos;
    }

    IEnumerator WaitAndSetNewPosition(float time)
    {
        yield return new WaitForSeconds(time);
        SetRandomPosition();
        isStopped = false;
    }

    void Move()
    {
        if (isStopped == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, speed * Time.deltaTime);
            //move
            transform.position = Vector3.MoveTowards(
                transform.position, // start point
                newPosition, // destination 
                speed * Time.deltaTime // movement distance
            );

            // When the object arrive at current destination,
            // Wait for a random amount of time and go to a new route
            if (transform.position == newPosition)
            {
                isStopped = true;
                pauseTime = Random.Range(minStopTime, maxStopTime);
                StartCoroutine(WaitAndSetNewPosition(pauseTime));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        bool canCollide = other.GetComponent<Player>().CanCollide;
        if (canCollide == true) {
            Instantiate(explosionEffect,
                    gameObject.transform.position,
                    gameObject.transform.rotation
            );
            Destroy(gameObject);
        }
    }
}