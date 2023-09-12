using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public struct GameDataVector : IDataVector<GameDataVector>
{

    public GameDataVector(int People = 0, float Money = 0f, float dailyIncome = 0f,float Happiness = 0f)
    {
        this.People = People;
        this.Money = Money;
        this.dailyIncome = dailyIncome;
        this.Happiness = Happiness;
    }
    public int People { get; private set; }
    public float dailyIncome {get; private set;}
    public float Money { get; private set; }
    public float Happiness { get;private set; }

    public static GameDataVector operator +(GameDataVector left, GameDataVector right)
    {
        return new GameDataVector(
            left.People + right.People,
            left.Money + right.Money,
            left.dailyIncome + right.dailyIncome,
            left.Happiness + right.Happiness
        );
    }

    public static GameDataVector operator -(GameDataVector left, GameDataVector right)
    {
        return new GameDataVector(
            left.People - right.People,
            left.Money - right.Money,
            left.dailyIncome - right.dailyIncome,
            left.Happiness - right.Happiness
        );
    }
    public readonly GameDataVector Add(GameDataVector other)
    {
        return this + other;
    }

    public readonly GameDataVector Minus(GameDataVector other)
    {
        return this - other;    
    }
}
