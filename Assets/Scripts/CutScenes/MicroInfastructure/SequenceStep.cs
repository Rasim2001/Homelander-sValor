using System;

namespace CutScenes.MicroInfastructure
{
    public class SequenceStep
    {
        public Action Action { get; set; }
        public Action<Action> AsyncAction { get; set; }
        public bool IsParallel { get; set; }
        public float Delay { get; set; }
    }
}