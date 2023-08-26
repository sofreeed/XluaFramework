using UnityEngine;

namespace TypeEvent.Example
{
    public struct InterfaceEventA
    {
    }

    public struct InterfaceEventB
    {
    }

    public class InterfaceEventModeExample : MonoBehaviour
        , IOnEvent<InterfaceEventA>
        , IOnEvent<InterfaceEventB>
    {
        /*
         * 继承IOnEvent接口后，实现方法OnEvent(T t)
         * 具备了能力this.RegisterEvent()和this.UnRegisterEvent()
         * 执行上面方法的时候，自动将OnEvent(T t)注册到事件系统
         */
        
        //实现接口方法
        public void OnEvent(InterfaceEventA e)
        {
            Debug.Log(e.GetType().Name);
        }

        //实现接口方法
        public void OnEvent(InterfaceEventB e)
        {
            Debug.Log(e.GetType().Name);
        }

        
        
        private void Start()
        {
            this.RegisterEvent<InterfaceEventA>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<InterfaceEventB>();
        }

        private void OnDestroy()
        {
            this.UnRegisterEvent<InterfaceEventB>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<InterfaceEventA>();
                TypeEventSystem.Global.Send<InterfaceEventB>();
            }
        }
    }
}