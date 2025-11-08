using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class MLAgent : Agent
{
    public Transform targetTransform;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    public bool Won = false;

    //reference to RBS script
    public RBS rbs;
    
    Rigidbody AgentRb;
    public override void Initialize()
    {
        AgentRb = GetComponent<Rigidbody>();
        startingPosition = transform.localPosition;
        startingRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        Reset();

        AgentRb.velocity = Vector3.zero;
    }

    private void Update()
    {
        //if opponent has won reset boolean and give a negative reward.
        if (rbs.Won)
        {
            rbs.Won = false;
            AddReward(-0.5f);
            EndEpisode();
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //3 observations of the x, y, z values of each object.
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(rbs.transform.localPosition);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f; //move forward    
                break;
            case 2:
                dirToGo = transform.forward * -1f; //move backward
                break;
            case 3:
                rotateDir = transform.up * 1f; //rotate right
                break;
            case 4:
                rotateDir = transform.up * -1f; //rotate left
                break;
        }

        //apply rotation
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        //apply force 
        AgentRb.AddForce(dirToGo, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)//get actions from agent
    {
        //moves the agent
        MoveAgent(actionBuffers.DiscreteActions);
    }

    //used for the player to control the agent
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3; //rotate right
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1; //move forward
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4; //rotate left
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2; //move backward
        }
    }

    //reset agents position and rotation
    private void Reset()
    {
        transform.localPosition = startingPosition;
        transform.localRotation = startingRotation;
    }
    
    //used when agent overlaps obejcts 
    private void OnTriggerEnter(Collider other)
    {
        //when the agent reaches the target give a positive reward, increment score and set Won to true
        if (other.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            //Debug.Log(GetCumulativeReward());
            Won = true;
            AgentScore.scoreValue += 1;

            //if the score has reached a limit of 1000, increment set and reset score to 0.
            if (AgentScore.scoreValue == 1000)
            {
                AgentScore.set += 1;
                AgentScore.scoreValue = 0;
            }
            EndEpisode();
        }
    }
}
