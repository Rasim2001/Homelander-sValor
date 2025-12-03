using System;
using System.Collections.Generic;

namespace Infastructure.Data
{
    [Serializable]
    public class KillData
    {
        public List<string> ClearedEnemyCamps = new List<string>();
        public List<string> ClearedEnviromentResources { get; } = new List<string>();
    }
}