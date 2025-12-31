using UnityEngine;

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


    [SerializeField]
    private TileType[,] mapArray;
/*
(299, 229, 229) Zone non constructible = 0 Tile asset : snow-tile
(255, 233, 127) Chemin = 1 Tile asset : snow-tile-straight/square
(255, 178, 127) intersection = 2 Tile asset : snow-tile-split/crossing
(0, 255, 33) Entree = 3 Tile asset : snow-tile-end
(255, 0, 0) Destination = 4 Tile asset : snow-tile-end-round
other Zone constructible = 5 Tile asset : snow-tile-hill
*/

    private TileType[,] mapArcs;


    private int[,] mapTruc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //A faire : Agresser l'utilisateur à choisir une map
        // Convertir la map en mapArray
        //faire d'autre trucs

        TileType[,] mapArrayTest = { //map Test
                {TileType.empty, TileType.empty, TileType.end, TileType.empty, TileType.empty},
                {TileType.empty, TileType.empty, TileType.road, TileType.empty, TileType.empty},
                {TileType.start, TileType.road, TileType.cross, TileType.end, TileType.empty},
                {TileType.empty, TileType.constructible, TileType.empty, TileType.empty, TileType.empty},
                {TileType.empty, TileType.empty, TileType.empty, TileType.empty, TileType.empty}
        };

        mapArray = mapArrayTest;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
