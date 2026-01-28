using UnityEngine;

public class Generator : Tower
{
    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (Map.coords tile in this.getNeighbours())
        {
            if (Map.Instance.GetMapArrayCoords(tile) == Map.TileType.construct)
            {
                Player.Instance.supplieTower(tile);
            }
        }
    }

    // Update is called once per frame
    void Update() { }
}
