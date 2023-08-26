using UnityEngine;

public class FsmWork : IStateNode
{
    private StateMachine machine;
    private float workTime;
    public void OnCreate(StateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        workTime = 3f;
        Debug.Log("开始上班...");
    }

    public void OnUpdate()
    {
        workTime -= Time.deltaTime;
        if (workTime < 0)
        {
            machine.ChangeState<FsmSleep>();
        }
    }

    public void OnExit()
    {
        Debug.Log("下班回家...");
    }
}