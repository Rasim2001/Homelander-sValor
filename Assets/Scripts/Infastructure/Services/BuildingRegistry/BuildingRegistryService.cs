using System;
using System.Collections.Generic;
using BuildProcessManagement;

namespace Infastructure.Services.BuildingRegistry
{
    public class BuildingRegistryService : IBuildingRegistryService
    {
        private readonly List<BuildInfo> _allBuildInfos = new List<BuildInfo>();

        public event Action OnBuildAddHappened;

        public void AddBuild(BuildInfo buildInfo)
        {
            if (!_allBuildInfos.Contains(buildInfo))
            {
                _allBuildInfos.Add(buildInfo);

                OnBuildAddHappened?.Invoke();
            }
        }

        public void RemoveBuild(BuildInfo buildInfo)
        {
            if (_allBuildInfos.Contains(buildInfo))
                _allBuildInfos.Remove(buildInfo);
        }

        public List<BuildInfo> GetAllBuildInfos() =>
            _allBuildInfos;
    }
}