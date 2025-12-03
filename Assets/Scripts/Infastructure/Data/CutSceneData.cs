using System;

namespace Infastructure.Data
{
    [Serializable]
    public class CutSceneData
    {
        public bool Active;

        public CutSceneData(bool active) =>
            Active = active;
    }
}