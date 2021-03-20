using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevIn : MonoBehaviour
{

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 20;

    public int num_collectables = 0;
    public int num_left = 0;
    public GameObject collectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Spawn collectables
        num_collectables = (int)((Random.value % 7) + 3);
        num_left = num_collectables;

        for (int i = 0; i < num_collectables; i++)
        {
            Instantiate(collectPrefab,
                        new Vector3(Random.value * 8 - 4,
                                    7.0f,
                                    Random.value * 8 - 4),
                        Quaternion.identity);
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable")) {
            other.gameObject.SetActive(false);
        }
    }

}
