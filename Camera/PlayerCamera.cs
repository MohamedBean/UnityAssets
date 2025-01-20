using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform playerTransform;
    private float cameraSpeed = 10f;
    public float shakeAmount = 0.7f;
    public bool shake = false;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector2 distanceBetweenCameraAndPlayer = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y);
        transform.position = new Vector3(transform.position.x + (distanceBetweenCameraAndPlayer.x/cameraSpeed), transform.position.y + (distanceBetweenCameraAndPlayer.y/cameraSpeed), -17);
    }

    private void Update()
    {
        if (shake)
        {
            transform.localPosition = transform.localPosition + UnityEngine.Random.insideUnitSphere * shakeAmount;
        }
    }
}
