using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemyGraph : MonoBehaviour
{   

    private Map.TileType[,] mapArray;

    private Dictionary<Map.coords, (Map.coords, int)> adjDict;
    private Dictionary<Map.coords, Map.TileType> typeDict;



    private void UpdateMap()
    {

        this.mapArray = Map.Instance.GetMapArray();

    }

    public void CreateGraph()
    {
        UpdateMap();
        if (mapArray != null)
        {
            //parcours horizontal
            for (int i=0; i < mapArray.Length; i++) 
            {
                //données pour le calcul de chemins
                var previous_tile_is_node = false;
                var previous_node= new Map.coords(-1,-1);
                var weightPath = 0;
                var inPath = false;

                for (int j=0;  j < mapArray.GetLength(i); j++)
                {
                    //si on tombe sur un croisement ou début/fin
                    if (mapArray[i,j]==Map.TileType.start || mapArray[i,j]==Map.TileType.end || mapArray[i,j]==Map.TileType.cross)
                    {
                        typeDict.Add(new Map.coords(i, j), mapArray[i,j]);

                        //si on se trouve dans un chemin
                        if (inPath)
                        {
                            adjDict.Add(previous_node, (new Map.coords(i, j), weightPath)); //ajout de l'adjacence
                            
                            //reset du chemin
                            inPath = false;
                            weightPath = 0;
                        }

                        //maj de la de la dernière node
                        previous_node = new Map.coords(i, j);
                        previous_tile_is_node= true;

                    } 
                    //si on tombe sur un chemin
                    else if (mapArray[i,j]==Map.TileType.road)
                    {
                        //si on sort d'un noeud ou début/fin ou si on est déjà dans un chemin
                        if (previous_tile_is_node || inPath)
                        {
                            inPath = true; //on entre dans un chemin
                            weightPath++;

                            //parcours des cases au dessus voir si il y a une tour construite
                            for (int k=1; k<2; k++)
                            {
                                if (i-k >= 0)
                                {
                                    if (mapArray[i-k,j]==Map.TileType.construct)
                                    {
                                        weightPath += 2;
                                    }
                                
                                }
                                
                                if (i+k<mapArray.Length)
                                {
                                    if (mapArray[i+k,j]==Map.TileType.construct)
                                    {
                                        weightPath += 2;
                                    }
                                }
                            }

                        } 

                        //mis à jour de la previous tile
                        previous_tile_is_node = false;

                    } else //si on tombe sur une autre case
                    {
                        previous_tile_is_node = false;
                    }
                    
                }

               
            }

            //parcours vertical
            for (int j=0; j < mapArray.GetLength(0); j++)
            {

                //données pour le calcul de chemins
                var previous_tile_is_node = false;
                var previous_node = new Map.coords(-1, -1);
                var weightPath = 0;
                var inPath = false;


                for (int i=0; i<mapArray.Length; i++)
                {
                    //si on tombe sur un croisement ou début/fin
                    if (mapArray[i, j] == Map.TileType.start || mapArray[i, j] == Map.TileType.end || mapArray[i, j] == Map.TileType.cross)
                    {
                        typeDict.Add(new Map.coords(i, j), mapArray[i, j]);

                        //si on se trouve dans un chemin
                        if (inPath)
                        {
                            adjDict.Add(previous_node, (new Map.coords(i, j), weightPath)); //ajout de l'adjacence

                            //reset du chemin
                            inPath = false;
                            weightPath = 0;
                        }

                        //maj de la de la dernière node
                        previous_node = new Map.coords(i, j);
                        previous_tile_is_node = true;

                    }
                    //si on tombe sur un chemin
                    else if (mapArray[i, j] == Map.TileType.road)
                    {
                        //si on sort d'un noeud ou début/fin ou si on est déjà dans un chemin
                        if (previous_tile_is_node || inPath)
                        {
                            inPath = true; 
                            weightPath++;

                            //parcours des cases à côté voir si il y a une tour construite
                            for (int k = 1; k < 2; k++)
                            {
                                if (j - k >= 0)
                                {
                                    if (mapArray[i , j-k] == Map.TileType.construct)
                                    {
                                        weightPath += 2;
                                    }

                                }

                                if (j + k < mapArray.GetLength(0))
                                {
                                    if (mapArray[i, j+k] == Map.TileType.construct)
                                    {
                                        weightPath += 2;
                                    }
                                }
                            }

                        }

                        //mis à jour de la previous tile
                        previous_tile_is_node = false;

                    }
                    else //si on tombe sur une autre case
                    {
                        previous_tile_is_node = false;
                    }

                }
            }

        }

       



    }

    




    
    



    
    




}
