using UnityEngine;

public class BattleRole:object 
{
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
    /// <param name="type">攻击类型 1近战，2远程</param>
    public void attack(int type)
    {
        //近战和远程
        MonoBehaviour.print("attack");
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
}
