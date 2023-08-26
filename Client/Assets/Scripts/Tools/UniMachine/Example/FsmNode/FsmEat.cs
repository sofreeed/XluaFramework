using UnityEngine;

public class FsmEat : IStateNode
{
    private StateMachine machine;

    private float eatTime;
    
    public void OnCreate(StateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        eatTime = 1f;
        Debug.Log("开始时吃饭...");
    }

    public void OnUpdate()
    {
        eatTime -= Time.deltaTime;
        if (eatTime < 0)
        {
            machine.ChangeState<FsmWork>();
        }
    }

    public void OnExit()
    {
        Debug.Log("吃完了...");
    }
}
