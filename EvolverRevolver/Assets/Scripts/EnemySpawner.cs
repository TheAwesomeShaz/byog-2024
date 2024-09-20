using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefabToSpawn;
    [SerializeField] private float spawnInterval;
    [SerializeField] private bool canSpawn;

    private Transform[] patrolPoints;
    private Enemy spawnedEnemy;
    private Transform player;
    private float spawnDistance;
    private bool hasSpawned;

    private float spawnTimer;

    public void SetCanSpawn(bool value)
    {
        //TODO: have to check this approach in balancing maybe we keep on spawning enemies?
        // but then whats the point of them patrolling?
        if (spawnedEnemy != null)
        {
            canSpawn = value;
        }
    }

    public void SetEnemyPatrolPoints(Transform[] patrolPoints)
    {
        this.patrolPoints = patrolPoints; 
    }

    public void SetPlayerControllerTransform(Transform player)
    {
        this.player = player;
    }


    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < spawnDistance)
        {
            if (spawnTimer <= 0)
            {
                spawnedEnemy = Instantiate(enemyPrefabToSpawn, transform.position, Quaternion.identity);
                spawnedEnemy.SetPatrolPoints(patrolPoints);
                spawnedEnemy.OnDeath += SpawnedEnemy_OnDeath;
                spawnTimer = spawnInterval;
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
    }
    //IEnumerator SpawnEnemiesAfterInterval()
    //{
    //    while (true)
    //    {
    //        if (spawnedEnemy == null)
    //        {
    //            Debug.Log("Can Spawn enemy hence spawning" + canSpawn);
    //            spawnedEnemy = Instantiate(enemyPrefabToSpawn, transform.position, Quaternion.identity);
    //            spawnedEnemy.SetPatrolPoints(patrolPoints);
    //            spawnedEnemy.OnDeath += SpawnedEnemy_OnDeath;
    //        }
    //        Debug.Log("Waiting for spawnIntreval " + spawnInterval);
    //        yield return new WaitForSeconds(spawnInterval);
    //    }
    //}
    private void SpawnedEnemy_OnDeath()
    {
        spawnedEnemy = null;
        spawnedEnemy.OnDeath -= SpawnedEnemy_OnDeath;
    }
}
