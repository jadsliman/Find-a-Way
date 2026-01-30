using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    public BaseBlock[] neighbors = new BaseBlock[4];
    public int neighborsNumber;
    public Block[] layers = new Block[3];
    public int layersNumber;
    public int id;
    public bool isVisited;

    private void Awake()
    {
        neighborsNumber = 0;
        layersNumber = 0;
        isVisited = false;
    }
}
