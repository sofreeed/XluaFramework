using System.Collections.Generic;
using UnityEngine;

namespace TypeEvent.Example
{
    public class TypeEventSystemUnRegisterExample : MonoBehaviour
    {
        public struct EventA
        {
        }

        public struct EventB
        {
        }

        private void Start()
        {
            TypeEventSystem.Global.Register<EventA>(OnEventA);
            //自动反注册
            TypeEventSystem.Global.Register<EventB>(b => { }).UnRegisterWhenGameObjectDestroyed(this);
        }

        void OnEventA(EventA e)
        {
        }

        private void OnDestroy()
        {
            //单独调用反注册
            TypeEventSystem.Global.UnRegister<EventA>(OnEventA);
        }


        public class NoneMonoScript : IUnRegisterList
        {
            //实现接口 反注册列表
            public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();


            void Start()
            {
                //自动反注册
                TypeEventSystem.Global.Register<EventA>(a => { }).AddToUnregisterList(this);
            }

            void OnDestroy()
            {
                //继承接口后，执行反注册的方法，手动调用
                this.UnRegisterAll();
            }
        }
    }
}