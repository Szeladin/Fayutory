using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapSize = 50;
    [SerializeField] private float spacing = 2.0f;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private Renderer groundRenderer; 
    [SerializeField] private int textureResolution = 512;
    [SerializeField] private List<BiomeSettings> biomes;
    [SerializeField] private int biomeCentersCount = 4;
                    
    private List<Vector2> biomeCenters;
    private List<int> biomeCenterTypes;

    public static Vector2 MapMin { get; private set; }
    public static Vector2 MapMax { get; private set; }

    public enum BiomeType
    {
        Forest,
        Desert,
        Meadow,
        Swamp
    }

    [System.Serializable]
    public class MapObjectSettings
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnChance = 0.2f;
        public float positionJitter = 0.5f;
        public float minDistance = 1f;
    }

    [System.Serializable]
    public class BiomeSettings
    {
        public BiomeType biomeType;
        public List<MapObjectSettings> objects;
        public Color groundColor;
    }

    private BiomeSettings GetBiomeForPosition(float x, float z)
    {

        float minDist = float.MaxValue;
        int closest = 0;

        // Parametry szumu
        float noiseScale = 0.5f; // delikatniejszy wp³yw szumu
        float noiseStrength = 0.11f; // ³agodniejsze granice
        int octaves = 5;
        float lacunarity = 2.2f;
        float gain = 0.55f;

        // Dodaj szum do pozycji, by granice by³y nieregularne
        float offsetX = FractalNoise((x + 1000 + seed) * noiseScale, (z + 2000 + seed) * noiseScale, octaves, lacunarity, gain);
        float offsetZ = FractalNoise((x + 3000 + seed) * noiseScale, (z + 4000 + seed) * noiseScale, octaves, lacunarity, gain);

        Vector2 noisyPos = new Vector2(
            x + (offsetX - 0.5f) * noiseStrength * mapSize,
            z + (offsetZ - 0.5f) * noiseStrength * mapSize
        );

        for (int i = 0; i < biomeCenters.Count; i++)
        {
            float dist = Vector2.Distance(noisyPos, biomeCenters[i]);

            // Dodatkowe zak³ócenie dystansu szumem
            float noise = FractalNoise((x + seed) * noiseScale, (z + seed) * noiseScale, octaves, lacunarity, gain);
            dist *= Mathf.Lerp(1 - noiseStrength, 1 + noiseStrength, noise);

            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return biomes[biomeCenterTypes[closest]];
    }

    private void Start()
    {
        if (randomizeSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        biomeCenters = new List<Vector2>();
        biomeCenterTypes = new List<int>();
        Random.InitState(seed);

        for (int i = 0; i < biomes.Count; i++)
        {
            float x = Random.Range(0, mapSize);
            float z = Random.Range(0, mapSize);
            biomeCenters.Add(new Vector2(x, z));
            biomeCenterTypes.Add(i);
        }
        for (int i = biomes.Count; i < biomeCentersCount; i++)
        {
            float x = Random.Range(0, mapSize);
            float z = Random.Range(0, mapSize);
            int type = Random.Range(0, biomes.Count);
            biomeCenters.Add(new Vector2(x, z));
            biomeCenterTypes.Add(type);
        }
        GenerateMap();
    }
    private float FractalNoise(float x, float z, int octaves = 4, float lacunarity = 2f, float gain = 0.5f)
    {
        float amplitude = 1f, frequency = 1f, noise = 0f, max = 0f;
        for (int i = 0; i < octaves; i++)
        {
            noise += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            max += amplitude;
            amplitude *= gain;
            frequency *= lacunarity;
        }
        return noise / max;
    }
    private void GenerateMap()
    {
        Random.InitState(seed);

        float halfMap = (mapSize - 1) * spacing / 2f;
        MapMin = new Vector2(-halfMap, -halfMap);
        MapMax = new Vector2(halfMap, halfMap);
        List<Vector3> placedPositions = new List<Vector3>();

        // --- GENEROWANIE TEKSTURY GRUNTU ---
        if (groundRenderer != null && biomes != null && biomes.Count > 0)
        {
            Texture2D tex = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int x = 0; x < textureResolution; x++)
            {
                for (int z = 0; z < textureResolution; z++)
                {
                    float mapX = (float)(textureResolution - 1 - x) / (textureResolution - 1) * (mapSize - 1); // ODWROTNOŒÆ X
                    float mapZ = (float)(textureResolution - 1 - z) / (textureResolution - 1) * (mapSize - 1); // ODWROTNOŒÆ Z

                    BiomeSettings biome = GetBiomeForPosition(mapX, mapZ);
                    tex.SetPixel(x, z, biome.groundColor);
                }
            }
            tex.Apply();

            groundRenderer.material.mainTexture = tex;
        }

        if (groundRenderer != null)
        {
            float worldSize = mapSize * spacing;
            groundRenderer.transform.localScale = new Vector3(worldSize / 10f, 1, worldSize / 10f);
        }

        // --- GENEROWANIE OBIEKTÓW NA MAPIE ---
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                BiomeSettings biome = GetBiomeForPosition(x, z);
                foreach (var obj in biome.objects)
                {
                    if (Random.value < obj.spawnChance)
                    {
                        float jitterX = Random.Range(-obj.positionJitter, obj.positionJitter);
                        float jitterZ = Random.Range(-obj.positionJitter, obj.positionJitter);

                        float posX = x * spacing - halfMap + jitterX;
                        float posZ = z * spacing - halfMap + jitterZ;
                        Vector3 position = new Vector3(posX, 0, posZ);

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