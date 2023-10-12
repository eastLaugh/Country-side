INCLUDE Global.ink

-> Welcome
== Welcome
~SetName("村长")
欢迎来到我们的村庄！（鼠标点击继续）

我们的乡村打赢了脱贫攻坚战，然而，时代在改变，新的机遇也随之而来。
当前，中国政府高度重视乡村振兴战略，希望我们村子能实现产业兴旺、生态宜居、乡风文明、治理有效、生活富裕的战略要求。

听说你是国家派来的振兴干部，愿意帮助我们振兴乡村，是吗？
~ SetName("提示")
嘀嘀，我是您的人工智能助手小文
接到您的上级指示，您的使命是<color=red>完成所有村长的任务，以及使村民们的幸福度达到100</color>
但如果<color=red>村民们的幸福度降至0或者时间超过2030年1月1日</color>，乡村振兴就失败了哦
~SetName("村长")
听说你是国家派来的振兴干部，愿意帮助我们振兴乡村，是吗？
*[非常荣幸]
-> Chapter1

== Chapter1
太好了！首先，我们需要建造一些基础建筑来满足村民的生活生产需求。

*[好的]
-> CementRoad

== CementRoad
我们的村子目前只有少数几条水泥路，给村民的出行和农业生产带来很大的不便。因此，我们需要更多的水泥路来改善交通状况。
~ SetName("小文")
点击建筑栏图标，就可以进入建造模式啦，记得建造好了要再次点击原图标退出建造模式哦
在非建造模式下，点击建筑物所属地块，就可以看到建筑物属性了呦
~ SetName("村长")
建造十条水泥路，让村庄的交通更加便捷吧，记得完成后来<color=red>村委会</color>找我哦。
~ SetName("小文")
关闭对话界面后，再点击地图上的村委会，就能和村长对话啦
~ SetName("村长")
[任务]建造十条水泥路，让村庄的交通更加便捷吧，记得完成后来<color=red>村委会</color>找我哦。
~ Check("建10条水泥路")
*[我完成了]
太感谢你了。新建了这些水泥路，村民们可以轻松地去到不同地方，农产品也能更快速地运输出村，村民们会非常高兴的，也能为乡村经济发展注入新的动力。
** [...]
-> CementHouse


== CementHouse
~ SetName("村长")
我们村子现在每家每户都住上水泥房了，真的多亏了国家的脱贫政策啊！
看到屏幕上方的人口了吗？右边的第一个数值就是现在村子的人口，第二个数值就是现在村子的可容载人口。
不过我们还是需要建造更多的水泥房来作为备用住宅。先建造三个水泥房吧！
~ SetName("小文")
进入建造模式后，按Q或者E键就可以改变建筑物朝向了哦
另外，水泥房必须要和村委会用道路连通才能容纳人口哦
~ SetName("村长")
[任务]先建造三个水泥房吧！
~ Check("建造3个水泥房")
*[我建完了]
太感谢你了！之后你可以继续建造更多的水泥房作为备用。

** [...]
-> photovoltaic

== photovoltaic
我们这里的太阳能资源十分丰富呢，或许可以尝试利用这个清洁能源。

我们要知道光伏发电是一种非常环保和可持续的能源形式。通过利用太阳能，我们可以将阳光转化为电能，为我们的村庄提供清洁、稳定的电力供应。这对于改善我们的乡村环境质量，减少污染和碳排放有着重要的意义。

看到屏幕右上角的绿色图标了吗，它显示的是我们村的绿色能源占比，想必当我们村都使用绿色能源，村民应该会很开心吧。

先建造3个光伏装置看看效果吧，记得<color=red>光伏装置要建造在水泥房上哦！</color>
~ Check("建造3个光伏（住宅）")
*[完成了]
太好了！村民也反馈在供电不足的情况下也可以有电用了呢！

** [不用谢]
~SetName("提示")
~ShowAssignment()
请查看“任务”面板，村长已经为你布置了一些任务，赶快去完善它们吧！
*** [交给我吧]
~SetName("村长")
有问题就再来村委会找我吧。有了你，我们村子一定会变得越来越好！
-> END