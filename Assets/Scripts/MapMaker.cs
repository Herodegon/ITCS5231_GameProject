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
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows[i]; j++)
            {
                createNextNode(nodeCount);
                nodeCount++;
            }
        }
    }

    public void fillStartNode()
    {
        
    }

    public void createNextNode(int nodeNum)
    {
        GameObject nextNode = Instantiate(node, map.transform);
        nextNode.transform.localPosition = new Vector3(-660, 0, 0);
    }
}
