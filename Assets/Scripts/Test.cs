using UnityEngine;

public class Test:MonoBehaviour
{
    BattleController bc;
    public Test()
    {
        bc = new BattleController();
    }

    void Start()
    {
        bc.init(this.gameObject.transform);
    }

    void Update()
    {
        KeyboardManager.updateKey();
    }
}