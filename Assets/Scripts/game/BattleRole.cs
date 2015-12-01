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
    //位置索引
    private int _index = -1;
    //起始位置
    private Vector3 _startPos;
    //绑定的武将数据
    private HeroVo hVo;
    //攻击的目标
    private BattleRole targetBr = null;
    //人物状态
    private int status = STAND;
    //移动目标位置
    private Vector3 moveTargetPos;
    //动作回调代理
    private delegate void ActComplete();
    //速度
    private float speed = 1f;
    //横向速度
    private float vx = 0;
    //纵向速度
    private float vz = 0;
    //渲染计时器
    private Timer timer = null;
    //最小距离
    private float minDis = 1;
    //移动结束后的回调
    private ActComplete moveCompleteHandler = null;
    //是否往回移动
    private bool isMoveBack = false;
    //是否是我方队伍的人
    private bool _isMyTeam = false;
    //当前伤害的血量
    private int curDamage = 0;
    //是否归位
    private bool _isBack = true;
    //-------------get set ---------------
    public int index
    {
        get { return _index; }
        set { _index = value; }
    }

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

    public bool isBack
    {
        get { return _isBack; }
    }

    public BattleRole()
    {
        
    }

    /// <summary>
    /// 移动到某个地方
    /// </summary>
    /// <param name="pos">目标位置</param>
    /// <param name="isBack">是否返回原位</param>
    /// <param name="actComplete">移动结束回调</param>
    private void moveTo(Vector3 pos, bool isBack, ActComplete actComplete = null)
    {
        this._isBack = false;
        this.isMoveBack = isBack;
        this.moveTargetPos = pos;
        Vector3 curPos = this.roleGo.transform.localPosition;
        float x = this.moveTargetPos.x - curPos.x;
        float y = this.moveTargetPos.z - curPos.z;
        float angle = Mathf.Atan2(y, x);
        this.vx = Mathf.Cos(angle) * speed;
        this.vz = Mathf.Sin(angle) * speed;
        this.moveCompleteHandler = actComplete;
        this.status = MOVE;
    }

    private void moveToTargetComplete()
    {
        this.status = STAND;
        this.vx = 0;
        this.vz = 0;
        if (!this.isMoveBack)
        {
            //攻击动作
            float posX = this.roleGo.transform.localPosition.x;
            if(this.isMyTeam) posX += 1.5f;
            else posX -= 1.5f;
            this.roleGo.transform.DOLocalMoveX(posX, .1f).SetDelay(.5f).SetLoops(2, LoopType.Yoyo).OnComplete(attackCompleteHandler);
        }
        else
        {
            //回原位
            this.roleGo.transform.localPosition = this._startPos;
            this._isBack = true;
            NotificationCenter.getInstance().postNotification(BattleMsgConstant.ROLE_BACK);
        }
    }

    //攻击动作结束
    private void attackCompleteHandler()
    {
        int damage = DamageUtils.mathDamage(this.heroVo, this.targetBr.heroVo);
        //目标受伤
        this.targetBr.hurt(damage);
        //移动回原位
        this.moveTo(this.startPos, true, moveToTargetComplete);
    }

    /// <summary>
    /// 站立动作
    /// </summary>
    private void standStatus()
    {
        if (this.status == STAND)
        {
            this.vx = 0;
            this.vz = 0;
        }
    }

    /// <summary>
    /// 移动状态
    /// </summary>
    private void moveStatus()
    {
        if (this.status == MOVE)
        {
            //更新位置
            this.roleGo.transform.localPosition = new Vector3(this.roleGo.transform.localPosition.x + this.vx,
                                                              this.roleGo.transform.localPosition.y,
                                                              this.roleGo.transform.localPosition.z + this.vz);
            float dis = Vector3.Distance(this.roleGo.transform.localPosition, this.moveTargetPos);
            if (dis <= this.minDis)
            {
                if (this.moveCompleteHandler != null)
                    this.moveCompleteHandler.Invoke();
            }
        }
    }

    /// <summary>
    /// 更新角色状态
    /// </summary>
    private void updateStatus()
    {
        this.standStatus();
        this.moveStatus();
    }

    /// <summary>
    /// 帧循环
    /// </summary>
    private void updateHandler()
    {
        this.updateStatus();
    }

    /// <summary>
    /// 是否死亡
    /// </summary>
    /// <returns></returns>
    private bool isDead()
    {
        return this.heroVo.hp <= 0;
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
    /// 普通攻击
    /// </summary>
    public void attack()
    {
        //没有目标
        if (this.targetBr == null) return;
        //近战和远程
        int type = BattleConstant.CLOSE;
        switch (type)
        {
            case BattleConstant.CLOSE:
                //移动 + 打击 + 被击动作 + 移动回来
                Vector3 pos = new Vector3(-1, 0);
                if (!this.isMyTeam) pos = new Vector3(1, 0);
                Vector3 targetPos = this.targetBr.startPos + pos;
                this.moveTo(targetPos, false, moveToTargetComplete);
                break;
            case BattleConstant.REMOTE:
                //施法动作 + 魔法移动 + 被击动作
                break;
        }
    }

    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="damage">扣的血量</param>
    public void hurt(int damage)
    {
        //受伤效果
        MonoBehaviour.print("hurt");
        this.curDamage = damage;
        this.heroVo.hp -= damage;
        //发送死亡消息
        if(this.isDead()) NotificationCenter.getInstance().postNotification(BattleMsgConstant.ROLE_DEAD, this);
        float posX = this.roleGo.transform.localPosition.x;
        if(!this.isMyTeam) posX += 1.5f;
        else posX -= 1.5f;
        this.roleGo.transform.DOLocalMoveX(posX, .1f).SetLoops(2, LoopType.Yoyo).OnComplete(hurtCompleteHandler);
    }


    //受伤动作结束
    private void hurtCompleteHandler()
    {
        //扣血效果
        if(this.isDead())
            this.destroy();
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
    /// 销毁
    /// </summary>
    public void destroy()
    {
        GameObject.Destroy(this.roleGo);
        this.roleGo = null;
        this.timer = null;
        this.heroVo = null;
    }
}
