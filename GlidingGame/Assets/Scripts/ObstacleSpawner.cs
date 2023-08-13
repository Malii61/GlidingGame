using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float xLimit = 200f; // Limit of the x-axis for obstacle spawning
    [SerializeField] private float zLimit = 1500f; // Limit of the z-axis for obstacle spawning
    [SerializeField] private List<Transform> obstaclePrefabs = new List<Transform>(); // List of obstacle prefabs
    public float checkRadius = 38f; // Radius to check for spawnable area

    private void Start()
    {
        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        bool breakLoop = false;
        while (!breakLoop)
        {
            Transform prefab = GetRandomObstacle();

            bool isSpawnable = false;
            int isSpawnableCheckCounter = 0;
            while (!isSpawnable)
            {
                isSpawnableCheckCounter++;
                Vector3 randomPos = GetRandomPoint();
                isSpawnable = IsSpawnable(randomPos);
                if (isSpawnable)
                {
                    Instantiate(prefab, randomPos, Quaternion.identity);
                }
                if (isSpawnableCheckCounter > 100)
                {
                    breakLoop = true;
                    Debug.Log("Broken");
                    break;
                }
            }
        }
    }

    private Transform GetRandomObstacle()
    {
        return obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
    }

    private bool IsSpawnable(Vector3 pos)
    {
        // Get the world position of the transform
        Vector3 center = pos;

        // Get all colliders within a specified radius
        Collider[] colliders = Physics.OverlapSphere(center, checkRadius);

        // Process the colliders that are checked
        foreach (Collider collider in colliders)
        {
            if (collider.transform.TryGetComponent(out Obstacle obstacle))
            {
                return false; // If an obstacle is found, the position is not spawnable
            }
        }
        return true; // If no obstacle is found, the position is spawnable
    }

    private Vector3 GetRandomPoint()
    {
        // Generate a random position within the specified limits
        Vector3 randomPos = new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-2f, 6f), Random.Range(3, zLimit));
        return randomPos;
    }
}
