using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bonfire
{
    public interface IUpdateBonfireLevelAnimation
    {
        void PlayAnimation(List<Transform> Elements, float duration, Ease ease);
    }
}