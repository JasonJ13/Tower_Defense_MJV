using UnityEngine;

public class Generator : MonoBehaviour
{
    void OnEnable() { 
        foreach(tile in this.getNeighbours())
        {
            if (Map.Instance.GetMapArrayCoords(tile) == Map.TileType.construct)
            {
                
            }
        }
    }

    // Update is called once per frame
    void Update() { }
}
