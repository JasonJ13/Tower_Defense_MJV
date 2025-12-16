using UnityEngine;

public class MapLoader : MonoBehaviour
{

    [SerializeField]
    private int[,] mapArray;
/*
(299, 229, 229) Zone non constructible = 0
(255, 233, 127) Chemin = 1
(255, 178, 127) intersection = 2
(0, 255, 33) Entree = 3
(255, 0, 0) Destination = 4
other Zone constructible = 5
*/

    enum TileType{
        empty,
        road,
        cross,
        start,
        end,
        other
    }

    private TileType[,] mapArcs;


    private int[,] mapTruc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //A faire : Agresser l'utilisateur à choisir une map
        // Convertir la map en mapArray
        //faire d'autre trucs

        mapArray = [ //map Test
                [empty, empty, end, empty, empty],
                [empty, empty, road, empty, empty],
                [start, road, cross, end, empty],
                [empty, empty, empty, empty, empty],
                [empty, empty, empty, empty, empty]
                    ];
        }

    // Update is called once per frame
    void Update()
    {

    }
}
