using UnityEngine;

public class BattleRole:object 
{
    //----------人物状态------------
    //站立不动
    public const int STAND = 1;
    //移动
    public const int MOVE = 2;
    //攻击
    public const int ATTACK = 3;
    //受伤
    public const int HURT = 4;
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
    private float speed = .2f;
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
                this.moveAct(this.targetBr.startPos, moveComplete);
                break;
            case BattleConstant.REMOTE:
                //施法动作 + 魔法移动 + 被击动作
                break;
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="pos">目标位置</param>
    /// <param name="actComplete">移动结束回调</param>
    private void moveAct(Vector3 pos, ActComplete actComplete = null)
    {
        this.moveTargetPos = pos;
        float x = this.moveTargetPos.x - this._startPos.x;
        float y = this.moveTargetPos.z - this._startPos.z;
        float angle = Mathf.Atan2(y, x);
        this.vx = Mathf.Cos(angle) * speed;
        this.vz = Mathf.Sin(angle) * speed;
        this.moveCompleteHandler = actComplete;
        this.status = MOVE;
    }

    private void moveComplete()
    {
        this.standStatus();
        //攻击
    }

    public void hurt()
    {
        MonoBehaviour.print("hurt");
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
                //移动结束回调
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
}
