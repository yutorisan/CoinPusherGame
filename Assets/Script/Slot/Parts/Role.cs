using System;

namespace MedalPusher.Slot
{
    public struct Role : IEquatable<Role>
    {
        private readonly RoleEnum _value;
        private Role(RoleEnum roleEnum)
        {
            _value = roleEnum;
        }

        /// <summary>
        /// 役の種類数
        /// </summary>
        public static readonly int TotalTypes = Enum.GetValues(typeof(RoleEnum)).Length;

        public override bool Equals(object obj)
        {
            return obj is Role role && Equals(role);
        }

        public bool Equals(Role other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return -1939223833 + _value.GetHashCode();
        }

        public static bool operator ==(Role left, Role right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Role left, Role right)
        {
            return left._value != right._value;
        }

        #region Factory
        public static readonly Role Role0 = new Role(RoleEnum.Role0);
        public static readonly Role Role1 = new Role(RoleEnum.Role1);
        public static readonly Role Role2 = new Role(RoleEnum.Role2);
        public static readonly Role Role3 = new Role(RoleEnum.Role3);
        public static readonly Role Role4 = new Role(RoleEnum.Role4);
        public static readonly Role Role5 = new Role(RoleEnum.Role5);
        public static readonly Role Role6 = new Role(RoleEnum.Role6);
        public static readonly Role Role7 = new Role(RoleEnum.Role7);
        public static readonly Role Role8 = new Role(RoleEnum.Role8);
        public static readonly Role Role9 = new Role(RoleEnum.Role9);
        /// <summary>
        /// intからRoleを取得します
        /// </summary>
        /// <param name="n"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static Role FromInt32(int n)
        {
            if (!Enum.IsDefined(typeof(RoleEnum), n))
                throw new ArgumentOutOfRangeException(nameof(n), $"指定された値を{nameof(Role)}に変換できません。");
            return new Role((RoleEnum)n);    
        }
        /// <summary>
        /// すべての役の中からランダムの役を返します
        /// </summary>
        /// <returns></returns>
        public static Role FromRandom()
        {
            return FromInt32(UnityEngine.Random.Range(0, TotalTypes));
        }

        #endregion

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
    }

    /// <summary>
    /// リールの位置
    /// </summary>
    public enum ReelPos
    {
        Left, Center, Right
    }

    /// <summary>
    /// スロットの出目
    /// </summary>
    public struct RoleSet
    {
        public RoleSet(Role left, Role center, Role right)
        {
            this.Left = left;
            this.Center = center;
            this.Right = right;
        }

        public Role Left { get; }
        public Role Center { get; }
        public Role Right { get; }

        /// <summary>
        /// ビンゴになっている役を取得する
        /// ビンゴになっていなければnullが返る
        /// </summary>
        public Role? BingoRole
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
            public ReachInfo(Role reachRole, ReelPos reachPos)
            {
                this.ReachedRole = reachRole;
                this.ReachReelPos = reachPos;
            }
            /// <summary>
            /// リーチになっている役柄
            /// </summary>
            public Role ReachedRole { get; }
            /// <summary>
            /// 揃っていないリールの位置
            /// </summary>
            public ReelPos ReachReelPos { get; }
        }
    }
}