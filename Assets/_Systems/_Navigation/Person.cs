using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public abstract partial class Person
{

    [JsonProperty]
    public readonly List<IPromptProvider> promptProviders = new();

    public interface IPromptProvider
    {
        string GetPrompt();
    }

    [JsonProperty]
    public readonly string name;

    [JsonProperty]
    public Vector3 worldPosition { get; set; }
    [JsonProperty]
    public Slot destination { get; set; }

    [JsonProperty]
    public List<Slot> PathPoints { get; set; } = new();
    public event Action OnDataUpdate;
    public Person(string name, Vector3 worldPosition)
    {
        this.name = name;
        this.worldPosition = worldPosition;
    }

    public void AddPathPoint(Slot slot)
    {
        PathPoints.Add(slot);
        OnDataUpdate?.Invoke();
    }

    protected abstract void OnCreated();
    protected abstract void OnEnable();

    public string GetInitialPrompt()
    {
        StringBuilder builder = new();
        builder.AppendLine(CommonInitialPromotGlobal);
        if (this is IPromptProvider selfProvider)
        {
            builder.AppendLine(selfProvider.GetPrompt());
        }
        foreach (var promptProvider in promptProviders)
        {
            builder.AppendLine(promptProvider.GetPrompt());
        }
        return builder.ToString();
    }

    public const string CommonInitialPromotGlobal = "接下来，你将扮演乡村建设游戏中的一名NPC，用来指引玩家，帮助玩家解决问题。注意：保持真实度，不要出戏。回答尽可能简短、精简，口语化，零散化，像人类一样，一下不要说太多，最多50个字左右。我（即玩家）则是参与村庄建设指导的下乡大学生。以下是其他系统的一些信息，请充分利用：";
}
