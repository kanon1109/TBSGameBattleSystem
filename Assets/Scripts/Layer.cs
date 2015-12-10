using UnityEngine;
using System.Collections;
//层级管理
public class Layer : MonoBehaviour 
{
    //场景根对象
    public GameObject root;
    //战斗元素UI层
    public GameObject battleUILayer;
    //战斗场景
    public GameObject battleScene;

    private static Layer instance = null;
    public static Layer Instance
    {
        get { return instance; }
    }

	// Use this for initialization
	void Awake ()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public Vector2 getRootRect()
    {
        return new Vector2(root.GetComponent<RectTransform>().rect.width, root.GetComponent<RectTransform>().rect.height);
    }
}
