using System;
using System.Collections.Generic;
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

   private static float OFFSET = 0.5f; // Needed since tile position is located on their center

   /// <summary>
   /// used to access the 2DArray mapArray
   /// </summary>
   public struct coords 
   {
      public int row;
      public int column;
      public coords(int row, int column)
      {
         this.row = row;
         this.column = column;
      }

      override public string ToString()
      {
         return "(" + this.row.ToString() + "," + this.column.ToString() + ")";
      }
   }

   /// <summary>
   /// used for listing edges in mapArrayGraphAdj
   /// </summary>
   public struct edge 
   {
      public coords node1;
      public coords node2;

      public int weight;
      public edge(coords node1, coords node2, int weight)
      {
         this.node1 = node1;
         this.node2 = node2;
         this.weight = weight;
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
   private GameObject TileRoad;

   [SerializeField]
   private GameObject TileCross1;

   [SerializeField]
   private GameObject TileCross2;
   [SerializeField]
   private GameObject TileCross3;

   [SerializeField]
   private GameObject TileStart;

   [SerializeField]
   private GameObject TileEnd;

   [SerializeField]
   private GameObject TileConstructible;

   private TileType[,] mapArray; // 2DArray representation of a map

   private int height;
   private int width;

   private Dictionary<coords, TileType> mapGraphNodes; // Dictionary that contains all nodes and their infos
   private List<edge> mapGraphAdj; // List of weighted edges, weight being the length of the road between two nodes



   /// <summary>
   /// Given a coords, return weither it is inside the mapArray or not
   /// </summary>
   /// <param name="pos"></param>
   /// <returns></returns>
   public bool IsInMap(coords pos)
   {
      if (pos.column < 0 || pos.row < 0 || this.mapArray.GetLength(0) <= pos.row || this.mapArray.GetLength(1) <= pos.column)
      {
         return false;
      }
      return true;
   }

   /// <summary>
   /// convert a 3d position vector in coords for the mapArray (ignore the y axis)
   /// </summary>
   /// <param name="position"></param>
   /// <returns></returns>
   public coords PositionToCoords(Vector3 position) 
   {
      return new coords((int)Math.Floor(position.z - OFFSET), (int)Math.Floor(position.x - OFFSET));
   }

   /// <summary>
   /// mapArray is the 2DArray representation of a map
   /// </summary>
   /// <returns></returns>
   public TileType[,] GetMapArray()
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

/// <summary>
/// Dictionary that contains all nodes and their infos
/// </summary>
/// <returns></returns>
   public Dictionary<coords, TileType> GetMapGraphNodes()
   {
      return this.mapGraphNodes;
   }
/// <summary>
/// List of weighted edges, weight being the length of the road between two nodes
/// </summary>
/// <remarks>A node with no edges do not appear in this dictionnary</remarks>
/// <returns></returns>
   public List<edge> GetMapGraphAdj()
   {
      return this.mapGraphAdj;
   }


   // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
   {
      string mapDiskPath = Application.dataPath + "/Maps/map_01.png";
      //A faire : Agresser l'utilisateur à choisir une map
      //faire d'autre trucs

      this.mapArray = LoadMapArray(mapDiskPath);
      this.height = mapArray.GetLength(0);
      this.width = mapArray.GetLength(1);
      this.mapGraphNodes = CreateMapGraphNodes(mapArray);
      this.mapGraphAdj = CreateMapGraphAdj(mapArray, mapGraphNodes);
      CheckMap(mapArray);
      LoadTiles(mapArray);

      foreach (edge edgy in mapGraphAdj)
      {
         Debug.Log(edgy.node1 + "," + edgy.node2 + "," + edgy.weight);
      }

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

   /// <summary>
   /// create the mapArray associated to the png given path.
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   private TileType[,] LoadMapArray(string path)
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

   private Dictionary<coords, TileType> CreateMapGraphNodes(TileType[,] mapArray)
   {
      Dictionary<coords, TileType> mapGraphNodes = new Dictionary<coords, TileType> {}; 
      for (int row = 0; row < mapArray.GetLength(0); row++)
      {
         for (int column = 0; column < mapArray.GetLength(1); column++)
         {
            coords pos = new coords(row, column);
            if (mapArray[row, column] == TileType.start)
            {
               mapGraphNodes[pos] = TileType.start;
            }
            else if (mapArray[row, column] == TileType.end)
            {
               mapGraphNodes[pos] = TileType.end;
            }
            else if (mapArray[row, column] == TileType.cross)
            {
               mapGraphNodes[pos] = TileType.cross;
            }
         }
      }

      return mapGraphNodes;
   }


   /// <summary>
   /// This function creates the given graph of a mapArray
   /// </summary>
   /// <param name="mapArray"></param>
   /// <returns></returns>
   private List<edge> CreateMapGraphAdj(TileType[,] mapArray, Dictionary<coords, TileType> mapGraphNodes)
   { 

      List<edge> mapGraphAdj = new List<edge>();
      bool IsFirstRoadEncountered = true;
      coords firstRoadPosition = new coords(-1,-1);
      int weight = 0;

      // Horizontal sweep
      for (int row = 0; row < mapArray.GetLength(0); row++)
      {
         for (int column = 0; column < mapArray.GetLength(1); column++)
         {
            coords pos = new coords(row, column);
            if (mapArray[row, column] == TileType.road)
            {
               if (IsFirstRoadEncountered)
               {
                  firstRoadPosition = pos;
                  IsFirstRoadEncountered = false;
               }
               else
               {
                  weight++;
               }
            }

            else if (!IsFirstRoadEncountered)
            {
               if (weight > 0)
               {
                  firstRoadPosition.column--;
                  mapGraphAdj.Add(new edge(firstRoadPosition, pos, weight+1));                  
                  mapGraphAdj.Add(new edge(pos, firstRoadPosition, weight+1));
               }

               weight = 0;
               IsFirstRoadEncountered = true;
            }
         }
      }

      // Vertical sweep
      for (int column = 0; column < mapArray.GetLength(1); column++)
      {
         for (int row = 0; row < mapArray.GetLength(0); row++)
         {
            coords pos = new coords(row, column);
            if (mapArray[row, column] == TileType.road)
            {
               if (IsFirstRoadEncountered)
               {
                  firstRoadPosition = pos;
                  IsFirstRoadEncountered = false;
               }
               else
               {
                  weight++;
               }
            }

            else if (!IsFirstRoadEncountered)
            {
               if (weight > 0)
               {
                  firstRoadPosition.row--;
                  mapGraphAdj.Add(new edge(pos, firstRoadPosition, weight+1));                  
                  mapGraphAdj.Add(new edge(firstRoadPosition, pos, weight+1));                  
               }

               weight = 0;
               IsFirstRoadEncountered = true;
            }
         }
      }


      return mapGraphAdj;
   }


   /// <summary>
   /// Verifies the validity of a map
   /// </summary>
   /// <param name="mapArray"></param>
   private void CheckMap(TileType[,] mapArray)
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

   /// <summary>
   /// create the associated Tiles in mapArray in Unity.
   /// </summary>
   /// <param name="mapArray"></param>
   private void LoadTiles(TileType[,] mapArray)
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
                  Instantiate(TileRoad, position, rotation, this.transform);
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
