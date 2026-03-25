using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    [SerializeField] GameObject node;

    // Update is called once per frame
    void Awake()
    {
        fillStartNode();
    }

    public void fillStartNode()
    {
        
    }

    public void createNextNode(Vector3 pos, Quaternion rot)
    {
        Object.Instantiate(node, pos, rot);
    }
}
