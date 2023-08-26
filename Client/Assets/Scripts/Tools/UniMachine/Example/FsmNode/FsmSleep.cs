using UnityEngine;

public class FsmSleep : IStateNode
{
    private StateMachine machine;
    private float workTime;
    public void OnCreate(StateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        workTime = 2f;
        Debug.Log("开始睡觉...");
    }

    public void OnUpdate()
    {
        workTime -= Time.deltaTime;
        if (workTime < 0)
        {
            machine.ChangeState<FsmEat>();
        }
    }

    public void OnExit()
    {
        Debug.Log("睡醒了 新的一天开始了...");
    }
}