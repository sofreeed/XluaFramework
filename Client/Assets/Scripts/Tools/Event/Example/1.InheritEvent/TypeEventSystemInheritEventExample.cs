using UnityEngine;

namespace TypeEvent.Example
{
    public class TypeEventSystemInheritEventExample : MonoBehaviour
    {
        public interface IEventA
        {
        }

        /// 事件继承
        public struct EventB : IEventA
        {
        }

        private void Start()
        {
            //使用自动反注册！
            TypeEventSystem.Global.Register<IEventA>(e => { Debug.Log(e.GetType().Name); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<IEventA>(new EventB());

                //注意：无效，这里需要使用注册的类型！！！
                TypeEventSystem.Global.Send<EventB>();
            }
        }
    }
}