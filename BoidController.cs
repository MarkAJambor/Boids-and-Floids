using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public List<GameObject> flockList = new List<GameObject>();
    public List<Vector3> collisionPointList = new List<Vector3>();
    public Vector3[] obstacles;
    public GameObject[] neighbors;
    public GameObject target;
    public GameObject bodyMesh;
    public Transform rightWing;
    public Transform leftWing;
    public Transform body;
    public Vector3 flockDirection;
    public Vector3 flockPosition;
    public Vector3 neighborDirection;
    public Vector3 targetDirection;
    public Vector3 obstacleGoalDirection;
    public Vector3 overallGoalDirection;
    public Vector3 upVectorToUse;
    public float avoidanceStrength;
    public float cohesionStrength;
    public float directionStrength;
    public float targetSeekStrength;
    public float obstacleAvoidanceStrength;
    public float speed;
    public float rotationRate;
    public float flapRate;
    public float wingRotationAngle = 0;
    public float maxDistanceToTarget;
    public int numberOfObstacles = 0;
    public int numberOfNeighbors = 0;
    public int numberOfFlockmates;
    public bool flapUp = false;
    public bool avoidObstacles = false;

    // Start is called before the first frame update
    void Start()
    {
        flapRate = UnityEngine.Random.Range(8f, 20f);
        //disable body renderer is on a different physics layer
        if (this.gameObject.layer != 8)
        {
            bodyMesh.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FixedUpdate()
    {
        if (wingRotationAngle < -60)
        {
            flapUp = true;
        }
        else if (wingRotationAngle > 50)
        {
            flapUp = false;
        }
        if (flapUp)
        {
            wingRotationAngle += flapRate;
            rightWing.localRotation = Quaternion.Euler(0, 0, wingRotationAngle);
            leftWing.localRotation = Quaternion.Euler(0, 0, -wingRotationAngle);
            body.localPosition = new Vector3(0, body.localPosition.y - (flapRate * 0.013f), 0);
        }
        else
        {
            wingRotationAngle -= flapRate;
            rightWing.localRotation = Quaternion.Euler(0, 0, wingRotationAngle);
            leftWing.localRotation = Quaternion.Euler(0, 0, -wingRotationAngle);
            body.localPosition = new Vector3(0, body.localPosition.y + (flapRate * 0.013f), 0);
        }


        numberOfFlockmates = 1;
        flockPosition = Vector3.zero;
        flockPosition += this.transform.position;
        flockDirection = Vector3.zero;
        neighborDirection = Vector3.zero;
        overallGoalDirection = Vector3.zero;
        obstacleGoalDirection = Vector3.zero;
        avoidObstacles = false;

        for (int i = 0; i < numberOfNeighbors; i++)
        {
            numberOfFlockmates++;
            flockPosition += neighbors[i].transform.position;
            flockDirection += neighbors[i].transform.forward / Vector3.SqrMagnitude(this.transform.position - neighbors[i].transform.position);
            neighborDirection += (this.transform.position - neighbors[i].transform.position) / Vector3.SqrMagnitude(this.transform.position - neighbors[i].transform.position);
        }
        flockPosition /= numberOfFlockmates;
        flockDirection = flockDirection.normalized * (numberOfFlockmates - 1);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            if (Vector3.SqrMagnitude(this.transform.position - obstacles[i]) > 0)
            {
                obstacleGoalDirection += this.transform.position - obstacles[i];
                avoidObstacles = true;
            }
        }
        /*
        //old code for lists
        foreach (GameObject flockmate in flockList)
        {
            numberOfFlockmates++;
            flockPosition += flockmate.transform.position;
            flockDirection += flockmate.transform.forward / Vector3.SqrMagnitude(this.transform.position - flockmate.transform.position);
            neighborDirection += (this.transform.position - flockmate.transform.position) / Vector3.SqrMagnitude(this.transform.position - flockmate.transform.position);
        }
        flockPosition /= numberOfFlockmates;
        flockDirection = flockDirection.normalized * (numberOfFlockmates - 1);

        if (collisionPointList.Count != 0)
        {
            foreach (Vector3 collisionPoint in collisionPointList)
            {
                if (Vector3.SqrMagnitude(this.transform.position - collisionPoint) > 0)
                {
                    obstacleGoalDirection += 100 * (this.transform.position - collisionPoint) / Vector3.SqrMagnitude(this.transform.position - collisionPoint);
                    avoidObstacles = true;
                }
            }
        }
        */


        //point vector to center
        overallGoalDirection += (flockPosition - this.transform.position) * cohesionStrength;
        //align vector with flock
        overallGoalDirection += flockDirection * directionStrength;
        //avoid neighbors
        overallGoalDirection += neighborDirection * avoidanceStrength;
        //aim toward target
        if ((target.transform.position - this.transform.position).sqrMagnitude > maxDistanceToTarget * maxDistanceToTarget)
        {
            overallGoalDirection += (target.transform.position - this.transform.position).normalized * targetSeekStrength * 2;
        }
        else
        {
            overallGoalDirection += (target.transform.position - this.transform.position).normalized * targetSeekStrength;
        }

        

        //avoid obstacles
        if (avoidObstacles)
        {
            overallGoalDirection += obstacleGoalDirection * obstacleAvoidanceStrength;
        }

        if (overallGoalDirection.x != 0 || overallGoalDirection.y != 0 || overallGoalDirection.z != 0)
        {
            if (Vector3.Angle(this.transform.forward, overallGoalDirection) < 5)
            {
                upVectorToUse = Vector3.up;
            }
            else
            {
                upVectorToUse = Vector3.Cross(Vector3.Cross(this.transform.forward, overallGoalDirection), this.transform.forward);
            }
            //this.transform.forward = Vector3.RotateTowards(this.transform.forward, overallGoalDirection, rotationRate * Time.deltaTime, 0);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(overallGoalDirection, upVectorToUse), rotationRate * Time.deltaTime);
        }
        this.transform.Translate(this.transform.forward * speed * Time.deltaTime, Space.World);

        numberOfNeighbors = 0;
        numberOfObstacles = 0;
        ////old code for lists
        //flockList = new List<GameObject>();
        //collisionPointList = new List<Vector3>();
    }

    public void updateValuesFromManager()
    {
        //this should only be call if this object is a floid
        BoidManager manager = FindObjectOfType<BoidManager>();
        float scaleFactor = this.GetComponent<FloidController>().floidScaleFactor * (this.gameObject.layer - 8);
        avoidanceStrength *= scaleFactor * scaleFactor;
        speed *= scaleFactor;
        rotationRate /= (0.5f * scaleFactor);
    }

    public void OnTriggerStay(Collider other)
    {
        if (numberOfObstacles < obstacles.Length)
        {
            if (other.gameObject.layer < 8)
            {
                obstacles[numberOfObstacles] = other.ClosestPoint(this.transform.position);
                numberOfObstacles++;
            }
        }
        if (numberOfNeighbors < neighbors.Length)
        {
            //Debug.Log(other.gameObject.layer.ToString() + " " + this.gameObject.layer.ToString());
            if (other.gameObject.layer == this.gameObject.layer)
            {
                neighbors[numberOfNeighbors] = other.gameObject;
                numberOfNeighbors++;
            }
        }

        ////old code for using lists
        //if (other.gameObject.layer == 8)
        //{
        //    flockList.Add(other.gameObject);
        //}
        //else
        //{
        //    collisionPointList.Add(other.ClosestPoint(this.transform.position));
        //}
    }
}
