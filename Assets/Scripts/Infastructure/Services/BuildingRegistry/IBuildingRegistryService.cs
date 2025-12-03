using System;
using System.Collections.Generic;
using BuildProcessManagement;

namespace Infastructure.Services.BuildingRegistry
{
    public interface IBuildingRegistryService
    {
        void AddBuild(BuildInfo buildInfo);
        void RemoveBuild(BuildInfo buildInfo);
        List<BuildInfo> GetAllBuildInfos();
        event Action OnBuildAddHappened;
    }
}