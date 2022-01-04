using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloidController : MonoBehaviour
{
    public BoidController boid;
    public GameObject target;
    public int floidLayers;
    public int boidsPerLayer;
    public float maxDistanceToTarget;
    public float floidScaleFactor;
    public int[] floidPhysicsLayers;
    public int obstaclesToTrack;
    public int neighborsToTrack;
    public float avoidanceStrength;
    public float cohesionStrength;
    public float directionStrength;
    public float targetSeekStrength;
    public float obstacleAvoidanceStrength;
    public float speed;
    public float rotationRate;

    // Start is called before the first frame update
    void Start()
    {
        target = this.gameObject;
        //this.spawnDragoids();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; i++)
            {
                this.spawnDragoid();
            }
        }
    }

    public void spawnDragoids()
    {
        for (int i = 0; i < boidsPerLayer; i++)
        {
            this.spawnDragoid();
        }
    }

    public void spawnDragoid()
    {
        BoidController temp = Instantiate(boid);
        //temp.gameObject.transform.position = new Vector3(this.transform.position.x + 50 * i, this.transform.position.y + 50 * ii, this.transform.position.z + 50 * iii);
        Vector3 sphereSpawnSpot = UnityEngine.Random.onUnitSphere * 70 * floidScaleFactor * floidLayers;
        sphereSpawnSpot = new Vector3(sphereSpawnSpot.x, Mathf.Abs(sphereSpawnSpot.y), sphereSpawnSpot.z);
        temp.gameObject.transform.position = this.transform.position + sphereSpawnSpot;
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
        temp.gameObject.layer = floidPhysicsLayers[0] + floidLayers - 1;
        temp.maxDistanceToTarget = maxDistanceToTarget;

        if (floidLayers == 1)
        {
            //this is the boid layer so stop spawning layers
        }
        else
        {
            //set up boid to be another floid
            float scaleFactor = floidScaleFactor * (floidLayers - 1);
            temp.transform.localScale *= scaleFactor;
            temp.avoidanceStrength *= scaleFactor * scaleFactor;
            temp.speed = speed * scaleFactor;
            temp.rotationRate = rotationRate / (0.5f * scaleFactor);
            //temp.rotationRate = rotationRate / scaleFactor;
            temp.gameObject.AddComponent<FloidController>();
            temp.GetComponent<FloidController>().obstaclesToTrack = obstaclesToTrack;
            temp.GetComponent<FloidController>().neighborsToTrack = neighborsToTrack;
            temp.GetComponent<FloidController>().floidScaleFactor = floidScaleFactor;
            temp.GetComponent<FloidController>().target = target;
            temp.GetComponent<FloidController>().avoidanceStrength = avoidanceStrength;
            temp.GetComponent<FloidController>().cohesionStrength = cohesionStrength;
            temp.GetComponent<FloidController>().directionStrength = directionStrength;
            temp.GetComponent<FloidController>().targetSeekStrength = targetSeekStrength;
            temp.GetComponent<FloidController>().obstacleAvoidanceStrength = obstacleAvoidanceStrength;
            temp.GetComponent<FloidController>().speed = speed;
            temp.GetComponent<FloidController>().rotationRate = rotationRate;
            temp.GetComponent<FloidController>().floidLayers = floidLayers - 1;
            temp.GetComponent<FloidController>().boidsPerLayer = boidsPerLayer;
            temp.GetComponent<FloidController>().floidPhysicsLayers = floidPhysicsLayers;
            temp.GetComponent<FloidController>().boid = boid;
            temp.GetComponent<FloidController>().maxDistanceToTarget = maxDistanceToTarget;
            temp.GetComponent<FloidController>().spawnDragoids();
        }
    }
}
