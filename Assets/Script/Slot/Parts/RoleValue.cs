using System;
using UnityEngine;

namespace MedalPusher.Slot
{
    [Serializable]
    public struct RoleValue : IEquatable<RoleValue>
    {
        [SerializeField]
        private RoleEnum _value;

        private RoleValue(RoleEnum roleEnum)
        {
            _value = roleEnum;
        }

        /// <summary>
        /// 役の種類数
        /// </summary>
        public static readonly int TotalTypes = Enum.GetValues(typeof(RoleEnum)).Length;

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

        public static bool operator ==(RoleValue left, RoleValue right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(RoleValue left, RoleValue right)
        {
            return left._value != right._value;
        }

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
        public static RoleValue FromEnum(RoleEnum @enum) => new RoleValue(@enum);

        #endregion


    }

    /// <summary>
    /// リールの位置
    /// </summary>
    public enum ReelPos
    {
        Left, Center, Right
    }
    /// <summary>
    /// スロットの役
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
    public struct RoleSet
    {
        public RoleSet(RoleValue left, RoleValue center, RoleValue right)
        {
            this.Left = left;
            this.Center = center;
            this.Right = right;
        }

        public RoleValue Left { get; }
        public RoleValue Center { get; }
        public RoleValue Right { get; }

        /// <summary>
        /// ビンゴになっている役を取得する
        /// ビンゴになっていなければnullが返る
        /// </summary>
        public RoleValue? BingoRole
        {
            get
            {
                if (Left == Center && Center == Right) return Left;
                else return null;
            }
        }
        /// <summary>
        /// 揃っているかどうかを取得する
        /// </summary>
        public bool IsBingo => BingoRole != null;

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
                if (Left == Center)
                    return new ReachInfo(Left, ReelPos.Right);
                if (Center == Right)
                    return new ReachInfo(Center, ReelPos.Left);
                if (Left == Right)
                    return new ReachInfo(Left, ReelPos.Center);

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