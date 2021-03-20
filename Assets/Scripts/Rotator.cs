using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotator : MonoBehaviour
{

    private Rigidbody rb;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        // Get Object Rigidbody and Collider
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Ensure Trigger is on, is not kinemmatic, and has gravity
        col.isTrigger = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        Debug.Log("Trigger On : " + col.isTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate for effects
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    // Make collectable actually collectable without falling through the floor
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Rock"))
        {
            Destroy(this);
        }
        else { 
            // Reset Kinematics and gravity
            this.rb.useGravity = false;
            this.rb.isKinematic = true;

            Vector3 position = this.transform.localPosition;

            // Set velocity to zero and raise collectables
            this.rb.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(position.x, (position.y + 0.5f), position.z);

            // Set trigger so it is collectable
            this.col.isTrigger = true;
            Debug.Log("Trigger On : " + col.isTrigger);
        }

    }

}
