using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


public class Map : MonoBehaviour
{
   public static Map Instance { get; private set; }
   public static string mapDiskPath;
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
      generator,
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


      public bool Equals(coords pos2)
      {
         return this.row == pos2.row && this.column == pos2.column;
      }

      public override string ToString()
      {
         return "(" + this.row.ToString() + "," + this.column.ToString() + ")";
      }
   }
   public static int CompareCoords(coords c1, coords c2)
   {
      if (Map.Instance.GetDistanceFromEnd(c1) < Map.Instance.GetDistanceFromEnd(c2))
      {
         return -1;
      }
      else if (Map.Instance.GetDistanceFromEnd(c1) == Map.Instance.GetDistanceFromEnd(c2))
      {
         return 0;
      }
      else
      {
         return 1;
      }
   }


   private Color32 colorEmpty = new Color32(229, 229, 229, 255);
   private Color32 colorRoad = new Color32(255, 233, 127, 255);
   private Color32 colorCross = new Color32(255, 178, 127, 255);
   private Color32 colorStart = new Color32(255, 0, 0, 255);
   private Color32 colorEnd = new Color32(0, 255, 33, 255);

   [SerializeField]
   private GameObject TileCollider;

   [SerializeField]
   private GameObject TileEmpty; 

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

   private Graph graph;


   public bool IsInMap(coords pos)
   {
      if (pos.column < 0 || pos.row < 0 || this.mapArray.GetLength(0) <= pos.row || this.mapArray.GetLength(1) <= pos.column)
      {
         return false;
      }
      return true;
   }

   public coords PositionToCoords(Vector3 position) 
   {
      return new coords((int)Math.Floor(position.z), (int)Math.Floor(position.x));
   }

   public Vector3 CoordsToPosition(coords pos) 
   {
      return new Vector3(pos.column + OFFSET, 0, pos.row + OFFSET);
   }

   public TileType[,] GetMapArray()
   {
      return this.mapArray;
   }

   /// <summary>
   /// mapArray is the 2DArray representation of a map, access by coordss
   /// </summary>
   /// <returns></returns>
   public TileType GetMapArrayCoords(coords pos)
   {
      Debug.Assert(IsInMap(pos));
      return this.mapArray[pos.row, pos.column];
   }

   public void SetMapArray(coords pos, TileType type)
   {
      Debug.Assert(IsInMap(pos));
      this.mapArray[pos.row, pos.column] = type;
   }

   public Graph GetGraph()
   {
      return this.graph;
   }


/// <summary>
/// </summary>
/// <remarks>pos must a road tile, since cross/start/end tiles might have multiple edges</remarks>
/// <param name="pos"></param>
/// <returns></returns>
   public Graph.edge FindEdge(coords pos)
   {
      
      Debug.Assert(GetMapArrayCoords(pos)==TileType.road);
      coords node1 = new(-1,-1);
      coords node2 = new(-1,-1);
      node1 = pos;
      node2 = pos;
      node1.row--;
      if (IsInMap(node1) && GetMapArrayCoords(node1)==TileType.road || GetMapArrayCoords(node1)==TileType.cross || GetMapArrayCoords(node1)==TileType.start || GetMapArrayCoords(node1)==TileType.end)
      //if horizontal road
      {
         node1=pos;
         while (!(GetMapArrayCoords(node1)==TileType.cross || GetMapArrayCoords(node1)==TileType.start || GetMapArrayCoords(node1) == TileType.end))
         {
            node1.row--;
         }
         node2 = pos;
         while (!(GetMapArrayCoords(node2)==TileType.cross || GetMapArrayCoords(node2)==TileType.start || GetMapArrayCoords(node2) == TileType.end))
         {
            node2.row++;
         }
      }
      else
      //if vertical road
      {
         node1=pos;
         while (!(GetMapArrayCoords(node1)==TileType.cross || GetMapArrayCoords(node1)==TileType.start || GetMapArrayCoords(node1) == TileType.end))
         {
            node1.column--;
         }
         node2 = pos;
         while (!(GetMapArrayCoords(node2)==TileType.cross || GetMapArrayCoords(node2)==TileType.start || GetMapArrayCoords(node2) == TileType.end))
         {
            node2.column++;
         }
      }
      List<Graph.edge> edges = graph.GetEdges();
      foreach (Graph.edge edgy in edges)
      {
         if (edgy.node1.Equals(node1) && edgy.node2.Equals(node2))
         {
            return edgy;
         }
         if (edgy.node1.Equals(node1) && edgy.node2.Equals(node2))
         {
            return edgy;            
         }
      }
      
      Debug.LogError("edge not found for pos : " + pos.ToString() + "\n Here what nodes where found :" + node1.ToString() + node2.ToString());
      return new Graph.edge();


   }


/// <summary>
/// 
/// </summary>
/// <remarks>pos must be a start, end, cross or road tile</remarks>
/// <param name="pos"></param>
/// <returns></returns>
   public int GetDistanceFromEnd(coords pos)
   {
      if (this.graph.GetNodes().Contains(pos))
      {
         return this.graph.GetDistanceFromEnd(pos);
      }

      Debug.Assert(GetMapArrayCoords(pos)==TileType.road);
      coords nearestEnd = new(-1,-1);
      coords posTest = pos;
      posTest.row--;
      int lengthToEnd1=0;
      int lengthToEnd2=0;
      if (IsInMap(posTest) && GetMapArrayCoords(posTest)==TileType.road || GetMapArrayCoords(posTest)==TileType.cross || GetMapArrayCoords(posTest)==TileType.start || GetMapArrayCoords(posTest)==TileType.end)
      //if horizontal road
      {
         posTest=pos;
         while (!(GetMapArrayCoords(posTest)==TileType.cross || GetMapArrayCoords(posTest)==TileType.start || GetMapArrayCoords(posTest) == TileType.end))
         {
            lengthToEnd1++;
            posTest.row--;
         }
         lengthToEnd1= lengthToEnd1 + this.graph.GetDistanceFromEnd(posTest);
         posTest = pos;
         while (!(GetMapArrayCoords(posTest)==TileType.cross || GetMapArrayCoords(posTest)==TileType.start || GetMapArrayCoords(posTest) == TileType.end))
         {
            lengthToEnd2++;
            posTest.row++;
         }
         lengthToEnd2= lengthToEnd2 + this.graph.GetDistanceFromEnd(posTest);
      }
      else
      //if vertical road
      {
         posTest=pos;         
         while (!(GetMapArrayCoords(posTest)==TileType.cross || GetMapArrayCoords(posTest)==TileType.start || GetMapArrayCoords(posTest) == TileType.end))
         {
            lengthToEnd1++;
            posTest.column--;
         }
         lengthToEnd1= lengthToEnd1 + this.graph.GetDistanceFromEnd(posTest);
         posTest = pos;
         while (!(GetMapArrayCoords(posTest)==TileType.cross || GetMapArrayCoords(posTest)==TileType.start || GetMapArrayCoords(posTest) == TileType.end))
         {
            lengthToEnd2++;
            posTest.column++;
         }
         lengthToEnd2= lengthToEnd2 + this.graph.GetDistanceFromEnd(posTest);
      }
      return Math.Min(lengthToEnd1, lengthToEnd2);
   }



   // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
   {
      if (mapDiskPath == null){mapDiskPath = EditorUtility.OpenFilePanel("Map Loader", Application.dataPath + "/Maps/", "png");}

      this.mapArray = LoadMapArray(Map.mapDiskPath);
      this.height = mapArray.GetLength(0);
      this.width = mapArray.GetLength(1);
      CheckMap();
      this.graph = new();
      this.graph.CreateGraph(mapArray);
      LoadTiles();
      foreach (coords node1 in this.graph.GetNodes())
      {
         foreach (coords node2 in this.graph.GetNodes())
         {
//            Debug.Log(this.graph.GetPathWeight(node1, node2));
//            Debug.Log(this.graph.GetPath(node1, node2).Count);
         }
//         Debug.Log(this.graph.GetNodeInfos(node1).distanceFromEnd);
      }
      

//      foreach (edge edgy in mapGraphAdj){Debug.Log(edgy.node1 + "," + edgy.node2 + "," + edgy.weight);}

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

   /// <summary>
   /// Verifies the validity of a map, Abandonned
   /// </summary>
   /// <param name="mapArray"></param>
   private void CheckMap()
   {

//      bool endExist = false;
//      bool startExist = false;
      for (int row = 0; row < mapArray.GetLength(0); row++)
      {
         for (int column = 0; column < mapArray.GetLength(1); column++)
         {
               if (mapArray[row, column] == TileType.start)
               {
//                  startExist = true;
               }
               if (mapArray[row, column] == TileType.end)
               {
//                  endExist = true;
               }
         }
      }

      return;
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

   public class Graph
   {
      private Dictionary<coords, nodeInfos> dictNodes; // Dictionnary that contains all infos for a given node (ie coords)
      private List<edge> edges; // List of weighted edges, weight being the length of the road between two nodes
      private int[,] matAdj; // 
      private int[,] prev; // 

      public int MAXLENGTHPATH;


      public struct nodeInfos 
      {
         public static int idSetter=0;
         public List<coords> neighboors;
         public int id;
         public TileType type;
         public int distanceFromEnd;
         public coords nearestEnd;
         public nodeInfos(TileType type)
         {
            this.id = idSetter;
            idSetter++;
            this.type = type;
            this.neighboors = new();
            this.distanceFromEnd = int.MaxValue;
            this.nearestEnd = new coords(-1,-1);
         }
      }


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

   /// <summary>
   /// Dictionary that contains all nodes (coords) and their infos (nodeInfos)
   /// </summary>
   /// <returns></returns>
      public Dictionary<coords, nodeInfos> GetDictNodes()
      {
         return this.dictNodes;
      }

   /// <summary>
   /// List of nodes (represented by coords)
   /// </summary>
   /// <returns></returns>
      public List<coords> GetNodes()
      {
         return this.dictNodes.Keys.ToList();
      }

   /// <summary>
   /// nodeInfos is a STRUCT that contains ??? (TBA)
   /// </summary>
   /// <param name="node"></param>
   /// <returns></returns>
      public nodeInfos GetNodeInfos(coords node)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(node));
         return this.dictNodes[node];
      }

   /// <summary>
   /// List of weighted edges, weight being the length of the road between two nodes
   /// </summary>
   /// <returns></returns>
      public List<edge> GetEdges()
      {
         return this.edges;
      }

   /// <remarks> MUST BE FOLLOWED BY UpdateGraph() at some point, else it won't work</remarks>
      public void AddWeight(edge edgy, int value)
      {
         edgy.weight += value;
         this.edges[this.edges.FindIndex(x => x.node1.Equals(edgy.node1) && x.node1.Equals(edgy.node1))] = edgy;
         return;
      }

   /// <remarks>RECALCULATES THE ENTIRE GRAPH, so it is costly</remarks>
      public void UpdateGraph()
      {
         this.CreateMatAdj();
         this.CreateDistanceFromEnd();
      }


      public int GetPathWeight(coords pos1, coords pos2)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(pos1));
         Assert.IsTrue(this.dictNodes.ContainsKey(pos2));
         return this.matAdj[this.dictNodes[pos1].id, this.dictNodes[pos2].id];
      }

/*
procedure Path(u, v) is
    if prev[u][v] = null then
        return []
    path = [v]
    while u ≠ v do
        v = prev[u][v]
        path.prepend(v)
    return path
*/

      public List<coords> GetPath(coords pos1, coords pos2)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(pos1));
         Assert.IsTrue(this.dictNodes.ContainsKey(pos2));

         Dictionary<int, coords> dictIdToNode = new();
         foreach (coords node in this.dictNodes.Keys)
         {
            dictIdToNode[this.dictNodes[node].id] = node;
         }
         int u = this.dictNodes[pos1].id;
         int v = this.dictNodes[pos2].id;
         List<coords> path = new();
         if (this.prev[u,v] == -1){return path;}
         path.Add(dictIdToNode[v]);
         while (u != v)
         {
            v = this.prev[u,v];
            path.Add(dictIdToNode[v]);
         }
         path.Reverse();
         return path;
      }

      public int GetDistanceFromEnd(coords node)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(node));
         return this.dictNodes[node].distanceFromEnd;
      }

      public coords GetNearestEnd(coords node)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(node));
         return this.dictNodes[node].nearestEnd;
      }

      public List<coords> GetAllStart()
      {
         List<coords> starts = new();
         foreach (coords node in this.dictNodes.Keys)
         {
            if (this.dictNodes[node].type == TileType.start)
            {
               starts.Add(node);
            }
         }
         return starts; 
      }

      public void CreateNeighboors()
      {
         foreach (coords node in this.GetNodes())
         {
            nodeInfos value = this.dictNodes[node];            
            value.neighboors = new();
            foreach (edge edgy in this.edges)
            {
               if (edgy.node1.Equals(node))
               {
                  value.neighboors.Add(edgy.node2);
               }
               if (edgy.node2.Equals(node))
               {
                  value.neighboors.Add(edgy.node1);
               }
            }
         }
         return;
      }

      public List<coords> GetNeighboors(coords node)
      {
         return this.dictNodes[node].neighboors; 
      }

      public bool IsEdge(coords node1, coords node2)
      {
         Assert.IsTrue(this.dictNodes.ContainsKey(node1));
         Assert.IsTrue(this.dictNodes.ContainsKey(node2));
         foreach (edge edgy in this.edges)
         {
            if ((edgy.node1.Equals(node1) && edgy.node2.Equals(node2)) || (edgy.node2.Equals(node1) && edgy.node1.Equals(node2)))
            {
               return true;
            }
         }
         return false;
      }

      public int CompareDistanceFromEnd(nodeInfos n1, nodeInfos n2)
      {
            if (n1.distanceFromEnd < n2.distanceFromEnd)
            {
               return -1;
            }
            else if (n1.distanceFromEnd == n2.distanceFromEnd)
            {
               return 0;
            }
            else
            {
               return 1;
            }
      }

   /// <summary>
   /// This function creates the given graph of a mapArray 
   /// </summary>
   /// <param name="mapArray"></param>
      public void CreateGraph(TileType[,] mapArray)
      {
         this.MAXLENGTHPATH = mapArray.GetLength(0)*mapArray.GetLength(1);
         this.CreateDictNodes(mapArray);
         this.CreateEdges(mapArray);
         this.CreateMatAdj();
         this.CreateDistanceFromEnd();
         return;
      }

      private void CreateDictNodes(TileType[,] mapArray)
      {
         this.dictNodes = new();
         Graph.nodeInfos.idSetter = 0;
         for (int row = 0; row < mapArray.GetLength(0); row++)
         {
            for (int column = 0; column < mapArray.GetLength(1); column++)
            {
               coords pos = new coords(row, column);
               if (mapArray[row, column] == TileType.start)
               {
                  this.dictNodes[pos] = new nodeInfos(TileType.start);
               }
               else if (mapArray[row, column] == TileType.end)
               {
                  this.dictNodes[pos] = new nodeInfos(TileType.end);
               }
               else if (mapArray[row, column] == TileType.cross)
               {
                  this.dictNodes[pos] = new nodeInfos(TileType.cross);
               }
            }
         }

         return;
      }


      private void CreateEdges(TileType[,] mapArray)
      { 
         this.edges = new();
         Debug.Assert(this.dictNodes != null);
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
                  this.edges.Add(new edge(lastCrossPosition, pos, weight));                  
//                  this.edges.Add(new edge(pos, lastCrossPosition, weight));
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
                  edges.Add(new edge(lastCrossPosition, pos, weight));
//                  edges.Add(new edge(pos, lastCrossPosition, weight));
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
      

         return;
      }


/*  
procedure FloydWarshallWithPathReconstruction() is
    for each edge (u, v) do
        dist[u][v] = w(u, v)  // The weight of the edge (u, v)
        prev[u][v] = u
    for each vertex v do
        dist[v][v] = 0
        prev[v][v] = v
    for k from 1 to |V| do // standard Floyd-Warshall implementation
        for i from 1 to |V|
            for j from 1 to |V|
                if dist[i][j] > dist[i][k] + dist[k][j] then
                    dist[i][j] = dist[i][k] + dist[k][j]
                    prev[i][j] = prev[k][j]
*/
      private void CreateMatAdj()
      {
         int length = this.GetNodes().Count;
         this.matAdj = new int[length, length]; 
         this.prev = new int[length, length];        
         for (int row = 0; row < length; row++)
         {
            for (int column = 0; column < length; column++)
            {
               this.matAdj[row, column] = this.MAXLENGTHPATH + 1;
               this.prev[row, column] = -1;
               if (row==column)
               {
                  this.matAdj[row,column] = 0;
                  this.prev[row, column] = row;
               }
            }
         }
         foreach (edge edgy in this.edges)
         {
            this.matAdj[this.dictNodes[edgy.node1].id, this.dictNodes[edgy.node2].id] = edgy.weight;
            this.matAdj[this.dictNodes[edgy.node2].id, this.dictNodes[edgy.node1].id] = edgy.weight;
            this.prev[this.dictNodes[edgy.node1].id, this.dictNodes[edgy.node2].id] = this.dictNodes[edgy.node1].id;
            this.prev[this.dictNodes[edgy.node2].id, this.dictNodes[edgy.node1].id] = this.dictNodes[edgy.node2].id;
         }

         for (int k=0; k < length; k++)
         {
            for (int i=0; i < length; i++)
            {
               for (int j=0; j < length; j++)
               {
                if (this.matAdj[i,j] > this.matAdj[i,k] + this.matAdj[k, j])
                  {
                     this.matAdj[i,j] = this.matAdj[i,k] + this.matAdj[k,j];
                     this.prev[i,j] = this.prev[k,j];                     
                  }
               }
            }   
         }
      }

      private void CreateDistanceFromEnd()
      {
         List<coords> endNodes = new();
         var keys = GetNodes();
         foreach (coords node in keys)
         {
            nodeInfos value = dictNodes[node];
            value.distanceFromEnd = this.MAXLENGTHPATH;
            dictNodes[node] = value;
            if (dictNodes[node].type == TileType.end)
            {
               endNodes.Add(node);
            }
         }
         foreach (coords node in keys)
         {
            foreach (coords nodeEnd in endNodes)
            {
               nodeInfos value = dictNodes[node];
               int w = GetPathWeight(node, nodeEnd);
               if (w < dictNodes[node].distanceFromEnd)
               {
                  value.distanceFromEnd = w;
                  value.nearestEnd = nodeEnd;
                  dictNodes[node] = value;                  
               }
            }
         }
         return;
      }


   }
}
