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
         Debug.LogError("Error : Instance of Map already exists");
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

      public override string ToString()
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
      return new coords((int)Math.Floor(position.z), (int)Math.Floor(position.x));
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
      return this.mapArray[pos.row, pos.column];
   }

   public void SetMapArray((int column, int row) pos, TileType type)
   {
      this.mapArray[pos.row, pos.column] = type;
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
      string mapDiskPath = Application.dataPath + "/Maps/map_02.png";
      //A faire : Agresser l'utilisateur à choisir une map
      //faire d'autre trucs

      this.mapArray = LoadMapArray(mapDiskPath);
      this.height = mapArray.GetLength(0);
      this.width = mapArray.GetLength(1);
      this.mapGraphNodes = CreateMapGraphNodes();
      this.mapGraphAdj = CreateMapGraphAdj(mapGraphNodes);
      CheckMap();
      LoadTiles();

//      foreach (edge edgy in mapGraphAdj){Debug.Log(edgy.node1 + "," + edgy.node2 + "," + edgy.weight);}

      TileType[,] mapArrayTest =
      { //map Test
         { TileType.empty, TileType.empty, TileType.end, TileType.empty, TileType.empty },
         { TileType.empty, TileType.empty, TileType.road, TileType.empty, TileType.empty },
         { TileType.start, TileType.road, TileType.cross, TileType.end, TileType.empty },
         { TileType.empty, TileType.constructible, TileType.empty, TileType.empty, TileType.empty },
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

   private Dictionary<coords, TileType> CreateMapGraphNodes()
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
   private List<edge> CreateMapGraphAdj(Dictionary<coords, TileType> mapGraphNodes)
   { 

      List<edge> mapGraphAdj = new();
      bool OnAPath = false;
      coords lastCrossPosition = new coords(-1,-1);
      int weight = 0;

      // Horizontal sweep
      for (int row = 0; row < mapArray.GetLength(0); row++)
      {
         for (int column = 0; column < mapArray.GetLength(1); column++)
         {
            coords pos = new coords(row, column);
            if (OnAPath && mapArray[row, column] == TileType.road)
            {
               weight++;
            }
            else if (OnAPath && (mapArray[row, column] == TileType.start || mapArray[row, column] == TileType.end || mapArray[row, column] == TileType.cross)) //Si on est 
            {
               mapGraphAdj.Add(new edge(lastCrossPosition, pos, weight));                  
               mapGraphAdj.Add(new edge(pos, lastCrossPosition, weight));
               weight = 0;
            }
            else{
               OnAPath = false;
            }

            if (mapArray[row, column] == TileType.start || mapArray[row, column] == TileType.end || mapArray[row, column] == TileType.cross)
            {
               OnAPath = true;
               lastCrossPosition = pos;
            }
         }
         OnAPath = false;
      }

      weight = 0;
      OnAPath = false;
      // Vertical sweep
      for (int column = 0; column < mapArray.GetLength(1); column++)
      {
         for (int row = 0; row < mapArray.GetLength(0); row++)
         {
            coords pos = new coords(row, column);
            if (OnAPath && mapArray[row, column] == TileType.road)
            {
               weight++;
            }
            else if (OnAPath && (mapArray[row, column] == TileType.start || mapArray[row, column] == TileType.end || mapArray[row, column] == TileType.cross)) //Si on est 
            {
               mapGraphAdj.Add(new edge(lastCrossPosition, pos, weight));                  
               mapGraphAdj.Add(new edge(pos, lastCrossPosition, weight));
               weight = 0;
            }
            else{
               OnAPath = false;
            }

            if (mapArray[row, column] == TileType.start || mapArray[row, column] == TileType.end || mapArray[row, column] == TileType.cross)
            {
               OnAPath = true;
               lastCrossPosition = pos;
            }
         }  
         OnAPath = false;
      }
   

      return mapGraphAdj;
   }


   /// <summary>
   /// Verifies the validity of a map
   /// </summary>
   /// <param name="mapArray"></param>
   private void CheckMap()
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


   private Quaternion CorrectRotation(coords pos)
   {
      TileType tile = mapArray[pos.row, pos.column];
      if (!(tile == TileType.road || tile == TileType.cross))
      {
         return new Quaternion(0, 0, 0, 0);        
      }
      if (tile == TileType.road)
      {
         pos.row--;
         if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column] == TileType.cross))
         {
            return new Quaternion(0, 0, 0, 0);
         }
         else
         {
            return Quaternion.AngleAxis(90, Vector3.up);
         }
      }

      // If we are here, it means tile = Tiletype.cross
      bool rightNeighboor = false;
      bool leftNeighboor = false;
      bool upNeighboor = false;
      bool downNeighboor = false;
      int count = 0;
      pos.row--;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column] == TileType.cross))
      {
         downNeighboor = true;
         count++;         
      }
      pos.row++;
      pos.row++;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         upNeighboor = true;
         count++;
      }
      pos.row--;
      pos.column--;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         leftNeighboor = true;
         count++;
      }
      pos.column++;
      pos.column++;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         rightNeighboor = true;
         count++;
      }
      pos.column--;
      if (count == 4)
      {
         return new Quaternion(0, 0, 0, 0);
      }
      if (count == 3)
      {
         Debug.Log("testest : " + pos);
         Debug.Log(downNeighboor);
         if (downNeighboor == false)
         {
         return Quaternion.AngleAxis(0, Vector3.up);            
         }
         if (leftNeighboor == false)
         {
         return Quaternion.AngleAxis(90, Vector3.up);            
         }
         if (upNeighboor == false)
         {
         return Quaternion.AngleAxis(180, Vector3.up);            
         }
         if (rightNeighboor == false)
         {
         return Quaternion.AngleAxis(-90, Vector3.up);            
         }
      }
      if (count == 2)
      {
         if (leftNeighboor == true && upNeighboor == true)
         {
         return Quaternion.AngleAxis(0, Vector3.up);            
         }
         if (upNeighboor == true && rightNeighboor == true)
         {
         return Quaternion.AngleAxis(90, Vector3.up);            
         }
         if (rightNeighboor == true && downNeighboor == true)
         {
         return Quaternion.AngleAxis(180, Vector3.up);            
         }
         if (downNeighboor == true && leftNeighboor == true)
         {
         return Quaternion.AngleAxis(-90, Vector3.up);            
         }
      }


      Debug.LogError("Cross have not correct neighboors placement");   
      return new Quaternion(0, 0, 0, 0);
   }

   private GameObject CorrectCrossTile(coords pos)
   {
      int count = 0;
      pos.row--;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column] == TileType.cross))
      {
         count++;
      }
      pos.row++;  
      pos.row++;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         count++;
      }
      pos.row--;
      pos.column--;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         count++;
      }
      pos.column++;
      pos.column++;
      if (IsInMap(pos) && (mapArray[pos.row, pos.column]==TileType.start || mapArray[pos.row, pos.column]==TileType.end || mapArray[pos.row, pos.column]==TileType.road || mapArray[pos.row, pos.column]==TileType.cross)){
         count++;
      }
      pos.column--;
      
      if (count == 2)
      {
         return TileCross1;
      }
      if (count == 3)
      {
         return TileCross2;
      }
      if (count == 4)
      {
         return TileCross3;         
      }
      Debug.LogError("Cross have less than 2 correct neighboors");
      return TileEmpty;    
   }

   /// <summary>
   /// create the associated Tiles in mapArray in Unity.
   /// </summary>
   /// <param name="mapArray"></param>
   private void LoadTiles()
   {
      Vector3 position = new(0, 0, 0);
      coords pos = new(0, 0);
      for (int row = 0; row < mapArray.GetLength(0); row++)
      {
         pos.row = row;
         for (int column = 0; column < mapArray.GetLength(1); column++)
         {
            pos.column = column;
            position.Set(column + OFFSET, 0, row + OFFSET);
            if (mapArray[row, column] == TileType.empty)
            {
               Instantiate(TileEmpty, position, CorrectRotation(pos), this.transform);
            }
            else if (mapArray[row, column] == TileType.road)
            {
               Instantiate(TileRoad, position, CorrectRotation(pos), this.transform);
            }
            else if (mapArray[row, column] == TileType.cross)
            {
               Instantiate(CorrectCrossTile(pos), position, CorrectRotation(pos), this.transform);
            }
            else if (mapArray[row, column] == TileType.start)
            {
               Instantiate(TileStart, position, CorrectRotation(pos), this.transform);
            }
            else if (mapArray[row, column] == TileType.end)
            {
               Instantiate(TileEnd, position, CorrectRotation(pos), this.transform);
            }
            else
            {
               Instantiate(TileConstructible, position, CorrectRotation(pos), this.transform);
            }
         }
      }
   }
}
