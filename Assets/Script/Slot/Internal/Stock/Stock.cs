using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot.Internal.Stock
{
    /// <summary>
    /// <see cref="Stock"/>へのアクセス
    /// </summary>
    internal interface IStock
    {
        /// <summary>
        /// このストックのレベルをアップグレードする
        /// </summary>
        void UpGrade();
        /// <summary>
        /// このストックを消費する
        /// </summary>
        /// <returns>消費したストックのレベル</returns>
        int Spend();
        /// <summary>
        /// 消費済みのストックかどうか
        /// </summary>
        bool IsSpended { get; }
        /// <summary>
        /// レベル値
        /// </summary>
        int Level { get; }
    }

    /// <summary>
    /// スロットのストック
    /// </summary>
    internal class Stock : IStock
    {
        private int level;
        private readonly BooleanDisposable disposable = new BooleanDisposable();
        public static readonly int MaxLevel = 5;

        public void UpGrade()
        {
            DisposeCheck();
            if(level < MaxLevel) ++level;
        }

        public int Spend()
        {
            DisposeCheck();
            disposable.Dispose();
            //現在のレベルに設定されたStockLevel構造体を返す
            return level;
        }

        public int Level
        {
            get
            {
                DisposeCheck();
                return level;
            }
        }

        public bool IsSpended => disposable.IsDisposed;

        private void DisposeCheck()
        {
            if (disposable.IsDisposed) throw new InvalidOperationException("すでに消費されたストックです");
        }
    }
}