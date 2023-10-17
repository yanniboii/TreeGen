using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public int boidAmount;

    public float spawnRange;

    public GameObject boidPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBoids();
    }

    public void SpawnBoids()
    {
        for (int i = 0; i < boidAmount; i++)
        {
            Vector3 pos = new Vector3(this.transform.position.x + Random.Range(-spawnRange, spawnRange), this.transform.position.y + Random.Range(-spawnRange, spawnRange), this.transform.position.z + Random.Range(-spawnRange, spawnRange));
            GameObject boid = Instantiate(boidPrefab, pos, Random.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
