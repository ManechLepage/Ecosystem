using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeType {
    SPRUCE,
    PINE,
    BIRCH,
    NO_TREE
}
public enum TileType {
    SAND,
    GRASS,
    STONE,
    NO_TILE
}
public class NoiseSettings {
    public float noiseScale = 1f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float frequency = 1f;
    public float lacunarity = 2f;
    public int seed = 0;
    public float smoothness = 1f;
    public float height = 1f;
    public float amplitude = 1f;

    public NoiseSettings(float noiseScale, int octaves, float persistance,
            float frequency, float lacunarity, int seed, float smoothness, float height, float amplitude) {
        this.noiseScale = noiseScale;
        this.octaves = octaves;
        this.persistance = persistance;
        this.frequency = frequency;
        this.lacunarity = lacunarity;
        this.seed = seed;
        this.smoothness = smoothness;
        this.height = height;
        this.amplitude = amplitude;
    }
}
public class HeightRange {
    public float minimum;
    public float maximum;

    public HeightRange(float minimum, float maximum) {
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public bool Contains(float value) {
        return value >= minimum && value <= maximum;
    }
}

public class Biome {
    public string name;
    public Dictionary<TileType, HeightRange> tile_heights = new Dictionary<TileType, HeightRange>();
    public Dictionary<TreeType, HeightRange> tree_heights = new Dictionary<TreeType, HeightRange>();
    public int seed;

    public Biome(string name, Dictionary<TileType, HeightRange> tile_heights,
            Dictionary<TreeType, HeightRange> tree_heights, int seed) {
        this.name = name;
        this.tile_heights = tile_heights;
        this.tree_heights = tree_heights;
        this.seed = seed;
    }

    public virtual float BiomeNoise(float x, float y, NoiseSettings settings) {
        return Mathf.PerlinNoise((x + settings.seed) * settings.noiseScale / settings.smoothness, (y + settings.seed) * settings.noiseScale / settings.smoothness);
    }

    public TileType GetTileType(float noise) {
        // noise is between -1 and 1
        foreach (TileType tile_type in tile_heights.Keys) {
            if (tile_heights[tile_type].Contains(noise)) {
                return tile_type;
            }
        }

        return TileType.NO_TILE;
    }

    public TreeType GetTreeType(float noise) {
        // noise is between -1 and 1
        foreach (TreeType tree_type in tree_heights.Keys) {
            if (tree_heights[tree_type].Contains(noise)) {
                return tree_type;
            }
        }

        return TreeType.NO_TREE;
    }
}

public class Island : Biome {
    public Island() : base("Island", new Dictionary<TileType, HeightRange>() {
        { TileType.SAND, new HeightRange(-1, -0.05f) },
        { TileType.GRASS, new HeightRange(-0.05f, 0.5f) },
        { TileType.STONE, new HeightRange(0.5f, 1) }
    }, new Dictionary<TreeType, HeightRange>() {
        { TreeType.SPRUCE, new HeightRange(-1, 0.5f) },
        { TreeType.PINE, new HeightRange(0.5f, 1) }
    }, 0) { }

    public override float BiomeNoise(float x, float y, NoiseSettings settings) {
        // Créer un nouveau noise
        SimpleTerrain terrain = new SimpleTerrain();
        Vector2 noise = terrain.SimpleNoise(x, y, settings);
        return noise.x / noise.y;
    }
}
public class BorealForest : Biome {
    // error : UnityException: RandomRangeInt is not allowed to be called from a MonoBehaviour constructor (or instance field initializer), call it in Awake or Start instead. Called from MonoBehaviour 'TerrainGenerator' on game object 'Terrain'.
    //See "Script Serialization" page in the Unity Manual for further details.
    //UnityEngine.Random.Range (System.Int32 minInclusive, System.Int32 maxExclusive) (at <3be1a7ff939c43f181c0a10b5a0189ac>:0)
    public BorealForest() : base("Boreal Forest", new Dictionary<TileType, HeightRange>() {
        { TileType.SAND, new HeightRange(-1, -0.1f) },
        { TileType.GRASS, new HeightRange(-0.1f, 0.8f) },
        { TileType.STONE, new HeightRange(0.8f, 1) }
    }, new Dictionary<TreeType, HeightRange>() {
        { TreeType.SPRUCE, new HeightRange(-1, 0.5f) },
        { TreeType.PINE, new HeightRange(0.5f, 1) }
    }, 0) { }

    public override float BiomeNoise(float x, float y, NoiseSettings settings) {
        SimpleTerrain terrain = new SimpleTerrain();
        Vector2 base_noise = terrain.SimpleNoise(x / 3f, y / 3f, settings);
        float noise = base_noise.x;
        float maxValue = base_noise.y;

        return (noise / maxValue * 1.5f - 0.65f) * settings.height * 1.5f;
    }
}

public class SimpleTerrain {
    public Vector2 SimpleNoise(float x, float y, NoiseSettings settings) {
        float noise = 0;
        float frequency = settings.frequency;
        float amplitude = settings.amplitude;
        float maxValue = 0;

        for (int i = 0; i < settings.octaves; i++) {
            noise += Mathf.PerlinNoise((x + settings.seed) * settings.noiseScale * frequency, (y + settings.seed)
                * settings.noiseScale * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= settings.persistance;
            frequency *= settings.lacunarity;
        }
        return new Vector2(noise, maxValue);
    }
}

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject tree;

    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Tile Settings")]
    public bool isFlatTop = true;
    public float outerSize = 1f;

    [Header("Noise Settings")]
    public string biomeName = "Island";
    public float noiseScale = 1f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;
    public float smoothness = 1f;
    public float height = 1f;

    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private Biome biome;

    [Header("Materials")]
    public Material sand;
    public Material grass;
    public Material stone;

    [Header("Tree Prefabs")]
    public GameObject[] spruce;

    private List<List<GameObject>> tiles = new List<List<GameObject>>();
    private List<GameObject> trees = new List<GameObject>();
    private int number_of_trees = 0;

    void Start()
    {
        noiseSettings = new NoiseSettings(noiseScale, octaves, persistance,
        1f, lacunarity, seed, smoothness, height, 50f);
        switch (biomeName)
        {
            case "Island":
                biome = new Island();
                break;
            case "Boreal Forest":
                biome = new BorealForest();
                break;
            default:
                biome = new Island();
                break;
        }
        noiseSettings.frequency = 1f;
        
        noiseSettings.smoothness *= outerSize * (gridSize.x / 128f);
        noiseSettings.height *= (gridSize.x / 128f);
        if (noiseSettings.seed == 0)
        {
            noiseSettings.seed = Random.Range(0, 100000);
        }
        LayoutGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Regenerate();
        }
        //else
        //{
        //    Regenerate(false);
        //}
    }

    void Regenerate(bool regenerate_seed = true)
    {
        EmptyList();
        number_of_trees = 0;
        if (regenerate_seed)
        {
            noiseSettings.seed = Random.Range(0, 100000);
        }
        LayoutGrid();
    }

    void EmptyList()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Destroy(tiles[y][x]);
            }
        }
        tiles.Clear();

        for (int y = 0; y < number_of_trees; y++)
        {
            Destroy(trees[y]);
        }
        trees.Clear();
    }
    
    private void LayoutGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            tiles.Add(new List<GameObject>());
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = GetPositionForHexFromCoordinates(new Vector2Int(x, y));
                position.y = Mathf.Round(position.y * outerSize / 50) / outerSize * 50;
                GameObject current_tile = Instantiate(tile, position, Quaternion.identity);
                current_tile.transform.parent = gameObject.transform;
                current_tile.name = $"Tile: x:{x}, y:{y}";
                current_tile.transform.localScale = new Vector3(outerSize * 100, outerSize * 100, outerSize * 300);
                current_tile.transform.rotation = Quaternion.Euler(-90, (isFlatTop ? 30 : 0), 0);
                TileType tile_type = TileType.GRASS;

                tile_type = biome.GetTileType(position.y/ outerSize / 25f);

                if (tile_type == TileType.SAND)
                {
                    current_tile.GetComponent<MeshRenderer>().material = sand;
                }
                else if (tile_type == TileType.STONE)
                {
                    current_tile.GetComponent<MeshRenderer>().material = stone;
                }
                else
                {
                    current_tile.GetComponent<MeshRenderer>().material = grass;
                }

                tiles[y].Add(current_tile);
                
                TreeType tree_type = biome.GetTreeType(position.y / outerSize / 25f);
                if (tree_type != TreeType.NO_TREE && TreeNoise(x/15, y/15) * 3 > position.y / outerSize / 25f && position.y + outerSize > 0
                    && tile_type == TileType.GRASS && Random.Range(1, 5) < 3) // TreeNoise(x/15, y/15) * 3 > position.y / 3 - 20
                {
                    if (tree_type == TreeType.SPRUCE)
                    {
                        tree = spruce[Random.Range(0, spruce.Length)];
                    }
                    else // Ajouter les autres arbres
                    {
                        tree = spruce[Random.Range(0, spruce.Length)];
                    }
                    
                    Vector3 pos = new Vector3(position.x, position.y + outerSize * 3f, position.z);
                    GameObject current_tree = Instantiate(tree, pos, Quaternion.identity);
                    current_tree.transform.localScale = new Vector3(outerSize, outerSize * (tree.transform.localScale.y /
                       tree.transform.localScale.x), outerSize * (tree.transform.localScale.z / tree.transform.localScale.x));
                    current_tree.transform.parent = gameObject.transform;
                    current_tree.name = $"Tree: ({x},{y})";
                    current_tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 6) * 60, 0);
                    trees.Add(current_tree);
                    number_of_trees++;
                }
            }
        }
    }
    
    public float TreeNoise(float x, float y) {
        return Mathf.PerlinNoise((x + noiseSettings.seed) * noiseSettings.noiseScale /
            noiseSettings.smoothness, (y + noiseSettings.seed) * noiseSettings.noiseScale / noiseSettings.smoothness);
    }

    public float MountainGeneration(float x, float y) {
        SimpleTerrain terrain = new SimpleTerrain();
        Vector2 base_noise = terrain.SimpleNoise(x / 1.5f, y / 1.5f, noiseSettings);
        float noise = base_noise.x;
        float maxValue = base_noise.y;

        return noise / maxValue * noiseSettings.height * 2.5f;
    }

    public float IslandGeneration(float x, float y, float real_x, float real_y, float center_x = 0.5f, float center_y = 0.5f) {
        SimpleTerrain terrain = new SimpleTerrain();
        Vector2 base_noise = terrain.SimpleNoise(x, y, noiseSettings);
        float noise = base_noise.x;
        float maxValue = base_noise.y;

        float shape = Mathf.PerlinNoise((x + noiseSettings.seed) * noiseSettings.noiseScale /
            noiseSettings.smoothness, (y + noiseSettings.seed) * noiseSettings.noiseScale / noiseSettings.smoothness) + 1;

        float distance = Mathf.Min(GetDistanceBetweenPoints(new Vector2(real_x, real_y), new Vector2(gridSize.x * center_x, gridSize.y * center_y))
                            / (GetDistanceBetweenPoints(new Vector2(0, 0), new Vector2(gridSize.x * center_x, gridSize.y * center_y) * 3f * shape)) * 5.5f, 1);

        noise = noise / maxValue;
        noise = noise - distance;
        
        return Mathf.Max(noise, -1) * noiseSettings.height * 1.5f;
    }

    private float GetDistanceBetweenPoints(Vector2 pointA, Vector2 pointB)
    {
        return Mathf.Sqrt(Mathf.Pow(pointA.x - pointB.x, 2) + Mathf.Pow(pointA.y - pointB.y, 2));
    }
    
    public Vector3 GetPositionForHexFromCoordinates(Vector2Int coordinates)
    {
        int column = coordinates.x;
        int row = coordinates.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize;

        if (!isFlatTop)
        {
            shouldOffset = row % 2 == 0;
            width = Mathf.Sqrt(3f) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * 0.75f;

            offset = shouldOffset ? width / 2f : 0f;

            xPosition = (column * horizontalDistance) + offset;
            yPosition = row * verticalDistance;
        }
        else
        {
            shouldOffset = column % 2 == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3f) * size;

            horizontalDistance = width * 0.75f;
            verticalDistance = height;

            offset = shouldOffset ? height / 2f : 0f;

            xPosition = column * horizontalDistance;
            yPosition = (row * verticalDistance) - offset;
        }

        float y = 0;

        Vector2 tile_pos = new Vector2(xPosition / noiseSettings.smoothness / 25f, -yPosition / noiseSettings.smoothness / 25f);

        if (biomeName == "Island")
        {
            y = IslandGeneration(tile_pos.x, tile_pos.y, coordinates.x, coordinates.y);
        }
        else if (biomeName == "Mountains")
        {
            y = MountainGeneration(tile_pos.x, tile_pos.y);
        }
        else {
            y = biome.BiomeNoise(tile_pos.x, tile_pos.y, noiseSettings);
        }

       // Debug.Log(y);

        return new Vector3(xPosition, y * size * 5, -yPosition);
    }
}
