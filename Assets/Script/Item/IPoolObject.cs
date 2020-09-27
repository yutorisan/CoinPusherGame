using System;
using System.Security;

namespace MedalPusher.Item
{
    /// <summary>
    /// オブジェクトプールで管理されるオブジェクトであることを示す
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// オブジェクトプールへ返します。
        /// </summary>
        void ReturnToPool();
    }
}