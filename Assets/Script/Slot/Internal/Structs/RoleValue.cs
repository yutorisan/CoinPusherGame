﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace MedalPusher.Slot
{
    [Serializable]
    public struct RoleValue : IEquatable<RoleValue>, IComparable<RoleValue>
    {
        private static readonly IReadOnlyList<RoleValue> _all = Enum.GetValues(typeof(RoleEnum))
                                                                    .Cast<RoleEnum>()
                                                                    .Select(e => new RoleValue(e))
                                                                    .ToList();
        [SerializeField]
        private RoleEnum _value;

        private RoleValue(RoleEnum roleEnum)
        {
            _value = roleEnum;
        }

        /// <summary>
        /// 役の種類数
        /// </summary>
        public static readonly int TotalTypes = _all.Count;
        public static readonly IReadOnlyList<RoleValue> Every = _all;
        /// <summary>
        /// startからendまで正転するときのRole順リストを取得する
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<RoleValue> ForwardLoops(RoleValue start, RoleValue end)
        {
            RoleValue now = start;
            yield return now;
            do
            {
                now = now.Next;
                yield return now;
            } while (now != end);
        }
        /// <summary>
        /// startからendまで逆転するときのRole順リストを取得する
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<RoleValue> BackwardLoops(RoleValue start, RoleValue end)
        {
            RoleValue now = start;
            yield return now;
            do
            {
                now = now.Previous;
                yield return now;
            } while (now != end);
        }

        /// <summary>
        /// リールの並び順において、Role間の差分の個数を算出する
        /// fromからtargetまで何個Roleがあるか（targetを含む個数）
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int RemainCount(RoleValue from, RoleValue target)
        {
            int diff = target.Index - from.Index;
            return diff >= 0 ? diff : diff + TotalTypes;
        }
        /// <summary>
        /// 次の役柄を取得する
        /// </summary>
        public RoleValue Next
        {
            get
            {
                if(Index == TotalTypes - 1) //最後尾のRoleの場合
                {
                    return new RoleValue((RoleEnum)0); //RoleEnumの先頭を返す
                }
                else
                {
                    return new RoleValue((RoleEnum)((int)_value + 1)); //次のRoleEnumを返す
                }
            }
        }
        /// <summary>
        /// 前の役柄を取得する
        /// </summary>
        public RoleValue Previous
        {
            get
            {
                if (Index == 0) //先頭のRoleの場合
                {
                    return new RoleValue((RoleEnum)(TotalTypes - 1)); //最後のRoleEnumを返す
                }
                else
                {
                    return new RoleValue((RoleEnum)((int)_value - 1)); //前のRoleEnumを返す
                }
            }
        }
        /// <summary>
        /// この役から指定の役まで正転したときの役リストを取得する
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<RoleValue> ForwardTo(RoleValue end) => ForwardLoops(this, end);
        /// <summary>
        /// この役から指定の役まで逆転したときの役リストを取得する
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<RoleValue> BackwardTo(RoleValue end) => BackwardLoops(this, end);
        /// <summary>
        /// Roleの順番
        /// </summary>
        public int Index => (int)_value;

        #region Factory
        public static readonly RoleValue Role0 = new RoleValue(RoleEnum.Role0);
        public static readonly RoleValue Role1 = new RoleValue(RoleEnum.Role1);
        public static readonly RoleValue Role2 = new RoleValue(RoleEnum.Role2);
        public static readonly RoleValue Role3 = new RoleValue(RoleEnum.Role3);
        public static readonly RoleValue Role4 = new RoleValue(RoleEnum.Role4);
        public static readonly RoleValue Role5 = new RoleValue(RoleEnum.Role5);
        public static readonly RoleValue Role6 = new RoleValue(RoleEnum.Role6);
        public static readonly RoleValue Role7 = new RoleValue(RoleEnum.Role7);
        public static readonly RoleValue Role8 = new RoleValue(RoleEnum.Role8);
        public static readonly RoleValue Role9 = new RoleValue(RoleEnum.Role9);
        /// <summary>
        /// intからRoleを取得します
        /// </summary>
        /// <param name="n"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static RoleValue FromInt32(int n)
        {
            if (!Enum.IsDefined(typeof(RoleEnum), n))
                throw new ArgumentOutOfRangeException(nameof(n), $"指定された値を{nameof(RoleValue)}に変換できません。");
            return new RoleValue((RoleEnum)n);    
        }
        /// <summary>
        /// すべての役の中からランダムの役を返します
        /// </summary>
        /// <returns></returns>
        public static RoleValue FromRandom()
        {
            return FromInt32(UnityEngine.Random.Range(0, TotalTypes));
        }
        /// <summary>
        /// 引数に指定した役を除いたなかから、ランダムで返します
        /// </summary>
        /// <param name="except">除く役</param>
        /// <returns></returns>
        public static RoleValue FromRandom(params RoleValue[] except)
        {
            if (except.Length == TotalTypes) throw new InvalidOperationException();
            RoleValue retRole;
            do
            {
                retRole = FromRandom();
            } while (except.Any(role => role == retRole));
            return retRole;
        }
        public static RoleValue FromEnum(RoleEnum @enum) => new RoleValue(@enum);

        #endregion

        public override bool Equals(object obj)
        {
            return obj is RoleValue role && Equals(role);
        }

        public bool Equals(RoleValue other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return -1939223833 + _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public int CompareTo(RoleValue other)
        {
            return Index.CompareTo(other.Index);
        }

        public static bool operator ==(RoleValue left, RoleValue right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(RoleValue left, RoleValue right)
        {
            return left._value != right._value;
        }
    }

    /// <summary>
    /// リールの位置
    /// </summary>
    public enum ReelPos
    {
        Left, Middle, Right
    }
    /// <summary>
    /// スロットの役（ここに定義された順番にリール上に並ぶ）
    /// </summary>
    public enum RoleEnum
    {
        Role0,
        Role1,
        Role2,
        Role3,
        Role4,
        Role5,
        Role6,
        Role7,
        Role8,
        Role9,
    }

    /// <summary>
    /// スロットの出目
    /// </summary>
    public readonly struct RoleSet
    {
        public RoleSet(RoleValue left, RoleValue middle, RoleValue right)
        {
            this.Left = left;
            this.Middle = middle;
            this.Right = right;
        }

        public override string ToString()
        {
            return $"{Left}, {Middle}, {Right}";
        }

        public RoleValue this[ReelPos index]
        {
            get
            {
                switch (index)
                {
                    case ReelPos.Left: return Left;
                    case ReelPos.Middle: return Middle;
                    case ReelPos.Right: return Right;
                    default: throw new InvalidEnumArgumentException();
                }
            }
        }

        public RoleValue Left { get; }
        public RoleValue Middle { get; }
        public RoleValue Right { get; }

        /// <summary>
        /// ビンゴになっている役を取得する
        /// ビンゴになっていなければnullが返る
        /// </summary>
        public RoleValue? BingoRole
        {
            get
            {
                if (Left == Middle && Middle == Right) return Left;
                else return null;
            }
        }
        /// <summary>
        /// 揃っているかどうかを取得する
        /// </summary>
        public bool IsBingo => BingoRole.HasValue;

        /// <summary>
        /// リーチ情報を取得する
        /// リーチじゃなければnullが返る
        /// </summary>
        public ReachInfo? ReachStatus
        {
            get
            {
                //ビンゴならリーチではないのでnull
                if (IsBingo) return null;

                //いずれかリーチならその情報を返す
                if (Left == Middle)
                    return new ReachInfo(Left, ReelPos.Right);
                if (Middle == Right)
                    return new ReachInfo(Middle, ReelPos.Left);
                if (Left == Right)
                    return new ReachInfo(Right, ReelPos.Middle);

                //何も揃っていないならnull
                return null;
            }
        }
        /// <summary>
        /// リーチかどうかを取得する
        /// </summary>
        public bool IsReach => ReachStatus != null;

        /// <summary>
        /// リーチの状態を示す情報
        /// </summary>
        public struct ReachInfo
        {
            public ReachInfo(RoleValue reachRole, ReelPos reachPos)
            {
                this.ReachedRole = reachRole;
                this.ReachReelPos = reachPos;
            }
            /// <summary>
            /// リーチになっている役柄
            /// </summary>
            public RoleValue ReachedRole { get; }
            /// <summary>
            /// 揃っていないリールの位置
            /// </summary>
            public ReelPos ReachReelPos { get; }
        }
    }
}