// random map generator
// source: https://www.youtube.com/watch?v=6B7yOnqpK_Y
// uses 2 tiles
// uses the Random Walker Algorithm

using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange){
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
    }
}