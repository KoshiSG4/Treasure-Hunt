using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] Player Player;
    [SerializeField] Enemy Enemy;
    [SerializeField] Enemy Enemy1;

    [SerializeField] HealthPotions healthPotions;
    [SerializeField] MagicSpell magicSpell;
    [SerializeField] Weapon Weapon;
    [SerializeField] Trap Trap;
    [SerializeField] Coins Coins;
    [SerializeField] Chest Chest;

    [SerializeField] public int width = 10;
    [SerializeField] public int length = 10;

    [SerializeField] float perlinFrequencyX = 0.1f;
    [SerializeField] float perlinFrequencyZ = 0.1f;
    [SerializeField] float perlinNoiseStrength = 7f;

    [SerializeField] Material material;
    enum TerrainStyle
    {
        TerrainColor,
        BlackToWhite,
        WhiteToBlack,
    }

    [SerializeField] TerrainStyle terrainStyle;

    Gradient TerrainGradient;
    Gradient BlackToWhiteGradient;
    Gradient WhiteToBlackGradient;

    public Vector3[] vertices;
    int[] tris;
    Vector2[] uvs;
    UnityEngine.Color[] colors;
    public float[] noiseArray;
    public Vector3[] worldPointsArray;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    NavMeshSurface navMeshSurface;
    MeshCollider meshCollider;

    float minHeight = 0;
    float maxHeight = 0;

    public LayerMask unwalkableMask;
    // public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] nodeGrid;
    float nodeDiameter;
    public int gridSizeX, gridSizeY = 100;
    public List<Node> path;
    public float noise;


    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Procudural Terrain";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;

        navMeshSurface = GetComponent<NavMeshSurface>();
        meshCollider = GetComponent<MeshCollider>();

        #region Gradient Terrain

        GradientColorKey[] colorKeyTerrain = new GradientColorKey[8];
        colorKeyTerrain[0].color = new UnityEngine.Color(0, 0.086f, 0.35f, 1);
        colorKeyTerrain[0].time = 0.0f;

        colorKeyTerrain[1].color = new UnityEngine.Color(0, 0.135f, 1, 1);
        colorKeyTerrain[1].time = 0.082f;

        colorKeyTerrain[2].color = new UnityEngine.Color(0, 0.735f, 1, 1);
        colorKeyTerrain[2].time = 0.26f;

        colorKeyTerrain[3].color = new UnityEngine.Color(1, 0.91f, 0.5f, 1);
        colorKeyTerrain[3].time = 0.31f;

        colorKeyTerrain[4].color = new UnityEngine.Color(0.06f, 0.31f, 0, 1);
        colorKeyTerrain[4].time = 0.45f;

        colorKeyTerrain[5].color = new UnityEngine.Color(0.31f, 0.195f, 0.11f, 1);
        colorKeyTerrain[5].time = 0.59f;

        colorKeyTerrain[6].color = new UnityEngine.Color(0.41f, 0.41f, 0.41f, 1);
        colorKeyTerrain[6].time = 0.79f;

        colorKeyTerrain[7].color = new UnityEngine.Color(1, 1, 1, 1);
        colorKeyTerrain[7].time = 1.0f;

        GradientAlphaKey[] alphaKeyTerrain = new GradientAlphaKey[2];

        alphaKeyTerrain[0].alpha = 1.0f;
        alphaKeyTerrain[0].time = 0.0f;
        alphaKeyTerrain[1].alpha = 1.0f;
        alphaKeyTerrain[1].time = 1.0f;

        TerrainGradient = new Gradient();

        TerrainGradient.SetKeys(colorKeyTerrain, alphaKeyTerrain);

        #endregion

        #region Black-To-White Gradient Code

        GradientColorKey[] colorKeyBTW = new GradientColorKey[2];

        colorKeyBTW[0].color = new UnityEngine.Color(0, 0, 0, 1);
        colorKeyBTW[0].time = 0.0f;

        colorKeyBTW[1].color = new UnityEngine.Color(1, 1, 1, 1);
        colorKeyBTW[1].time = 1;

        GradientAlphaKey[] alphaKeyBTW = new GradientAlphaKey[2];

        alphaKeyBTW[0].alpha = 1.0f;
        alphaKeyBTW[0].time = 0.0f;

        alphaKeyBTW[1].alpha = 1.0f;
        alphaKeyBTW[1].time = 1.0f;

        BlackToWhiteGradient = new Gradient();

        BlackToWhiteGradient.SetKeys(colorKeyBTW, alphaKeyBTW);

        #endregion

        #region White-To-Black Gradient Code

        GradientColorKey[] colorKeyWTB = new GradientColorKey[2];

        colorKeyWTB[0].color = new UnityEngine.Color(1, 1, 1, 1);
        colorKeyWTB[0].time = 0.0f;

        colorKeyWTB[1].color = new UnityEngine.Color(0, 0, 0, 1);
        colorKeyWTB[1].time = 1;

        GradientAlphaKey[] alphaKeyWTB = new GradientAlphaKey[2];

        alphaKeyWTB[0].alpha = 1.0f;
        alphaKeyWTB[0].time = 0.0f;

        alphaKeyWTB[1].alpha = 1.0f;
        alphaKeyWTB[1].time = 1.0f;

        WhiteToBlackGradient = new Gradient();

        WhiteToBlackGradient.SetKeys(colorKeyWTB, alphaKeyWTB);

        #endregion
        GenerateMeshData();

        CreateTerrain();

        PlaceAssets();

    }
    
    void GenerateMeshData()
    {
        vertices = new Vector3[(width + 1) * (length + 1)];
        noiseArray = new float[(width + 1) * (length + 1)];

        int i = 0;
        for(int z = 0; z <= length; z++)
        {
            for(int x = 0; x <= width; x++)
            {

                float y = Mathf.PerlinNoise(x * perlinFrequencyX, z * perlinFrequencyZ) * perlinNoiseStrength;

                noiseArray[i] = Mathf.Round(y);

                vertices[i] = new Vector3(x, y, z);

                if (y > maxHeight)
                {
                    maxHeight = y;
                }
                if (y < minHeight)
                {
                    minHeight = y;
                }

                i++; 
            }
        }

        tris = new int[width * length * 6];

        int currentTrianglePoint = 0;
        int currentVertexPoint = 0;

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                tris[currentTrianglePoint + 0] = currentVertexPoint + 0;
                tris[currentTrianglePoint + 1] = currentVertexPoint + width + 1;
                tris[currentTrianglePoint + 2] = currentVertexPoint + 1;
                tris[currentTrianglePoint + 3] = currentVertexPoint + 1;
                tris[currentTrianglePoint + 4] = currentVertexPoint + width + 1;
                tris[currentTrianglePoint + 5] = currentVertexPoint + width + 2;

                currentVertexPoint++;
                currentTrianglePoint += 6;
            }
            currentVertexPoint++;
        }

        uvs = new Vector2[vertices.Length];

        i = 0;
        for ( int z = 0; z <= length; z++)
        {
            for ( int x = 0; x <= width; x++)
            {
                uvs[i] = new Vector2((float)x / width, (float)z / length);
                i++;
            }
        }

        colors = new UnityEngine.Color[vertices.Length];
        i = 0;
        for (int z = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);

                switch (terrainStyle)
                {
                    case TerrainStyle.TerrainColor:
                        colors[i] = TerrainGradient.Evaluate(height); break;

                    case TerrainStyle.BlackToWhite:
                        colors[i] = BlackToWhiteGradient.Evaluate(height); break;

                    case TerrainStyle.WhiteToBlack:
                        colors[i] = WhiteToBlackGradient.Evaluate(height);
                        break;
                }
                i++;
            }
        }
    }

    public float GetTerrainHeight(int x, int z)
    {
        float terrainHeight = Mathf.PerlinNoise(x * perlinFrequencyX, z * perlinFrequencyZ) * perlinNoiseStrength;
        return terrainHeight;
    }
    void CreateTerrain()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        navMeshSurface.BuildNavMesh();
    }
    void PlaceAssets()
    {

        Instantiate<HealthPotions>(healthPotions);
        Instantiate<MagicSpell>(magicSpell);
        Instantiate<Chest>(Chest);
        Instantiate<Coins>(Coins);
        Instantiate<Weapon>(Weapon);
        Instantiate<Trap>(Trap);

       /* Instantiate<Player>(Player);
        Instantiate<Enemy>(Enemy);
        Instantiate<Enemy>(Enemy1);*/
    }

}
 