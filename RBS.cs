using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RBS : MonoBehaviour
{
    public GameObject Target;

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f); //position of the centre ray cast
    public float frontsideSensorPosition = 0.5f; //position of the right ray cast
    public float SensorAngle = 30; //the amount of degrees the angled ray cast will be angled at.
    public bool Won = false;

    //reference to the ml agent script
    public MLAgent mlagent;
    
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    //used when ray cast hit an obstacle
    private bool avoiding = false;

    //inital speed of cube
    private float speed = 1.0f;

    //used when ray casts hit the target
    private bool HitTarget = false;

    private void Start()
    {
        startingPosition = transform.localPosition;
        startingRotation = transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {

        //Rules
        MoveToPlayer();
        AvoidObstacles();
        ChangeSpeed();

        //if other AI has won reset position and reset opponents won boolean to false
        if (mlagent.Won)
        {
            transform.localPosition = startingPosition;
            transform.localRotation = startingRotation;
            mlagent.Won = false;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        //if won reset position and set won to true
        if (other.CompareTag("Target"))
        {
            transform.localPosition = startingPosition;
            transform.localRotation = startingRotation;
            Won = true;
            RBSScore.scoreValue += 1;

            //if the score has reached a limit of 1000, increment set and reset score to 0.
            if (RBSScore.scoreValue == 1000)
            {
                RBSScore.set += 1;
                RBSScore.scoreValue = 0;
            }
        }
    }

    //rule 1 - Move AI towards target
    private void MoveToPlayer()
    {
        //rotate cube so its directly facing the target
        Quaternion targetRotation = Quaternion.LookRotation(Target.transform.localPosition - transform.localPosition);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 1 * Time.deltaTime);

        //move cube along it forward vector
        transform.localPosition += transform.forward * speed * Time.deltaTime;
    }

    //rule 2 - Avoid Obstacles
    private void AvoidObstacles()
    {
        RaycastHit hit;

        //Senors starting position
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

        //The amount to degrees to rotate the cube
        float avoidAngle = 0f;

        //always reset bools when this function is accessed
        avoiding = false;
        HitTarget = false;

        //Front right sensor
        //move sensor along the front face of the cube to the front right side.
        sensorStartPos += transform.right * frontsideSensorPosition;

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //if the ray cast hits the target then we do not want ot dodge it.
            if (hit.collider.CompareTag("Target"))
            {
                avoiding = false;
                HitTarget = true;
                avoidAngle = 0;
            }
            else //dodge obstacles at the given degrees
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidAngle -= 25f;
            }
        }

        //Front right angle sensor
        //Quaternion.AngleAxis() transforms the ray cast line to face an angle
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(SensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //if the ray cast hits the target then we do not want ot dodge it.
            if (hit.collider.CompareTag("Target"))
            {
                avoiding = false;
                HitTarget = true;
                avoidAngle = 0;
            }
            else //dodge obstacles at the given degrees
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidAngle -= 15f;
            }
        }

        //Front left sensor
        sensorStartPos -= transform.right * frontsideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //if the ray cast hits the target then we do not want ot dodge it.
            if (hit.collider.CompareTag("Target"))
            {
                avoiding = false;
                HitTarget = true;
                avoidAngle = 0;
            }
            else //dodge obstacles at the given degrees
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidAngle += 25f;
            }
        }

        //Front left angle sensor
        //Quaternion.AngleAxis() transforms the ray cast line to face an angle
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-SensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //if the ray cast hits the target then we do not want ot dodge it.
            if (hit.collider.CompareTag("Target"))
            {
                avoiding = false;
                HitTarget = true;
                avoidAngle = 0;
            }
            else //dodge obstacles at the given degrees
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidAngle += 15f;
            }
        }

        
        if (avoidAngle == 0f && !HitTarget)
        {
            //front center sensor
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                //if the ray cast hits the target then we do not want ot dodge it.
                if (hit.collider.CompareTag("Target"))
                {
                    avoiding = false;
                    HitTarget = true;
                    avoidAngle = 0;
                }
                else 
                {
                    //get the hit normal of the obstalce to determine if the cube is rotating left or right.
                    //this is neassary to help the cube move around objects when both right and left ray cast trigger which caues a total avoid angle of 0.

                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;

                    if (hit.normal.x < 0)
                    {
                        avoidAngle = -25;
                    }
                    else
                    {
                        avoidAngle = 25;
                    }
                }
            }
        }

        //apply rotation 
        if (avoiding)
        {
            transform.Rotate(Vector3.up, avoidAngle);
        }
    }

    //rule 3 - change speed
    private void ChangeSpeed()
    {
        //change speed if the ray cast hit no objects
        if (avoiding == true)
        {
            speed = 1.0f;
        }
        else
        {
            speed = 3.0f;
        }
    }
}
