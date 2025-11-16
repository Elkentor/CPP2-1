using UnityEngine;
using System.Collections.Generic;

public class CollectibleZoneManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> collectibles; // Assign in Inspector
    [SerializeField] private List<Transform> spawnPoints;   // Assign in Inspector
    public float spreadRadius = 5f;

    void Start()
    {
        SpawnMultipleCollectibles(3); // Spawn 3 collectibles at random spawn points zone
    }

    public void SpawnMultipleCollectibles(int count)
    {
        if (spawnPoints.Count < count)
        {
            Debug.LogError("Not enough spawn points to place collectibles!");
            return;
        }

        List<int> usedIndices = new List<int>();

        for (int i = 0; i < count; i++)
        {
            GameObject collectible = GetRandomCollectible();
            if (collectible == null) continue;

            int index;
            do
            {
                index = Random.Range(0, spawnPoints.Count);
            } while (usedIndices.Contains(index));

            usedIndices.Add(index);

            // OFFSET ALÉATOIRE AUTOUR DU POINT
            Vector3 offset = Random.insideUnitSphere * spreadRadius;
            offset.y = 0; // reste au sol

            Vector3 finalPos = spawnPoints[index].position + offset;

            Instantiate(collectible, finalPos, Quaternion.identity);
        }
    }

    public GameObject GetRandomCollectible()
    {
        if (collectibles == null || collectibles.Count == 0)
        {
            Debug.LogWarning("Collectible list is empty!");
            return null;
        }

        int index = Random.Range(0, collectibles.Count);
        return collectibles[index];
    }
}