using UnityEngine;

public class BSPNode
{
    public RectInt area;         
    public BSPNode left;
    public BSPNode right;
    public RectInt room;         
    public bool hasRoom;

    public BSPNode(RectInt area)
    {
        this.area = area;
    }

    public bool IsLeaf => left == null && right == null;
    public bool Split(int minSize)
    {
        if (!IsLeaf) return false;
        bool splitHorizontally = Random.value > 0.5f;
        if (area.width > area.height * 1.25f) splitHorizontally = false;
        else if (area.height > area.width * 1.25f) splitHorizontally = true;

        int max = (splitHorizontally ? area.height : area.width) - minSize;
        if (max < minSize) return false; 

        int splitPos = Random.Range(minSize, max + 1);

        if (splitHorizontally)
        {
            left = new BSPNode(new RectInt(area.x, area.y, area.width, splitPos));
            right = new BSPNode(new RectInt(area.x, area.y + splitPos, area.width, area.height - splitPos));
        }
        else
        {
            left = new BSPNode(new RectInt(area.x, area.y, splitPos, area.height));
            right = new BSPNode(new RectInt(area.x + splitPos, area.y, area.width - splitPos, area.height));
        }
        return true;
    }
}