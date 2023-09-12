// source: https://www.youtube.com/watch?v=3g440SA2hKU

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    public Tile[] upNeighbours;
    
    [SerializeField]
    public Tile[] rightNeighbours;
    
    [SerializeField]
    public Tile[] downNeighbours;
    
    [SerializeField]
    public Tile[] leftNeighbours;
}