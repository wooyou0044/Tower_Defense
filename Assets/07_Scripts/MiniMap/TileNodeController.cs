using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodeController : MonoBehaviour
{
    [SerializeField] TileType type;
    List<TileNode> haveNodes = new List<TileNode>();

    public TileType Type
    {
        get => type;
    }

    public List<TileNode> HaveNodes
    {
        get => haveNodes;
    }

    public void AddNodes(TileNode node)
    {
        haveNodes.Add(node);
    }

    public void ClearNodes()
    {
        haveNodes.Clear();
    }

    public TileNode FindClosestTile(Vector3 mousePos)
    {
        if(haveNodes.Count < 0)
        {
            return null;
        }
        TileNode closestTile = null;
        float minDistance = float.MaxValue;
        Vector3 tileWorldPos;

        for(int i=0; i<haveNodes.Count; i++)
        {
            tileWorldPos = haveNodes[i].worldPosition;
            float distance = Vector2.Distance(new Vector2(mousePos.x, mousePos.z), new Vector2(tileWorldPos.x, tileWorldPos.z));
            if(distance < minDistance)
            {
                minDistance = distance;
                closestTile = haveNodes[i];
            }
        }

        return closestTile;
    }
}
