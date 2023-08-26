using UnityEngine;

public class FsmExample : MonoBehaviour
{
    private StateMachine machine;
    void Start()
    {
        machine = new StateMachine(this);
        machine.AddNode<FsmEat>();
        machine.AddNode<FsmWork>();
        machine.AddNode<FsmSleep>();

        machine.Run<FsmEat>();
    }

    private void Update()
    {
        //只有当然执行的状态会执行
        machine.Update();
    }
}