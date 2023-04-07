using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public char biome = 'I';
    public float noiseScale = 1f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;
    public float smoothness = 1f;
    public float height = 1f;

    [Header("Materials")]
    public Material sand;
    public Material grass;
    public Material stone;

    [Header("Tree Prefabs")]
    public GameObject[] spruce;

    //private System.Random random_seed = new System.Random();
    private List<List<GameObject>> tiles = new List<List<GameObject>>();
    private List<GameObject> trees = new List<GameObject>();
    private int number_of_trees = 0;

    private int SAND = 0;
    private int GRASS = 1;
    private int STONE = 2;

    void Start()
    {
        smoothness *= outerSize * (gridSize.x / 128f);
        height *= (gridSize.x / 128f);
        if (seed == 0)
        {
            seed = Random.Range(0, 100000);
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
            seed = Random.Range(0, 100000);
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
                current_tile.transform.localScale = new Vector3(outerSize * 100, outerSize * 100, outerSize * 100);
                current_tile.transform.rotation = Quaternion.Euler(-90, (isFlatTop ? 30 : 0), 0);
                int tile_type = GRASS;

                if (position.y < -0.05f * outerSize * noiseScale * height * (1 + Random.Range(-0.2f, 0.2f)))
                {
                    current_tile.GetComponent<MeshRenderer>().material = sand;
                    tile_type = SAND;
                }
                else if (position.y < 0.5f * outerSize * noiseScale * height * (1 + Random.Range(-0.2f, 0.2f)))
                {
                    current_tile.GetComponent<MeshRenderer>().material = grass;
                    tile_type = GRASS;
                }
                else
                {
                    current_tile.GetComponent<MeshRenderer>().material = stone;
                    tile_type = STONE;
                }

                tiles[y].Add(current_tile);

                tree = spruce[0]; //Random.Range(0, treePrefabs.Length)];
                
                if (TreeNoise(x/15, y/15) * 3 > position.y / 3 - 20 && position.y + outerSize > 0
                    && tile_type == GRASS && Random.Range(1, 5) < 3)
                {
                    Vector3 pos = new Vector3(position.x, position.y + outerSize * 3f, position.z);
                    GameObject current_tree = Instantiate(tree, pos, Quaternion.identity);
                    current_tree.transform.localScale = new Vector3(outerSize, outerSize * (tree.transform.localScale.y /
                        tree.transform.localScale.x), outerSize * (tree.transform.localScale.z / tree.transform.localScale.x));
                    trees.Add(current_tree);
                    number_of_trees++;
                }
            }
        }
    }
    
    public float TreeNoise(float x, float y) {
        return Mathf.PerlinNoise((x + seed) * noiseScale / smoothness, (y + seed) * noiseScale / smoothness);
    }

    public Vector2 SimpleNoise(float x, float y) {
        float noise = 0;
        float frequency = 1;
        float amplitude = 50;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++) {
            noise += Mathf.PerlinNoise((x + seed) * noiseScale * frequency, (y + seed) * noiseScale * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        return new Vector2(noise, maxValue);
    }

    public float MountainGeneration(float x, float y) {
        Vector2 base_noise = SimpleNoise(x / 1.5f, y / 1.5f);
        float noise = base_noise.x;
        float maxValue = base_noise.y;

        return noise / maxValue * height * 2.5f;
    }

    public float IslandGeneration(float x, float y, float real_x, float real_y, float center_x = 0.5f, float center_y = 0.5f) {
        Vector2 base_noise = SimpleNoise(x, y);
        float noise = base_noise.x;
        float maxValue = base_noise.y;

        float shape = Mathf.PerlinNoise((x + seed) * noiseScale / smoothness, (y + seed) * noiseScale / smoothness) + 1;

        float distance = Mathf.Min(GetDistanceBetweenPoints(new Vector2(real_x, real_y), new Vector2(gridSize.x * center_x, gridSize.y * center_y))
                            / (GetDistanceBetweenPoints(new Vector2(0, 0), new Vector2(gridSize.x * center_x, gridSize.y * center_y) * 3f * shape)) * 5.5f, 1);

        noise = noise / maxValue;
        noise = noise - distance;
        
        return Mathf.Max(noise, -1) * height * 1.5f;
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

        Vector2 tile_pos = new Vector2(xPosition / smoothness / 25f, -yPosition / smoothness / 25f);

        if (biome == 'I')
        {
            y = IslandGeneration(tile_pos.x, tile_pos.y, coordinates.x, coordinates.y);
        }
        else if (biome == 'M')
        {
            y = MountainGeneration(tile_pos.x, tile_pos.y);
        }

        return new Vector3(xPosition, y * size * 5, -yPosition);
    }
}
