using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject treePrefab;
    public int width = 50;
    public int height = 50;
    public float spacing = 2.0f;
    public float treeChance = 0.2f; // 20% chance per cell

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (Random.value < treeChance)
                {
                    Vector3 position = new Vector3(x * spacing, 0, z * spacing);

                    // Optional: Raycast to terrain for Y adjustment
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
