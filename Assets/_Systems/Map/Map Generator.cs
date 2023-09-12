using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator
{
    public abstract void Generate(Map map, Vector2Int size, Slot[] slots);
}

[AttributeUsage(AttributeTargets.Class)]
public class RegisterAsMapLayer : Attribute
{
    public int Order { get; set; } = -1;
}

[RegisterAsMapLayer]
public class WaterAndPlain : MapGenerator
{
    public override void Generate(Map map, Vector2Int size, Slot[] slots)
    {

        float PerlinOffsetX = UnityEngine.Random.Range(0f, 1f);
        float PerlinOffsetY = UnityEngine.Random.Range(0f, 1f);

        //遍历格子
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                float noise = Mathf.PerlinNoise(PerlinOffsetX + i * 0.1f, PerlinOffsetY + j * 0.1f);
                Slot newSlot;

                if (noise > 0.2f)
                {
                    newSlot = new Slots.Plain(map, new Vector2(i, j), new());
                    if (noise > 0.7f)
                    {
                        new MapObjects.Tree().Inject(newSlot);
                    }
                }
                else
                {
                    newSlot = new Slots.Water(map, new Vector2(i, j), new());
                    new MapObjects.Lake().Inject(newSlot);
                }

                slots[i * size.y + j] = newSlot;
            }
        }
    }
}