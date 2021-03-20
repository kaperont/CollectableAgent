using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import ML-Agents
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Collector_Agent_2 : Agent
{
    // Variable Declarations
    Rigidbody rBody;
    public float forceMultiplier = 25;

    // Collectable Declarations
    public int num_collectables = 0;
    public int num_left = 0;
    public GameObject collectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // If the Agent fell off the board, zero its momentum.
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Spawn collectables
        System.Random random = new System.Random();
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

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent location
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        // Keep Agent moving
        rBody.AddForce(controlSignal * forceMultiplier);
        SetReward(-0.00005f);

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
    }

    private void OnTriggerEnter(Collider other)
    {
        // If Agent collides with a collectable...
        if (other.gameObject.CompareTag("Collectable"))
        {
            // "Collect" object
            other.gameObject.SetActive(false);

            // Decrement number of remaining collectables
            num_left = (num_left - 1);

            // Set Positive Reward
            SetReward(1.0f);
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