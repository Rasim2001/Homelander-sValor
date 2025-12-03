using System.Collections.Generic;
using _Tutorial;

namespace Infastructure.Services.NearestBuildFind
{
    public interface INearestBuildFindService
    {
        TutorialHints GetNearestStone();
        TutorialHints GetNearestTree();
        TutorialHints GetNearestTower();
        List<TutorialHints> Stones { get; }
        List<TutorialHints> Trees { get; }
        List<TutorialHints> Towers { get; }
    }
}