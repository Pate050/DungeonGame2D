using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomSettigns : ScriptableObject
{
    public List<Prop> propsToPlace;

    [Min(0)]
    public int CrateQuantityMin = 1;
    [Min(0)]
    public int CrateQuantityMax = 1;

    [Min(0)]
    public int TreasureQuantityMin = 1;
    [Min(0)]
    public int TreasureQuantityMax = 1;

    [Min(0)]
    public int ObstacleQuantityMin = 1;
    [Min(0)]
    public int ObstacleQuantityMax = 1;
}
