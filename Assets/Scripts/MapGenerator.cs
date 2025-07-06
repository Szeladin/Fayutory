using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private int mapSize = 50;
    [SerializeField] private float spacing = 2.0f;
    [SerializeField, Range(0f, 1f)] private float treeChance = 0.2f; // 20% szans na drzewo
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private float positionJitter = 0.5f; // Maksymalne losowe przesuniêcie

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

        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                if (Random.value < treeChance)
                {
                    // Dodaj losowe przesuniêcie w obrêbie komórki
                    float jitterX = Random.Range(-positionJitter, positionJitter);
                    float jitterZ = Random.Range(-positionJitter, positionJitter);

                    float posX = x * spacing - halfMap + jitterX;
                    float posZ = z * spacing - halfMap + jitterZ;
                    Vector3 position = new Vector3(posX, 0, posZ);

                    RaycastHit hit;
                    if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out hit, 100))
                    {
                        position.y = hit.point.y;
                    }

                    Instantiate(treePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}