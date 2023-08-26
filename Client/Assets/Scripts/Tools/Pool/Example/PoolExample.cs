using UnityEngine;

namespace Pool.Example
{
    public class PoolExample : MonoBehaviour
    {
        /*
         * 对象池有两种SimpleObjectPool和SafeObjectPool
         * SimpleObjectPool生产的对象没有约束
         *      创建对象池的时候需要提供Reset和Destory方法
         *      Allocate和Recycle2Cache方法需要使用对象池的API
         * SafeObjectPool生产的对象需要实现IPoolable和IPoolType接口，所以更安全一些
         *      对象实现接口方法OnRecycled进行Reset
         *      Allocate和Recycle2Cache可以使用对象方法封装对象池API，使用方便一些
         *
         * 注意1：SafeObjectPool的安全性还体现在最大个数限制，如果超过最大个数，回收的时候不会进池子
         * 注意2：因为SafeObjectPool生产的对象需要有约束，所以Unity的组件需要用SimpleObjectPool，
         *      并提供生产和Reset的方法。
         * 
         * 生产工厂（也有提供生产函数的接口）：
         * DefaultObjectFactory：通过new的方式，需要public构造函数
         * NonPublicObjectFactory：没有public构造函数，使用反射
         * CustomObjectFactory：自定义生产工厂
         *
         * SafeObjectPool默认使用DefaultObjectFactory
         * SimpleObjectPool需要自己提供生产工厂或者函数
         */

        class Fish
        {
        }

        private void Start()
        {
            #region SimpleObjectPool

            var pool = new SimpleObjectPool<Fish>(() => new Fish(), initCount: 50);
            Debug.Log(pool.CurCount);

            var fish = pool.Allocate();
            Debug.Log(pool.CurCount);

            pool.Recycle(fish);
            Debug.Log(pool.CurCount);

            //GameObject的对象池，提供生产和Reset方法
            var gameObjPool = new SimpleObjectPool<GameObject>(() =>
            {
                var gameObj = new GameObject("AGameObject");
                // 初始化...
                return gameObj;
            }, (gameObj) =>
            {
                // 回收对象Reset
                //gameObj.name = "test";
            });
            //提供销毁的方法
            gameObjPool.Clear(gameObj => { Destroy(gameObj); });

            #endregion


            #region SafeObjectPool

            //设置生产方法
            SafeObjectPool<Bullet>.Instance.SetFactoryMethod(() => { return new Bullet(); });
            //设置生产工厂
            SafeObjectPool<Bullet>.Instance.SetObjectFactory(new DefaultObjectFactory<Bullet>());

            SafeObjectPool<Bullet>.Instance.Init(50, 25);

            var bullet = Bullet.Allocate();
            Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);

            bullet.Recycle2Cache();
            Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);

            #endregion
        }


        class Bullet : IPoolable, IPoolType
        {
            public void OnRecycled()
            {
                Debug.Log("回收了");
            }

            public bool IsRecycled { get; set; }

            public static Bullet Allocate()
            {
                return SafeObjectPool<Bullet>.Instance.Allocate();
            }

            public void Recycle2Cache()
            {
                SafeObjectPool<Bullet>.Instance.Recycle(this);
            }
        }
    }

// 50
// 49
// 50
// 回收了 x 25
// 24
// 回收了 24
// 回收了
// 回收了 25
}