using DG.Tweening;
using UnityEngine;
using support;
public class BattleRole : object 
{
    //----------人物状态------------
    //站立不动
    public const int STAND = 1;
    //移动
    public const int MOVE = 2;
    //--------------------------
    //人物模型
    private GameObject roleGo = null;
    //起始位置
    private Vector3 _startPos;
    //绑定的武将数据
    private HeroVo hVo;
    //攻击的目标
    private BattleRole targetBr = null;
    //渲染计时器
    private Timer timer = null;
    //最小距离
    private float minDis = 1;
    //是否往回移动
    private bool isMoveBack = false;
    //是否是我方队伍的人
    private bool _isMyTeam = false;
    //当前伤害的血量
    private int curDamage = 0;
    //是否归位
    private bool _isAttacking = false;
    //血条
    private HpBar hpBar;
    private GameObject hpBarGo;
    //-------------get set ---------------
    public Vector3 startPos
    {
        get { return _startPos; }
    }

    public HeroVo heroVo
    {
        get { return hVo; }
        set { hVo = value; }
    }

    public bool isMyTeam
    {
        get { return _isMyTeam; }
        set { _isMyTeam = value; }
    }
    //是否正处于攻击状态
    public bool isAttacking
    {
        get { return _isAttacking; }
        set { _isAttacking = value; }
    }
    //位置索引
    public Vector2 posIndexVector;
    //位置索引
    public int index = -1;
    //上一个位置
    public int prevIndex = -1;
    //下一个位置
    public int nextIndex = -1;
    //标记是否死亡
    public bool isDeaded = false;
    public BattleRole()
    {
        
    }

    /// <summary>
    /// 创建角色模型
    /// </summary>
    /// <param name=parent>父级容器</param>
    /// <returns></returns>
    public void create(Transform parent, Vector3 pos = new Vector3())
    {
        GameObject pf = Resources.Load("Prefabs/role") as GameObject;
        this.roleGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
        this.roleGo.transform.SetParent(parent);
        this.roleGo.transform.localScale = new Vector3(1, 1, 1);
        this.roleGo.transform.localPosition = pos;
        this._startPos = pos;
        if (this.timer == null)
            this.timer = this.roleGo.AddComponent<Timer>();
        else
            this.timer = this.roleGo.GetComponent<Timer>();
        this.timer.createTimer(.01f, -1, updateHandler);
        this.timer.start();
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="pos">位置</param>
    public void setPosition(Vector3 pos)
    {
        if (this.roleGo == null) return;
        this.roleGo.transform.localPosition = pos;
        this._startPos = pos;
    }

    /// <summary>
    /// 创建血条
    /// </summary>
    /// <param name="parent">父级容器</param>
    public void createHpBar(Transform parent)
    {
        if (this.hpBar == null)
        {
            GameObject pf = Resources.Load("Prefabs/hpBar") as GameObject;
            this.hpBarGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
            this.hpBarGo.transform.SetParent(parent);
            this.hpBarGo.transform.localScale = new Vector3(1, 1, 1);
            this.hpBarGo.transform.localPosition = new Vector3();
            this.hpBar = this.hpBarGo.GetComponent<HpBar>();
        }
    }

    /// <summary>
    /// 初始化满血血量
    /// </summary>
    /// <param name="maxHp">满血</param>
    public void initHpBarMax(int maxHp)
    {
        if (this.hpBar != null)
            this.hpBar.setMaxHp(maxHp);
    }

    /// <summary>
    /// 更新血条位置
    /// </summary>
    private void updateHpBarPos()
    {
        if (this.hpBarGo != null)
            this.hpBarGo.transform.position =
                Camera.main.WorldToScreenPoint(this.roleGo.transform.position) + new Vector3(0, 30, 0);
    }

    /// <summary>
    /// 移动到某个地方
    /// </summary>
    /// <param name="pos">目标位置</param>
    /// <param name="isBack">是否返回原位</param>
    /// <param name="actComplete">移动结束回调</param>
    private void moveAct(Vector3 pos, bool isBack, TweenCallback actComplete = null)
    {
        this.isMoveBack = isBack;
        this.roleGo.transform.DOLocalMove(pos, .4f).SetEase(Ease.Linear).OnComplete(actComplete);
    }

    private void moveToTargetComplete()
    {
        if (!this.isMoveBack)
        {
            //攻击动作
            this.attackAct();
        }
        else
        {
            //回原位
            MonoBehaviour.print("moveBack");
            this.roleGo.transform.localPosition = this._startPos;
            this._isAttacking = false;
            NotificationCenter.getInstance().postNotification(BattleMsgConstant.ROLE_BACK);
        }
    }

    /// <summary>
    /// 帧循环
    /// </summary>
    private void updateHandler()
    {
        this.updateHpBarPos();
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    public void attack()
    {
        //没有目标
        if (this.targetBr == null) return;
        if (this.heroVo == null) return;
        //近战和远程
        switch (this.heroVo.attackType)
        {
            case BattleConstant.CLOSE:
                //移动 + 打击 + 被击动作 + 移动回来
                Vector3 pos = new Vector3(-1, 0);
                if (!this.isMyTeam) pos = new Vector3(1, 0);
                Vector3 targetPos = this.targetBr.startPos + pos;
                this.moveAct(targetPos, false, moveToTargetComplete);
                break;
            case BattleConstant.REMOTE:
                //施法动作 + 魔法移动 + 被击动作
                this.skillAttackAct();
                break;
        }
    }

    /// <summary>
    /// 施法动作
    /// </summary>
    private void skillAttackAct()
    {
        float posX = this.roleGo.transform.localPosition.x;
        if (this.isMyTeam) posX -= 1.5f;
        else posX += 1.5f;
        this.roleGo.transform.DOLocalMoveX(posX, .1f).SetLoops(2, LoopType.Yoyo).OnComplete(skillAttackCompleteHandler);
    }

    /// <summary>
    /// 施法动作结束
    /// </summary>
    private void skillAttackCompleteHandler()
    {
        //创建效果
        GameObject effectGo = EffectManager.createEffect(this.startPos,
                                                        "skillEffect",
                                                        Layer.Instance.battleScene.transform);
        //移动效果
        effectGo.transform.DOLocalMove(this.targetBr.startPos, .3f).SetEase(Ease.Linear).OnComplete(() => effectMoveCompleteHandler(effectGo));
    }

    private void effectMoveCompleteHandler(GameObject effectGo)
    {
        this._isAttacking = false;
        this.isMoveBack = true;
        GameObject.Destroy(effectGo);
        effectGo = null;
        //执行伤害
        int damage = DamageUtils.mathDamage(this.heroVo, this.targetBr.heroVo);
        //目标受伤
        this.targetBr.hurt(damage);
        this.moveToTargetComplete();
    }

    /// <summary>
    /// 普通攻击动作
    /// </summary>
    private void attackAct()
    {
        float posX = this.roleGo.transform.localPosition.x;
        if (this.isMyTeam) posX += 1.5f;
        else posX -= 1.5f;
        this.roleGo.transform.DOLocalMoveX(posX, .1f).SetDelay(.5f).SetLoops(2, LoopType.Yoyo).OnComplete(attackCompleteHandler);
    }

    /// <summary>
    /// 攻击动作结束
    /// </summary>
    private void attackCompleteHandler()
    {
        int damage = DamageUtils.mathDamage(this.heroVo, this.targetBr.heroVo);
        //目标受伤
        this.targetBr.hurt(damage);
        //移动回原位
        this.moveAct(this.startPos, true, moveToTargetComplete);
    }

    /// <summary>
    /// 受伤动作
    /// </summary>
    private void hurtAct()
    {
        float posX = this.roleGo.transform.localPosition.x;
        if (!this.isMyTeam) posX += 1.5f;
        else posX -= 1.5f;
        this.roleGo.transform.DOLocalMoveX(posX, .1f).SetLoops(2, LoopType.Yoyo).OnComplete(hurtCompleteHandler);
    }

    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="damage">扣的血量</param>
    public void hurt(int damage)
    {
        //受伤效果
        this.curDamage = damage;
        this.heroVo.hp -= damage;
        Damage.show(Layer.Instance.battleUILayer.transform, damage, this.hpBar.transform.localPosition);
        if (this.hpBar != null) 
            this.hpBar.setHp(this.heroVo.hp);
        //发送死亡消息
        if (this.isDead()) 
            NotificationCenter.getInstance().postNotification(BattleMsgConstant.ROLE_DEAD, this);
        this.hurtAct();
    }

    //受伤动作结束
    private void hurtCompleteHandler()
    {
        //扣血效果
        if(this.isDead())
        {
            MonoBehaviour.print("dead");
            this.destroy();
        }
    }

    /// <summary>
    /// 设置攻击者的目标对象
    /// </summary>
    /// <param name="br">目标对象</param>
    public void setAttackTarget(BattleRole br)
    {
        this.targetBr = br;
    }

    /// <summary>
    /// 是否死亡
    /// </summary>
    /// <returns></returns>
    private bool isDead()
    {
        if (this.heroVo == null) return true;
        return this.heroVo.hp <= 0;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void destroy()
    {
        if (this.timer != null) 
            this.timer.stop();
        if (this.roleGo != null) 
            this.roleGo.transform.DOKill();
        GameObject.Destroy(this.roleGo);
        this.roleGo = null;
        GameObject.Destroy(this.hpBarGo);
        this.hpBarGo = null;
        this.hpBar = null;
        this.timer = null;
        this.heroVo = null;
    }
}
