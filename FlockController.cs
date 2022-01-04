using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    public BoidController boid;
    public GameObject target;
    public int obstaclesToTrack;
    public int neighborsToTrack;
    public float maxDistanceToTarget;
    public float avoidanceStrength;
    public float cohesionStrength;
    public float directionStrength;
    public float targetSeekStrength;
    public float obstacleAvoidanceStrength;
    public float speed;
    public float rotationRate;
    public int flockSize;
    public int i;
    public int ii;
    public int iii;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (i = 0; i < 10; i++)
            {
                this.spawnDragoid();
            }
        }
    }

    public void spawnDragoids()
    {
        for (i = 0; i < flockSize; i++)
        {
             this.spawnDragoid();
        }
    }

    public void spawnDragoid()
    {
        BoidController temp = Instantiate(boid);
        Vector3 spawnLocation = UnityEngine.Random.insideUnitSphere * 400;
        temp.gameObject.transform.position = new Vector3(spawnLocation.x, Mathf.Abs(spawnLocation.y), spawnLocation.z);
        temp.target = target;
        temp.avoidanceStrength = avoidanceStrength;
        temp.cohesionStrength = cohesionStrength;
        temp.directionStrength = directionStrength;
        temp.targetSeekStrength = targetSeekStrength;
        temp.obstacleAvoidanceStrength = obstacleAvoidanceStrength;
        temp.speed = speed;
        temp.rotationRate = rotationRate;
        temp.obstacles = new Vector3[obstaclesToTrack];
        temp.neighbors = new GameObject[neighborsToTrack];
        temp.maxDistanceToTarget = maxDistanceToTarget;
    }
}