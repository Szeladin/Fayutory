using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapSize = 50;
    [SerializeField] private float spacing = 2.0f;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private Vector2 groundSize = new Vector2(100, 100);

    public static Vector2 MapMin { get; private set; }
    public static Vector2 MapMax { get; private set; }

    [System.Serializable]

    public class MapObjectSettings
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnChance = 0.2f;
        public float positionJitter = 0.5f;
        public float minDistance = 1f;
    }

    [SerializeField] private List<MapObjectSettings> mapObjects;
    private void Start()
    {
        if (randomizeSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        GenerateMap();
    }

    private void GenerateMap()
    {
        Random.InitState(seed);

        float halfMap = (mapSize - 1) * spacing / 2f;
        MapMin = new Vector2(-halfMap, -halfMap);
        MapMax = new Vector2(halfMap, halfMap);
        List<Vector3> placedPositions = new List<Vector3>();

        if (groundPrefab != null)
        {
            Vector3 groundPosition = new Vector3(0, 0, 0);
            GameObject ground = Instantiate(groundPrefab, groundPosition, Quaternion.identity, transform);

            // Skalowanie gruntu (dla domyœlnego Plane 10x10 jednostek)
            float scaleX = groundSize.x;
            float scaleZ = groundSize.y;
            ground.transform.localScale = new Vector3(scaleX, 0, scaleZ);
        }

        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                foreach (var obj in mapObjects)
                {
                    if (Random.value < obj.spawnChance)
                    {
                        float jitterX = Random.Range(-obj.positionJitter, obj.positionJitter);
                        float jitterZ = Random.Range(-obj.positionJitter, obj.positionJitter);

                        float posX = x * spacing - halfMap + jitterX;
                        float posZ = z * spacing - halfMap + jitterZ;
                        Vector3 position = new Vector3(posX, 0, posZ);

                        // SprawdŸ, czy nowy obiekt nie jest zbyt blisko ju¿ istniej¹cych
                        bool tooClose = false;
                        foreach (var placed in placedPositions)
                        {
                            if (Vector3.Distance(placed, position) < obj.minDistance)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (tooClose)
                            continue;

                        RaycastHit hit;
                        if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out hit, 100))
                        {
                            position.y = hit.point.y;
                        }

                        Instantiate(obj.prefab, position, Quaternion.identity, transform);
                        placedPositions.Add(position);
                    }
                }
            }
        }
    }
}