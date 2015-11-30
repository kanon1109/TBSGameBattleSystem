using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 规则：
/// 1.根据数量获取出战人数，队伍中每人轮流出战。（例如：第一轮 1号位出战，第二轮2号位 …… 依次类推）
/// 2.攻击者优先攻击最靠近自己的敌人
/// </summary>
public class BattleController
{
    //攻击类型
    enum attackType
    {
        CLOSE = 1,
        REMOTE = 2
    }
    //我方队伍
    private List<BattleRole> myTeam = new List<BattleRole>();
    //对方队伍
    private List<BattleRole> targetTeam = new List<BattleRole>();
    //攻击方临时列表
    private List<BattleRole> attackList = new List<BattleRole>(); 
    //我方队伍的起始索引
    private int myTeamIndex = 0;
    //敌方队伍的起始索引
    private int targetTeamIndex = 0;
    //是否是我方队伍出手
    private bool isMyTeam;
    //人物模型的父级
    private Transform roleParent;
    //当前回合数
    private int curRound = 0;
    /// <summary>
    /// 初始化
    /// </summary>
    public void init(Transform parent)
    {
        KeyboardManager.registerKey(UnityEngine.KeyCode.A, onKeyAttackHandler, false);
        KeyboardManager.registerKey(UnityEngine.KeyCode.H, onKeyHurtHandler, false);
        this.roleParent = parent;
        this.initTestData();
        this.resetData();
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    private void resetData()
    {
        this.myTeamIndex = 0;
        this.targetTeamIndex = 0;
        this.isMyTeam = true;
    }

    private void onKeyHurtHandler()
    {
        //用于测试防御动作
    }

    private void onKeyAttackHandler()
    {
        //用于测试攻击动作
        this.getAttacker(true, 5);
        this.getAttackTarget(true);
        this.startRound();
    }

    private void initTestData()
    {
        //此处为测试数据
        Vector3 pos = new Vector3(-5, 0, 45);
        for (int i = 0; i < 6; ++i)
        {
            HeroVo hVo = new HeroVo();
            hVo.atk = 10;
            hVo.def = 5;
            hVo.hp = 100;

            BattleRole br = new BattleRole();
            br.index = i;
            br.create(this.roleParent, pos);
            br.heroVo = hVo;
            myTeam.Add(br);
            pos.z -= 5;
            if (i == 2)
            {
                pos.x -= 5;
                pos.z = 45;
            }
        }

        pos = new Vector3(10, 0, 45);
        for (int i = 0; i < 6; ++i)
        {
            HeroVo hVo = new HeroVo();
            hVo.atk = 10;
            hVo.def = 5;
            hVo.hp = 5;

            BattleRole br = new BattleRole();
            br.index = i;
            br.create(this.roleParent, pos);
            br.heroVo = hVo;
            targetTeam.Add(br);
            pos.z -= 5;
            if (i == 2)
            {
                pos.x += 5;
                pos.z = 45;
            }
        }
    }

    /// <summary>
    /// 讲本轮出战的攻击者存入列表中
    /// </summary>
    /// <param name="isMyTeam">是否从我方队伍里获取</param>
    /// <param name="num">获取数量</param>
    private void getAttacker(bool isMyTeam, int num)
    {
        this.attackList.Clear();
        List<BattleRole> team = null;
        int teamIndex;
        if (isMyTeam)
        {
            teamIndex = this.myTeamIndex;
            team = this.myTeam;
        }
        else
        {
            teamIndex = this.targetTeamIndex;
            team = this.targetTeam;
        }
        //判断全部死亡
        if(this.allDead(isMyTeam)) return;
        //如果索引超过了最后一位则到第一位
        if (teamIndex > team.Count - 1) teamIndex = 0;
        //人数不超过列表长度
        if (num > team.Count) num = team.Count;
        for (int i = 0; i < num; ++i)
        {
            MonoBehaviour.print(teamIndex + "号位置");
            BattleRole br = team[teamIndex];
            this.attackList.Add(br);
            teamIndex++;
            if (teamIndex > team.Count - 1) teamIndex = 0;
        }
        //保存当前的队伍中选择的人物索引
        if (isMyTeam)
            this.myTeamIndex = teamIndex;
        else
            this.targetTeamIndex = teamIndex;
        MonoBehaviour.print("this.myTeamIndex " + this.myTeamIndex);
    }

    /// <summary>
    /// 获取被攻击目标（普通攻击）
    /// </summary>
    /// <param name="isMyTeam">是否是我方队伍攻击</param>
    private void getAttackTarget(bool isMyTeam)
    {
        List<BattleRole> targetList;
        if (isMyTeam) targetList = this.targetTeam;
        else targetList = this.myTeam;
        int count = this.attackList.Count;
        int targetCount = targetList.Count;
        //关闭列表 存放一次进攻后血量为0的角色
        List<BattleRole> closeList = new List<BattleRole>();
        for (int i = 0; i < count; ++i)
        {
            BattleRole attackRole = this.attackList[i];
            float dis = float.MaxValue;
            int index = 0;
            //查找距离最近的对手
            for (int j = 0; j < targetCount; ++j)
            {
                BattleRole targetBr = targetList[j];
                if (closeList.IndexOf(targetBr) != -1) continue;
                float curDis = (attackRole.startPos - targetBr.startPos).sqrMagnitude;
                if (curDis < dis)
                {
                    index = j;
                    dis = curDis;
                }
            }

            //计算被攻击者剩余血量 如果进攻后血量为0 则放入关闭列表中
            BattleRole chooseBr = targetList[index];
            if (DamageUtils.checkRoleDead(attackRole.heroVo, chooseBr.heroVo))
                closeList.Add(chooseBr);
            //将目标存放进去
            attackRole.setAttackTarget(chooseBr);
        }
    }

    /// <summary>
    /// 开始一轮攻击
    /// </summary>
    private void startRound()
    {
        this.curRound++;
        //选出攻击者
        this.selectedAttack(null);
    }

    private void selectedAttack(object param)
    {
        MonoBehaviour.print("selectedAttack");
        if (this.attackList.Count == 0) return;
        BattleRole br = this.attackList[0];
        this.attackList.RemoveAt(0);
        MonoBehaviour.print("index " + br.index);
        bool autoDestroy = false;
        if (this.attackList.Count == 1) autoDestroy = true;
        Delay.setDelay(this.roleParent.gameObject, 500, selectedAttack, autoDestroy);
    }

    /// <summary>
    /// 是否全部死亡
    /// </summary>
    /// <param name="isMyTeam">是否是我方队伍</param>
    /// <returns></returns>
    private bool allDead(bool isMyTeam)
    {
        List<BattleRole> team = null;
        if (isMyTeam)
            team = this.myTeam;
        else
            team = this.targetTeam;
        return team.Count == 0;
    }
}