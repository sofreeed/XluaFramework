using UnityEngine;

namespace TypeEvent.Example
{
    public class TypeEventSystemBasicExample : MonoBehaviour
    {
        public struct TestEventA
        {
            public int Age;
        }

        private void OnEnable()
        {
            TypeEventSystem.Global.Register<TestEventA>(OnTestEventAProcess);
        }

        private void OnDisable()
        {
            TypeEventSystem.Global.UnRegister<TestEventA>(OnTestEventAProcess);
        }

        void OnTestEventAProcess(TestEventA e)
        {
            Debug.Log(e.Age);
        }

        private void Update()
        {
            // 鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send(new TestEventA()
                {
                    Age = 18
                });
            }

            // 鼠标右键点击
            if (Input.GetMouseButtonDown(1))
            {
                //注意：如果参数为空，那么内部会调用new T();
                TypeEventSystem.Global.Send<TestEventA>();
            }
        }
    }
}