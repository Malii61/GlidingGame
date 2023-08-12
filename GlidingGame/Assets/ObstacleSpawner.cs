using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float xLimit = 200f;
    [SerializeField] private float zLimit = 1500f;
    [SerializeField] private Transform cylinderPrefab;
    [SerializeField] private Transform cubePrefab;
    public float checkRadius = 20f; // Kontrol edilecek menzil yarıçapı
    private void Start()
    {
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
        SpawnObstacle(100, cylinderPrefab);
        SpawnObstacle(500, cubePrefab);
    }
    private void SpawnObstacle(int obstacleCount, Transform prefab)
    {
        bool breakLoop = false;
        for (int i = 0; i < obstacleCount; i++)
        {
            if (breakLoop)
                break;

            Vector3 randomPos = Vector3.zero;
            bool isSpawnable = false;
            int isSpawnableCheckCounter = 0;
            while (!isSpawnable)
            {
                isSpawnableCheckCounter++;
                randomPos = GetRandomPoint();
                isSpawnable = IsSpawnable(randomPos);
                if (isSpawnable)
                {
                    Instantiate(prefab, randomPos, Quaternion.identity);
                }
                if (isSpawnableCheckCounter > 100)
                {
                    breakLoop = true;
                    break;
                }
            }
        }

    }
    private bool IsSpawnable(Vector3 pos)
    {
        // Transform'un d�nya koordinatlar�ndaki pozisyonunu al�yoruz
        Vector3 center = pos;

        // Belirli bir yar��ap i�indeki t�m colliderlar� al�yoruz
        Collider[] colliders = Physics.OverlapSphere(center, checkRadius);

        // Kontrol edilen collider'lar� i�leme
        foreach (Collider collider in colliders)
        {
            if ( collider.transform.TryGetComponent(out Obstacle obstacle))
            {
                return false;
            }
        }
        return true;
    }
    private Vector3 GetRandomPoint()
    {
        Vector3 randomPos = new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-2f, 6f), Random.Range(3, zLimit));
        return randomPos;
    }
}
