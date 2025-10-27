using UnityEngine;
using System.Collections.Generic;

public class CollectibleZoneManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> collectibles; // Assign in Inspector
    [SerializeField] private List<Transform> spawnPoints;   // Assign in Inspector

    void Start()
    {
        SpawnMultipleCollectibles(3); // Spawn 3 collectibles at random spawn points
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

            Instantiate(collectible, spawnPoints[index].position, Quaternion.identity);
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