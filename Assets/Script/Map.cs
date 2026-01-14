using System;
using System.IO;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    private void Awake()
    {
        if (Map.Instance != null)
        {
            Debug.LogError("Error : Instance of App already exists");
        }
        Map.Instance = this;
    }

    public enum TileType
    {
        empty,
        road,
        cross,
        start,
        end,
        constructible,
        construct,
    }

    private static float OFFSET = 0.5f; // Needed since tiles position is on their center

    public struct coords
    {
        public int column;
        public int row;

        public coords(int c, int r)
        {
            column = c;
            row = r;
        }
    }

    /*
    (229, 229, 229) Zone non constructible = 0 Tile asset : snow-tile-hill
    (255, 233, 127) Chemin = 1 Tile asset : snow-tile-straight/square
    (255, 178, 127) intersection = 2 Tile asset : snow-tile-split/crossing
    (0, 255, 33) Entree = 3 Tile asset : snow-tile-end
    (255, 0, 0) Destination = 4 Tile asset : snow-tile-end-round
    other Zone constructible = 5 Tile asset : snow-tile
    */

    private Color32 colorEmpty = new Color32(229, 229, 229, 255);
    private Color32 colorRoad = new Color32(255, 233, 127, 255);
    private Color32 colorCross = new Color32(255, 178, 127, 255);
    private Color32 colorStart = new Color32(0, 255, 33, 255);
    private Color32 colorEnd = new Color32(255, 0, 0, 255);

    [SerializeField]
    private GameObject TileCollider;

    [SerializeField]
    private GameObject TileEmpty; // visual of the tile

    [SerializeField]
    private GameObject TileRoad1;

    [SerializeField]
    private GameObject TileRoad2;

    [SerializeField]
    private GameObject TileCross1;

    [SerializeField]
    private GameObject TileCross2;

    [SerializeField]
    private GameObject TileStart;

    [SerializeField]
    private GameObject TileEnd;

    [SerializeField]
    private GameObject TileConstructible;

    private TileType[,] mapArray; // 2DArray representation of a map

    private TileType[,] mapArcs;

    private int[,] mapTruc;

    public bool IsInMap(coords pos)
    {
        if (
            pos.column < 0
            || pos.row < 0
            || this.mapArray.GetLength(0) <= pos.row
            || this.mapArray.GetLength(1) <= pos.column
        )
        {
            return false;
        }
        return true;
    }

    public coords PositionToCoords(Vector3 position) // convert 3d position in coords for the mapArray
    {
        return new coords(
            (int)Math.Floor(position.z - OFFSET),
            (int)Math.Floor(position.x - OFFSET)
        );
    }

    public TileType[,] GetMapArray() // MapArray is the 2DArray representation of a map
    {
        return this.mapArray;
    }

    public TileType GetMapArrayCoords(coords pos)
    {
        return this.mapArray[pos.column, pos.row];
    }

    public void SetMapArray((int column, int row) pos, TileType type)
    {
        this.mapArray[pos.column, pos.row] = type;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        string mapDiskPath = Application.dataPath + "/Maps/map_01.png";
        //A faire : Agresser l'utilisateur à choisir une map
        // Convertir la map en mapArray
        //faire d'autre trucs

        mapArray = LoadMapArray(mapDiskPath);
        CheckMap(mapArray);
        LoadTiles(mapArray);

        TileType[,] mapArrayTest =
        { //map Test
            { TileType.empty, TileType.empty, TileType.end, TileType.empty, TileType.empty },
            { TileType.empty, TileType.empty, TileType.road, TileType.empty, TileType.empty },
            { TileType.start, TileType.road, TileType.cross, TileType.end, TileType.empty },
            {
                TileType.empty,
                TileType.constructible,
                TileType.empty,
                TileType.empty,
                TileType.empty,
            },
            { TileType.empty, TileType.empty, TileType.empty, TileType.empty, TileType.empty },
        };

        //        mapArray = mapArrayTest;
    }

    private TileType[,] LoadMapArray(string path) // create the MapArray associated to the png given path.
    {
        Texture2D mapImage = new Texture2D(2, 2);
        mapImage.LoadImage(File.ReadAllBytes(path));

        TileType[,] mapArray = new TileType[mapImage.height, mapImage.width];

        Color32[] pixels = mapImage.GetPixels32();
        for (int row = 0; row < mapImage.height; row++)
        {
            for (int column = 0; column < mapImage.width; column++)
            {
                Color32 pixel = pixels[row * mapImage.width + column]; // L'image se lit déjà de bas en haut et de gauche apparemment ?

                if (pixel.Equals(colorEmpty))
                {
                    mapArray[row, column] = TileType.empty;
                }
                else if (pixel.Equals(colorRoad))
                {
                    mapArray[row, column] = TileType.road;
                }
                else if (pixel.Equals(colorCross))
                {
                    mapArray[row, column] = TileType.cross;
                }
                else if (pixel.Equals(colorStart))
                {
                    mapArray[row, column] = TileType.start;
                }
                else if (pixel.Equals(colorEnd))
                {
                    mapArray[row, column] = TileType.end;
                }
                else
                {
                    mapArray[row, column] = TileType.constructible;
                }
            }
        }

        return mapArray;
    }

    private void CheckMap(TileType[,] mapArray) //Verify the validity of a map
    {
        bool endExist = false;
        bool startExist = false;
        for (int row = 0; row < mapArray.GetLength(0); row++)
        {
            for (int column = 0; column < mapArray.GetLength(1); column++)
            {
                if (mapArray[row, column] == TileType.start)
                {
                    startExist = true;
                }
                if (mapArray[row, column] == TileType.end)
                {
                    endExist = true;
                }
            }
        }
    }

    private void LoadTiles(TileType[,] mapArray) // create the Tiles associated to the MapArray .
    {
        Vector3 position = new Vector3(0, 0, 0);
        Quaternion rotation = new Quaternion(0, 0, 0, 0);

        for (int row = 0; row < mapArray.GetLength(0); row++)
        {
            for (int column = 0; column < mapArray.GetLength(1); column++)
            {
                position.Set(column + OFFSET, 0, row + OFFSET);

                if (mapArray[row, column] == TileType.empty)
                {
                    Instantiate(TileEmpty, position, rotation, this.transform);
                }
                else if (mapArray[row, column] == TileType.road)
                {
                    Instantiate(TileRoad1, position, rotation, this.transform);
                }
                else if (mapArray[row, column] == TileType.cross)
                {
                    Instantiate(TileCross1, position, rotation, this.transform);
                }
                else if (mapArray[row, column] == TileType.start)
                {
                    Instantiate(TileStart, position, rotation, this.transform);
                }
                else if (mapArray[row, column] == TileType.end)
                {
                    Instantiate(TileEnd, position, rotation, this.transform);
                }
                else
                {
                    Instantiate(TileConstructible, position, rotation, this.transform);
                }
            }
        }
    }
}
