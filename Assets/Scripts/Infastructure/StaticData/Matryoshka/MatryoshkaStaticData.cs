using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infastructure.StaticData.Matryoshka
{
    [CreateAssetMenu(fileName = "MatryoshkaData", menuName = "StaticData/Matryoshka")]
    public class MatryoshkaStaticData : SerializedScriptableObject
    {
        public Dictionary<EnemyTypeId, MatryoshkaConfig> MatryoshkaConfigsDictionary;
    }

    [Serializable]
    public class MatryoshkaConfig
    {
        public List<MatryoshkaData> MatryoshkaDatas;
    }

    [Serializable]
    public class MatryoshkaData
    {
        public MatryoshkaId MatryoshkaId;
        public int Hp;
        public Vector3 Scale;
    }
}