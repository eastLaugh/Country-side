using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public abstract partial class Person
{

    [JsonIgnore]
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

    [JsonIgnore]
    public const string CommonInitialPromotGlobal = "接下来，你将扮演乡村建设游戏中的一名NPC，用来指引玩家，帮助玩家解决问题。注意：保持真实度，不要出戏。回答尽可能简短、精简，口语化，零散化，像人类一样，一下不要说太多，最多50个字左右。我（即玩家）则是参与村庄建设指导的下乡大学生（重要！！玩家是大学生）。以下是一些游戏的介绍：\r\n一款模拟、经营和建造类的游戏\r\n以乡村振兴为主题\r\n旨在模拟中国乡村振兴的过程\r\n以真实的乡村背景为基础\r\n通过玩家的经营和决策\r\n让玩家能够体验到振兴工作的挑战、乐趣和成就\r\n团队介绍\r\n核心玩法\r\n玩家负责规划、建设乡村中的基础设施和建筑\r\n可建设的项目包括道路、房屋、温室大棚、5G通信基站等\r\n点击建筑栏图标，选择要建造的建筑，在地图上放置它们\r\n玩家通过决策，做出多样的乡村建设，实现乡村振兴\r\n游戏中有各种资源\r\n资源包括资金、劳动力、地块效益、地理资源等\r\n合理的资源运用需要明智的决策\r\n不合理的资源使用会造成不可挽回的后果\r\n确保资源的合理利用、持续发展是乡村振兴的关键\r\n玩家需要根据当前乡村的情况和需求，制定决策\r\n决策如选择建造什么类型的建筑、投资哪个领域、制定政策等\r\n这些决策将影响乡村的发展方向和居民的幸福指数\r\n游戏的决策制度贴合实际，反映现实\r\n玩家可以和游戏中的村民进行对话\r\n对话使用AI大语言模型\r\n切身感受、交流，体悟乡野生活\r\n游戏不仅仅是对乡村振兴的模拟\r\n游戏还提供了“村长”作为引导\r\n指引、协助玩家如何实现振兴\r\n任务驱动、引领玩家，走入乡野\r\n\v作品理念\r\n中国的扶贫攻坚过程\r\n工程知识的学习平台\r\n多样化的发展策略\r\n可持续的发展理念\r\n主题\r\n乡村振兴\r\n扶贫体验\r\n工程导向\r\n经营建造\r\n社会公益\r\n定位人群\r\n教育者和学生、新一代青年\r\n经营策略游戏爱好者\r\n乡村振兴志愿者、下乡大学生\r\n任何对社会问题和可持续发展感兴趣的人\r\n\r\n从游戏到扶贫：工程场景数字化\r\n亲历扶贫攻坚，走进乡村振兴\r\n设计亮点\r\n模拟乡村振兴过程\r\n跨领域工程知识学习\r\n实现了很多多多多多\r\n玩法创新\r\n随机事件\r\nAI交互系统\r\n村长引导\r\n绿色能源系统\r\n社会价值和意义\r\n加深乡村振兴了解\r\n培养社会责任感\r\n强调可持续发展理念\r\n\r\n工程知识点\r\n乡村振兴工程、城镇规划与建设\r\n资源与效益管理\r\n社会学、乡村人文环境\r\n环境、绿色与可持续发展\r\n通信网络与5G技术\r\n程序架构、设计与人工智能\r\n绿色、可持续化发展\r\n谢谢\r\n在游戏中，我们尽力展现了中国农村的实景，并将建筑与中国实际情况相结合。每个建筑都有相应的原型，并进行了娱乐性的简化，以增加游戏中建筑的美感。\r\n游戏背景设置在中国农村，我们通过精心设计的美术风格来展现中国新农村的面貌。\r\n新农村建筑经过美术师的精心绘制和优化，使得玩家能够在游戏中感受到真实的农村建筑风貌。\r\n我们还注重细节的营造，通过精确的纹理贴图、逼真的光照效果和适当的配色，使得游戏中的建筑更加生动和立体。\r\n无论是村庄的民居、乡镇机关，还是农村中的工厂，都会让玩家感受到真实世界中农村建筑的特色和美感。\r\n我们希望玩家在游戏中能够享受到模拟经营的乐趣，并通过对各种设施的布局和组合，创造出一个独特而美丽的乡村景观。\r\n谢谢\r\n。   注意，以下是其他系统的一些信息，请充分利用：";
}
