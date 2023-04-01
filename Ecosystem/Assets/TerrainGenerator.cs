using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tile;

    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Tile Settings")]
    public bool isFlatTop = true;
    public float outerSize = 1f;

    [Header("Noise Settings")]
    public float noiseScale = 1f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;
    public float smoothness = 1f;

    [Header("Materials")]
    public Material grass;
    public Material stone;

    void Start()
    {
        LayoutGrid();
    }

    private void LayoutGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = GetPositionForHexFromCoordinates(new Vector2Int(x, y));
                GameObject current_tile = Instantiate(tile, position, Quaternion.identity);
                current_tile.transform.localScale = new Vector3(outerSize * 100, outerSize * 100, outerSize * 100);
                current_tile.transform.rotation = Quaternion.Euler(-90, (isFlatTop ? 30 : 0), 0);

                if (position.y < 0.6f * outerSize * noiseScale)
                {
                    current_tile.GetComponent<MeshRenderer>().material = grass;
                }
                else
                {
                    current_tile.GetComponent<MeshRenderer>().material = stone;
                }
            }
        }
    }

    public float TerrainNoise(float x, float y) {
        // 3 octaves of noise

        float noise = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++) {
            noise += Mathf.PerlinNoise((x + seed) * noiseScale * frequency, (y + seed) * noiseScale * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        return Mathf.Round(noise / maxValue * noiseScale * 1.5f) / noiseScale / 1.5f;
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

        return new Vector3(xPosition, TerrainNoise(xPosition / smoothness / 25f, -yPosition / smoothness / 25f) * size * 5, -yPosition);
    }
}
