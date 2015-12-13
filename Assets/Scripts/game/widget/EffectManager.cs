using UnityEngine;
public class EffectManager
{
    /// <summary>
    /// 创建一个技能效果
    /// </summary>
    /// <param name="pos">初始位置</param>
    /// <param name="name">技能名称</param>
    /// <param name="Transform">父级名称</param>
    /// <returns>技能效果</returns>
    public static GameObject createEffect(Vector3 pos, string name, Transform parent)
    {
        GameObject pf = Resources.Load("Prefabs/" + name) as GameObject;
        GameObject effectGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
        effectGo.transform.SetParent(parent);
        effectGo.transform.localScale = new Vector3(1, 1, 1);
        effectGo.transform.localPosition = pos;
        return effectGo;
    }
}