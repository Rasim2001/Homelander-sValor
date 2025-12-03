using System;

namespace CutScenes.MicroInfastructure
{
    public static class CustomSequenceExtensions
    {
        public static TimelineSequence Add(this TimelineSequence sequence, Action action) =>
            sequence.Add(new SequenceStep { Action = action, IsParallel = false, Delay = 0 });

        public static TimelineSequence Add(this TimelineSequence sequence, Action<Action> asyncAction) =>
            sequence.Add(new SequenceStep { AsyncAction = asyncAction, IsParallel = false, Delay = 0 });

        public static TimelineSequence Join(this TimelineSequence sequence, Action action) =>
            sequence.Add(new SequenceStep { Action = action, IsParallel = true, Delay = 0 });

        public static TimelineSequence Join(this TimelineSequence sequence, Action<Action> asyncAction) =>
            sequence.Add(new SequenceStep { AsyncAction = asyncAction, IsParallel = true, Delay = 0 });

        public static TimelineSequence WithDelay(this TimelineSequence sequence, float delay) =>
            sequence.WithDelay(delay);
    }
}