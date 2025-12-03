using System.Collections;
using System.Collections.Generic;
using Infastructure;
using UnityEngine;

namespace CutScenes.MicroInfastructure
{
    public class TimelineSequence
    {
        private readonly List<SequenceStep> _steps = new List<SequenceStep>();
        private readonly ICoroutineRunner _coroutineRunner;
        private Coroutine _coroutine;

        public TimelineSequence(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public TimelineSequence Add(SequenceStep sequenceStep)
        {
            _steps.Add(sequenceStep);

            return this;
        }

        public TimelineSequence WithDelay(float delay)
        {
            if (_steps.Count > 0)
                _steps[^1].Delay = delay;

            return this;
        }

        public void Execute() =>
            _coroutine = _coroutineRunner.StartCoroutine(RunSequence());

        private IEnumerator RunSequence()
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                SequenceStep step = _steps[i];

                if (step.IsParallel)
                    continue;

                List<SequenceStep> parallelSteps = GetFollowingParallelSteps(i);

                foreach (SequenceStep pStep in parallelSteps)
                {
                    _coroutineRunner.StartCoroutine(pStep.AsyncAction != null
                        ? RunAsyncParallelStep(pStep)
                        : RunParallelStepWithDelay(pStep));
                }

                if (step.Delay > 0)
                    yield return new WaitForSeconds(step.Delay);

                if (step.AsyncAction != null)
                {
                    bool isCompleted = false;
                    step.AsyncAction(() => isCompleted = true);
                    while (!isCompleted)
                        yield return null;
                }
                else
                    step.Action?.Invoke();
            }
        }

        private List<SequenceStep> GetFollowingParallelSteps(int currentIndex)
        {
            List<SequenceStep> result = new();

            int nextIndex = currentIndex + 1;
            while (nextIndex < _steps.Count && _steps[nextIndex].IsParallel)
            {
                result.Add(_steps[nextIndex]);
                nextIndex++;
            }

            return result;
        }

        private IEnumerator RunParallelStepWithDelay(SequenceStep step)
        {
            if (step.Delay > 0)
                yield return new WaitForSeconds(step.Delay);

            step.Action?.Invoke();
        }

        private IEnumerator RunAsyncParallelStep(SequenceStep step)
        {
            if (step.Delay > 0)
                yield return new WaitForSeconds(step.Delay);

            bool isCompleted = false;
            step.AsyncAction(() => isCompleted = true);
            while (!isCompleted)
                yield return null;
        }
    }
}