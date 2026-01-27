using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TowerNotBuilt : Tower
{
    [SerializeField]
    private GameObject ghostTile;

    private List<GameObject> ghostTiles = new List<GameObject>();

    private Transform transform;

    public void positionGhostTile()
    {
        transform = GetComponent<Transform>();

        ghostTiles.ForEach(gtile => Destroy(gtile));

        coords = Map.Instance.PositionToCoords(transform.position);

        foreach (Map.coords tile in getNeighbours())
        {
            Vector3 positionGT = Map.Instance.CoordsToPosition(tile);
            positionGT.y += 0;
            ghostTiles.Add(Instantiate(ghostTile, positionGT, transform.rotation));
        }
    }

    public void OnDestroy()
    {
        ghostTiles.ForEach(gtile => Destroy(gtile));
    }
}
