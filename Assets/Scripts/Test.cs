using support;
using UnityEngine;
using UnityEngine.UI;

public class Test:MonoBehaviour
{
    BattleController bc;
    public GameObject battleScene;
    public Text roundTxt;
    public Test()
    {
        this.bc = new BattleController();
    }

    void Start()
    {
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.ROUND_FINISH, roundFinishHandler);
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.MY_TEAM_ALL_DEAD, myTeamAllDeadHandler);
        NotificationCenter.getInstance().addObserver(BattleMsgConstant.TARGET_TEAM_ALL_DEAD, targetTeamAllDeadHandler);
        this.bc.init();
        this.roundTxt.text = this.bc.curRound.ToString();
    }

    void Update()
    {
        KeyboardManager.updateKey();
    }

    private void roundFinishHandler(object param)
    {
        this.roundTxt.text = this.bc.curRound.ToString();
    }

    private void targetTeamAllDeadHandler(object param)
    {
        MonoBehaviour.print("敌方全部死亡");
    }

    private void myTeamAllDeadHandler(object param)
    {
        MonoBehaviour.print("我方全部死亡");
    }
}