using UnityEngine;

public class AnimalSpawnZone : MonoBehaviour
{
    public Vector3 zoneSize = new Vector3(10f, 1f, 10f);
    public AnimalType animalType;

    public Vector3 GetRandomPosition()
    {
        
        Vector3 randomPosition = transform.position +
            new Vector3(Random.Range(-zoneSize.x / 2f, zoneSize.x / 2f),
                        Random.Range(-zoneSize.y / 2f, zoneSize.y / 2f),
                        Random.Range(-zoneSize.z / 2f, zoneSize.z / 2f));
        return randomPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, zoneSize);
    }
}