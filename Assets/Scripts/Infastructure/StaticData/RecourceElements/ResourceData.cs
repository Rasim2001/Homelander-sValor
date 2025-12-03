using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.StaticData.RecourceElements
{
    [Serializable]
    public class ResourceData
    {
        public List<GameObject> ResoucePrefabs;
        public int CoinsValue;
        public int GridSizeX;
        public int DestructionProgress;
    }
}