using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    [SerializeField] GameObject node;
    [SerializeField] GameObject map;

    public void makeMap(int cols, int[] rows)
    {
        int nodeCount = 1;
        int placeX = 1400/cols;
        int ySign = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows[i]; j++)
            {
                if(j == 1)
                {
                    ySign = 1;
                }if(j == 2)
                {
                    ySign = -1;
                }
                createNextNode(nodeCount, placeX * i, ySign);
                nodeCount++;
            }
        }
    }

    public void fillStartNode()
    {
        
    }

    public void createNextNode(int nodeNum, int x, int y)
    {
        GameObject nextNode = Instantiate(node, map.transform);
        nextNode.transform.localPosition = new Vector3(-660 + x, 100 * y, 0);
    }
}
