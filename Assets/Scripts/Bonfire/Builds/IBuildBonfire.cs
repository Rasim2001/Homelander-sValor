using System;
using UnityEngine;

namespace Bonfire.Builds
{
    public interface IBuildBonfire
    {
        void Build(BonfireInfo bonfireInfo, GameObject previousBuild, Action OnCompleted);
    }
}