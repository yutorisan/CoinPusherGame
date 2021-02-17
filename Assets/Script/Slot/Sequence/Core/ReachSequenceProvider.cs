using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedalPusher.Slot.Sequences;
using System;
using UnityUtility;

namespace MedalPusher.Slot.Sequences
{
    public partial class ReelSequenceProvider
    {
        private interface IReachAntagonistSequenceProvider
        {
            ReelSequence GetAntagonistSequence(AntagonistType antagonist, bool isNextWin);
        }
        private class ReachAntagonistSequenceProvider : IReachAntagonistSequenceProvider
        {
            private readonly ReelSequenceProvider m_parent;
            private readonly IReadOnlyDictionary<AntagonistType, Func<bool, ReelSequence>> m_createMethodTable;
            public ReachAntagonistSequenceProvider(ReelSequenceProvider parent)
            {
                m_parent = parent;
                m_createMethodTable = new Dictionary<AntagonistType, Func<bool, ReelSequence>>()
                {
                    {AntagonistType.Type1, CreateType1 }
                };
            }

            public ReelSequence GetAntagonistSequence(AntagonistType antagonist, bool isNextWin)
            {
                return m_createMethodTable[antagonist](isNextWin);
            }

            private ReelSequence CreateType1(bool isNextWin)
            {
                //振幅
                var amplitude = m_parent.RoleIntervalAngle;

                var antagonistSq =
                ReelSequence.Empty
                            .Append(m_parent.RelativeRollSequence(amplitude * .5f, .3f)) //.5
                            .Append(m_parent.RelativeRollSequence(amplitude * -.5f, .3f)) // 0
                            .Append(m_parent.RelativeRollSequence(amplitude * .7f, .3f)) //.7
                            .Append(m_parent.RelativeRollSequence(amplitude * -.5f, .3f)) //.2
                            .Append(m_parent.RelativeRollSequence(amplitude * .6f, .3f)) //.8
                            .Append(m_parent.RelativeRollSequence(amplitude * -.4f, .3f)) //.4
                            .Append(m_parent.RelativeRollSequence(amplitude * .5f, .3f)) //.9
                            .AppendInterval(1f);


                if (isNextWin)
                {
                    antagonistSq.Append(m_parent.RelativeRollSequence(amplitude * .1f, .1f));
                }
                else
                {
                    antagonistSq.Append(m_parent.RelativeRollSequence(amplitude * -.9f, .4f));
                }

                return antagonistSq;
            }
        }


    }

    public enum AntagonistType
    {
        Type1
    }
}