using UnityEngine;
using DG.Tweening;
public class Damage
{
    /// <summary>
    /// 显示数字
    /// </summary>
    /// <param name="num">数字</param>
    /// <param name="pos">位置</param>
    public static void show(Transform parent, int num, Vector3 pos, bool isAdd = false)
    {
        GameObject pf = Resources.Load("Prefabs/NumImage") as GameObject;
        GameObject numGo = MonoBehaviour.Instantiate(pf, new Vector3(0, 0), new Quaternion()) as GameObject;
        numGo.transform.SetParent(parent);
        numGo.transform.localScale = new Vector3(1, 1, 1);
        numGo.transform.localPosition = pos;
        NumImage numImage = numGo.GetComponent<NumImage>();
        string str = num.ToString();
        if (!isAdd) str = "-" + str;
        else str = "+" + str;
        numImage.setStr(str);
        float posY = numGo.transform.localPosition.y + 30;
        numGo.transform.DOLocalMoveY(posY, .3f).SetEase(Ease.OutSine).OnComplete(() => {
            GameObject.Destroy(numGo);
            numGo = null;
            numImage = null;
        });
    }
}