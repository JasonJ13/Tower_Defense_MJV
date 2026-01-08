using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemyGraph : MonoBehaviour
{   

    private Map.TileType[,] mapArray;

    private Dictionary<Vector2, (Vector2, int)> adjDict;
    private Dictionary<Vector2, Map.TileType> typeDict;
    
    public void CreateGraph()
    {
        UpdateMap();
        if (mapArray != null)
        {
            //parcours horizontal
            for (int i=0; i < mapArray.Length; i++) 
            {
                //données pour le calcul de chemins
                var previous_tile = Map.TileType.empty;
                var previous_node=Vector2.zero;
                var weightPath = 0;
                var inPath = false;

                for (int j=0;  j < mapArray.GetLength(i); j++)
                {
                    //si on tombe sur un croisement ou début/fin
                    if (mapArray[i,j]==Map.TileType.start || mapArray[i,j]==Map.TileType.end || mapArray[i,j]==Map.TileType.cross)
                    {
                        typeDict.Add(new Vector2(i, j), mapArray[i,j]);

                        //si on se trouve dans un chemin
                        if (inPath)
                        {
                            adjDict.Add(previous_node, (new Vector2(i, j), weightPath)); //ajout de l'adjacence
                            
                            //reset du chemin
                            inPath = false;
                            weightPath = 0;
                        }

                        //maj de la de la dernière node
                        //previous_node =

                    }
                    
                }
            }

        }

    }

    private void UpdateMap()
    {

        this.mapArray = Map.Instance.GetMapArray();

    }


    
    



    
    




}
