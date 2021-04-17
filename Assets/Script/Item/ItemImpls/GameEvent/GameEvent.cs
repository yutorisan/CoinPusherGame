using System;

namespace MedalPusher.Item
{
    /// <summary>
    /// ゲーム内で発生するイベント
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// イベントを発生させる
        /// </summary>
        void Occur();
    }

    public abstract class GameEvent : IGameEvent
    {
        public abstract void Occur();
    }
}