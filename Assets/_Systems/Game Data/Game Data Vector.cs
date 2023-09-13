using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class GameDataVector : IDataVector<GameDataVector>
{

    public GameDataVector(int People = 0, float Money = 0f, float dailyIncome = 0f, float Happiness = 0f)
    {
        this.People = People;
        this.Money = Money;
        this.dailyIncome = dailyIncome;
        this.Happiness = Happiness;
    }
    public int People { get;  set; }
    public float dailyIncome { get;  set; }
    public float Money { get; set; }
    public float Happiness { get; set; }

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

    public static GameDataVector operator *(GameDataVector left, GameDataVector right)
    {
        return new GameDataVector(
            left.People * right.People,
            left.Money * right.Money,
            left.dailyIncome * right.dailyIncome,
            left.Happiness * right.Happiness
        );
    }
    public GameDataVector Add(GameDataVector other)
    {
        return this + other;
    }

    public  GameDataVector Minus(GameDataVector other)
    {
        return this - other;
    }

    public  GameDataVector Multiply(GameDataVector other)
    {
        return this * other;
    }
}

public struct Float : IDataVector<Float>
{
    public float m_value;
    public Float(float value)
    {
        m_value = value;
    }
    public Float Add(Float other)
    {
        return new Float(m_value + other.m_value);
    }

    public Float Minus(Float other)
    {
        return new Float(m_value - other.m_value);
    }

    public Float Multiply(Float other)
    {
        return new Float(m_value * other.m_value);
    }
}


public struct Int: IDataVector<Int>
{
    public int m_value;
    public Int(int value)
    {
        m_value = value;
    }
    public Int Add(Int other)
    {
        return new Int(m_value + other.m_value);
    }

    public Int Minus(Int other)
    {
        return new Int(m_value - other.m_value);
    }

    public Int Multiply(Int other)
    {
        return new Int(m_value * other.m_value);
    }
}
