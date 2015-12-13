using UnityEngine;
/// <summary>
/// 游戏内的伤害计算
/// 可自行修改伤害公式
/// </summary>
public class DamageUtils
{
    /// <summary>
    /// 计算伤害值
    /// </summary>
    /// <param name="attackHero">攻击数据</param>
    /// <param name="hurtHero">被攻击数据</param>
    /// <returns></returns>
    public static int mathDamage(HeroVo attackHero, HeroVo hurtHero)
    {
        //这里提供的是最简单的计算公式
        //伤害血量 = max((攻方.攻击力 - 守方.防御力), 1);
        return Mathf.Max((attackHero.atk - hurtHero.def), 1);
    }

    /// <summary>
    /// 判断一次进攻后该武将是否会死亡
    /// </summary>
    /// <param name="attackHero">进攻方数据</param>
    /// <param name="hurtHero">被攻击方数据</param>
    /// <returns></returns>
    public static bool checkRoleDead(HeroVo attackHero, HeroVo hurtHero)
    {
        int damage = DamageUtils.mathDamage(attackHero, hurtHero);
        MonoBehaviour.print("hurtHero.hp " + hurtHero.hp);
        MonoBehaviour.print("damage " + damage);
        return (bool)((hurtHero.hp - damage) <= 0);
    }
}
