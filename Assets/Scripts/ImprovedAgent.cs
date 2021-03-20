using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import ML Agents
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class ImprovedAgent : Agent
{
    // Initialize Environment variables
    Rigidbody AgentRB;

    // Initialize Agent speeds
    public float turnSpeed = 300f;
    public float moveSpeed = 2f;

    // Collectable Declarations
    public int num_collectables = 0;
    public int num_left = 0;
    public GameObject collectPrefab;

    public override void Initialize()
    {
        AgentRB = GetComponent<Rigidbody>();

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent location
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        var localVelocity = transform.InverseTransformDirection(AgentRB.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);

        // Collectable information
        sensor.AddObservation(num_collectables);
        sensor.AddObservation(num_left);
    }
    
    public void MoveAgent(ActionBuffers actionBuffers)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;

        var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
        var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
        var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

        dirToGo = transform.forward * forward;
        dirToGo += transform.right * right;
        rotateDir = -transform.up * rotate;

        AgentRB.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        if (AgentRB.velocity.sqrMagnitude > 25f) // slow it down
        {
            AgentRB.velocity *= 0.95f;
        }


    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // If Agent collects all collectables...
        if (num_left == 0)
        {
            // Set Positive Reward
            SetReward(1.0f);

            // End Episode
            EndEpisode();
        }

        // If Agent falls off edge...
        else if (this.transform.localPosition.y < 0)
        {

            // Destroy collectables
            var gameObjects = GameObject.FindGameObjectsWithTag("Collectable");
            for (var i = 0; i < gameObjects.Length; i++)
            {
                Destroy(gameObjects[i]);
            }

            // Set Negative Reward
            SetReward(-1.0f);

            // End Episode
            EndEpisode();
        }

        MoveAgent(actionBuffers);
    }

    public override void OnEpisodeBegin()
    {

        // If the Agent fell off the board, zero its momentum.
        if (transform.localPosition.y < 0)
        {
            AgentRB.angularVelocity = Vector3.zero;
            AgentRB.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }


        // Spawn Agent
        System.Random random = new System.Random();

        transform.position = new Vector3(random.Next(-18, 0),
                                        0.5f,
                                        random.Next(-5, 9));
        transform.rotation = Quaternion.Euler(new Vector3(0f, random.Next(-18, 18)));


        // Spawn collectables
        num_collectables = random.Next(3, 10);
        num_left = num_collectables;

        for (int i = 0; i < num_collectables; i++)
        {
            Instantiate(collectPrefab,
                        new Vector3(random.Next(-18, 18),
                                    7.0f,
                                    random.Next(-18, 18)),
                        Quaternion.identity);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // If Agent collides with a collectable...
        if (other.gameObject.CompareTag("Collectable"))
        {
            // "Collect" object
            other.gameObject.SetActive(false);

            // Decrement number of remaining collectables
            num_left = (num_left - 1);

            // Set Positive Reward
            AddReward(1.0f - (float)(num_left / num_collectables));

            // If no collectables exist (in case of two agents)...
            if (GameObject.Find("Collectable(Clone)") == null)
            {
                EndEpisode();
            }
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = 0;
        continuousActionsOut[1] = 0;
        continuousActionsOut[2] = 0;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
    }

}
