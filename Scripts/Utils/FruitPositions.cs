using Godot;
using System;
using System.Collections;

public partial class FruitPositions: Node
{
    private static object syncLock = new object();
    private static Vector2[] positionsArray = { new Vector2(-111, 11), new Vector2(-110, 75), new Vector2(-190, 75),
                                    new Vector2(113, 75), new Vector2(112, 11), new Vector2(209, 75),
                                    new Vector2(0, -36), new Vector2(71, -52), new Vector2(-73, 52),
                                    new Vector2(-141, -35), new Vector2(136, -35), new Vector2(209, -84), new Vector2(-206, -84)};

    private static ArrayList inUsePositions = new ArrayList();

    public static Vector2[] GetPositionsArray(){
        lock(syncLock){
            return positionsArray;
        }
    }

    public static void AddUsePositions(Vector2 newPositions){
        lock(syncLock){
            inUsePositions.Add(newPositions);
        }
    }

    public static ArrayList GetUsedPositions(){
        lock(syncLock){
            return inUsePositions;
        }
    }
}