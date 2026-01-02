using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    private enum TileType{
        empty,
        road,
        cross,
        start,
        end,
        constructible
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string mapDiskPath = Application.dataPath + "/Maps/map_01.png";
        //A faire : Agresser l'utilisateur à choisir une map
        // Convertir la map en mapArray
        //faire d'autre trucs

        mapArray = LoadMapArray(mapDiskPath);
        LoadTiles(mapArray);

        TileType[,] mapArrayTest = { //map Test
                {TileType.empty, TileType.empty, TileType.end, TileType.empty, TileType.empty},
                {TileType.empty, TileType.empty, TileType.road, TileType.empty, TileType.empty},
                {TileType.start, TileType.road, TileType.cross, TileType.end, TileType.empty},
                {TileType.empty, TileType.constructible, TileType.empty, TileType.empty, TileType.empty},
                {TileType.empty, TileType.empty, TileType.empty, TileType.empty, TileType.empty}
                };

//        mapArray = mapArrayTest;
    }

    // Update is called once per frame
    void Update(){}

    private TileType[,] LoadMapArray(string path){ // create the MapArray associated to the png given path.

        Texture2D mapImage = new Texture2D(2, 2);
        mapImage.LoadImage(File.ReadAllBytes(path));

        TileType[,] mapArray = new TileType[mapImage.height, mapImage.width];

        Color32[] pixels = mapImage.GetPixels32();
        for (int y = 0; y < mapImage.height; y++)
        {
            for (int x = 0; x < mapImage.width; x++)
            {
                Color32 pixel = pixels[y * mapImage.width + (mapImage.width-1-x)]; // VA SAVOIR POURQUOI mais juste "+ x" ça lis de droite à gauche .-.

                if (pixel.Equals(colorEmpty)){
                   mapArray[y,x] = TileType.empty;
                }
                else if (pixel.Equals(colorRoad)){
                   mapArray[y,x] = TileType.road;
                }
                else if (pixel.Equals(colorCross)){
                   mapArray[y,x] = TileType.cross;
                }
                else if (pixel.Equals(colorStart)){
                   mapArray[y,x] = TileType.start;
                }
                else if (pixel.Equals(colorEnd)){
                   mapArray[y,x] = TileType.end;
                }
                else{
                   mapArray[y,x] = TileType.constructible;
                }


            }
        }


        return mapArray;
    }

    private void LoadTiles(TileType[,] mapArray){ // create the Tiles associated to the MapArray .

        float offset = 0.5f;
        Vector3 position = new Vector3(0,0,0);
        Quaternion rotation = new Quaternion(0,0,0,0);

        for (int y = 0; y < mapArray.GetLength(0); y++)
        {
            for (int x = 0; x < mapArray.GetLength(1); x++)
            {
                position.Set(x+offset, 0, y+offset);

                if (mapArray[y,x] == TileType.empty){
                   Instantiate(TileEmpty, position, rotation, this.transform);
                }
                else if (mapArray[y,x] == TileType.road){
                   Instantiate(TileRoad1, position, rotation, this.transform);
                }
                else if (mapArray[y,x] == TileType.cross){
                   Instantiate(TileCross1, position, rotation, this.transform);
                }
                else if (mapArray[y,x] == TileType.start){
                   Instantiate(TileStart, position, rotation, this.transform);
                }
                else if (mapArray[y,x] == TileType.end){
                   Instantiate(TileEnd, position, rotation, this.transform);
                }
                else{
                   Instantiate(TileConstructible, position, rotation, this.transform);
                }                
            }
        }


    }




}
