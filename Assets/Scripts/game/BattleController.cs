using support;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 规则：
/// 1.根据数量获取出战人数，队伍中每人轮流出战。（例如：第一轮 1号位出战，第二轮2号位 …… 依次类推）
/// 2.攻击者优先攻击最靠近自己的敌人
/// 
/// TODO 
/// [一轮内的状态]
/// [显示掉血和人物血量]
/// [增加阵位 通过阵位判断距离]
/// 远程攻击
/// </summary>
public class BattleController
{
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
    //当前回合数
    private int _curRound = 1;
    public int curRound
    {
        get { return _curRound; }
    }
    //我方队伍是否攻击过
    private bool myTeamAttacked = false;
    //敌方队伍是否攻击过
    private bool targetTeamAttacked = false;
    //是否开始攻击了
    private bool isStartAttack = false;
    /// <summary>
    /// 初始化
    /// </summary>
    public void init()
    {
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.ROLE_DEAD, roleDeadHandler);
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.ROLE_DEAD, roleDeadHandler);
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.ROLE_BACK, roleBackHandler);
        KeyboardManager.registerKey(UnityEngine.KeyCode.A, onKeyAttackHandler, false);
        this.initTestData();
        this.resetData();
    }

    //人物归位消息
    private void roleBackHandler(object param)
    {
        if(this.isAllBack(this.isMyTeam))
        {
            if (!this.allDead(!this.isMyTeam))
            {
                //如果全部归位 攻防切换
                this.changeAttacker();
            }
            else
            {
                if(!this.isMyTeam)
                    NotificationCenter.getInstance().postNotification(BattleMsgConstant.MY_TEAM_ALL_DEAD); //我方全部死亡
                else
                    NotificationCenter.getInstance().postNotification(BattleMsgConstant.TARGET_TEAM_ALL_DEAD); //敌方全部死亡 
            }
        }
    }

    //人物死亡消息
    private void roleDeadHandler(object param)
    {
        //角色死亡消息
        BattleRole br = (BattleRole)param;
        int index = br.index;
        //从目标列表中删除此目标
        List<BattleRole> list;
        if (this.isMyTeam) 
            list = this.targetTeam;
        else 
            list = this.myTeam;
        int length = list.Count;
        for (int i = 0; i < length; i++)
        {
            BattleRole battleRole = list[i];
            if (battleRole.index == index)
            {
                list.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    private void resetData()
    {
        this._curRound = 1;
        this.myTeamIndex = 0;
        this.targetTeamIndex = 0;
        this.isMyTeam = true;
        this.myTeamAttacked = false;
        this.targetTeamAttacked = false;
        this.isStartAttack = false;
    }

    private void onKeyAttackHandler()
    {
        //用于测试攻击动作
        if (!this.myTeamAttacked && 
            !this.targetTeamAttacked && 
            !this.isStartAttack)
        {
            this.getAttacker(this.isMyTeam, 3);
            this.getAttackTarget(this.isMyTeam);
            this.startRoleAttack(null);
        }
    }

    private void initTestData()
    {
        //此处为测试数据
        for (int i = 0; i < 6; ++i)
        {
            HeroVo hVo = new HeroVo();
            hVo.atk = RandomUtil.randint(15, 35);
            hVo.atk = 20;
            hVo.def = RandomUtil.randint(2, 5);
            hVo.hp = RandomUtil.randint(20, 50);

            hVo.id = i + 1;

            BattleRole br = new BattleRole();
            br.index = i;
            br.create(Layer.Instance.battleScene.transform);
            br.createHpBar(Layer.Instance.battleUILayer.transform);
            br.initHpBarMax(hVo.hp);
            br.isMyTeam = true;
            br.heroVo = hVo;
            myTeam.Add(br);
        }

        for (int i = 0; i < 6; ++i)
        {
            HeroVo hVo = new HeroVo();
            hVo.atk = RandomUtil.randint(15, 35);
            hVo.def = RandomUtil.randint(2, 5);
            hVo.hp = RandomUtil.randint(20, 50);
            hVo.id = i + 6;

            BattleRole br = new BattleRole();
            br.index = i;
            br.create(Layer.Instance.battleScene.transform);
            br.createHpBar(Layer.Instance.battleUILayer.transform);
            br.initHpBarMax(hVo.hp);

            br.isMyTeam = false;
            br.heroVo = hVo;
            targetTeam.Add(br);
        }
        this.initTeamPos();
    }

    /// <summary>
    /// 初始化队伍位置
    /// </summary>
    private void initTeamPos()
    {
        BattleFormation.init();
        BattleRole br;
        int count = this.myTeam.Count;
        int[] matrixAry;
        Vector3 startPos = BattleFormation.leftStartPos;
        int gapH = BattleFormation.gapH;
        int gapV = BattleFormation.gapV;
        for (int i = 0; i < count; ++i)
        {
            br = this.myTeam[i];
            matrixAry = BattleFormation.leftMatrix[i];
            int xm = matrixAry[0];
            int zm = matrixAry[1];
            Vector3 pos = new Vector3(startPos.x + (xm - 1) * gapH, 
                                      startPos.y, 
                                      startPos.z - (zm - 1) * gapV);
            br.setPosition(pos);
            int posX = BattleFormation.leftPosIndex[i][0];
            int posY = BattleFormation.leftPosIndex[i][1];
            br.posIndexVector = new Vector2(posX, posY);
        }

        startPos = BattleFormation.rightStartPos;
        count = this.targetTeam.Count;
        for (int i = 0; i < count; ++i)
        {
            br = this.targetTeam[i];
            matrixAry = BattleFormation.rightMatrix[i];
            int xm = matrixAry[0];
            int zm = matrixAry[1];
            Vector3 pos = new Vector3(startPos.x + (xm - 1) * gapH,
                                      startPos.y,
                                      startPos.z - (zm - 1) * gapV);
            br.setPosition(pos);
            int posX = BattleFormation.rightPosIndex[i][0];
            int posY = BattleFormation.rightPosIndex[i][1];
            br.posIndexVector = new Vector2(posX, posY);
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
            //MonoBehaviour.print(teamIndex + "号位置");
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
                float curDis = (attackRole.posIndexVector - targetBr.posIndexVector).sqrMagnitude;
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
            MonoBehaviour.print("index " + chooseBr.index);
            attackRole.setAttackTarget(chooseBr);
        }
    }

    /// <summary>
    /// 角色开始攻击
    /// </summary>
    /// <param name="param"></param>
    private void startRoleAttack(object param)
    {
        //MonoBehaviour.print("this.attackList.Count " + this.attackList.Count);
        if (this.attackList.Count == 0) return;
        this.isStartAttack = true;
        BattleRole br = this.attackList[0];
        br.attack();
        this.attackList.RemoveAt(0);
        bool autoDestroy = false;
        if (this.attackList.Count == 0) autoDestroy = true;
        Delay.setDelay(Layer.Instance.battleScene, 500, startRoleAttack, autoDestroy);
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

    /// <summary>
    /// 是否全部归位
    /// </summary>
    /// <param name="isMyTeam">是否是我方队伍</param>
    /// <returns></returns>
    private bool isAllBack(bool isMyTeam)
    {
        List<BattleRole> team = null;
        if (isMyTeam)
            team = this.myTeam;
        else
            team = this.targetTeam;
        int count = team.Count;
        for (int i = 0; i < count; i++)
        {
            BattleRole br = team[i];
            if (!br.isBack) return false;
        }
        return true;
    }

    /// <summary>
    /// 切换进攻方
    /// </summary>
    private void changeAttacker()
    {
        if (this.isMyTeam && !this.myTeamAttacked) this.myTeamAttacked = true;
        if (!this.isMyTeam && !this.targetTeamAttacked) this.targetTeamAttacked = true;
        if (!this.myTeamAttacked || !this.targetTeamAttacked)
        {
            this.isMyTeam = !this.isMyTeam;
            this.getAttacker(this.isMyTeam, 3);
            this.getAttackTarget(this.isMyTeam);
            this.startRoleAttack(null);
        }
        //如果2次都打完了回合结束
        if (this.myTeamAttacked && this.targetTeamAttacked)
        {
            this._curRound++;
            this.myTeamAttacked = false;
            this.targetTeamAttacked = false;
            this.isMyTeam = true;
            this.isStartAttack = false;
            NotificationCenter.getInstance().postNotification(BattleMsgConstant.ROUND_FINISH);
        }
    }
}