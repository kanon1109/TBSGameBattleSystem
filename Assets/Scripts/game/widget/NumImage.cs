using UnityEngine;
public class NumImage:MonoBehaviour
{
    /// <summary>
    /// 显示掉血
    /// </summary>
    /// <param name="parent">父级容器</param>
    /// <param name="num">血量</param>
    /// <param name="pos">位置</param>
    public void setNum(int num)
    {
        string numStr = num.ToString();
        int length = numStr.Length;
        for (int i = 0; i < length; i++)
        {
            string str = numStr[i].ToString();
            GameObject pf = Resources.Load("Prefabs/num/" + str) as GameObject;
            GameObject numGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
            numGo.transform.SetParent(this.gameObject.transform);
            numGo.transform.localScale = new Vector3(1, 1, 1);
            numGo.transform.localPosition = new Vector3();
        }
    }

    /// <summary>
    /// 设置文字
    /// </summary>
    /// <param name="numStr">文字字符串</param>
    public void setStr(string numStr)
    {
        int length = numStr.Length;
        for (int i = 0; i < length; i++)
        {
            string str = numStr[i].ToString();
            GameObject pf = Resources.Load("Prefabs/num/" + str) as GameObject;
            GameObject numGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
            numGo.transform.SetParent(this.gameObject.transform);
            numGo.transform.localScale = new Vector3(1, 1, 1);
            numGo.transform.localPosition = new Vector3();
        }
    }
}