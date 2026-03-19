using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    [SerializeField] GameObject node;

    // Update is called once per frame
    void Update()
    {
        
    }

    void fillStartNode()
    {
        
    }

    void createNextNodes()
    {
        Instantiate(node, transform.position, Quaternion.identity);
    }
}
