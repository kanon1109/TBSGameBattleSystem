using UnityEngine;
public class HpBar:MonoBehaviour
{
    //当前血量
    private int hp;
    //最大血量
    private int maxHp;
    //血条对象
    public GameObject hpBarBg;
    //背景条对象
    public GameObject barBg;
    public HpBar()
    { 

    }

    /// <summary>
    /// 设置最大血量
    /// </summary>
    /// <param name="maxHp">最大血量</param>
    public void setMaxHp(int maxHp)
    {
        this.maxHp = maxHp;
        this.setHp(maxHp);
    }

    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="hp">血量</param>
    public void setHp(int hp)
    {
        if (hp < 0) hp = 0;
        this.hp = hp;
        float p = (float)this.hp / (float)this.maxHp;
        this.hpBarBg.transform.localScale = new Vector3(p, 1, 1);
    }
}