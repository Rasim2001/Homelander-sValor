using System.Collections.Generic;
using _Tutorial;
using Infastructure.Services.Flag;
using UnityEngine;

namespace Infastructure.Services.NearestBuildFind
{
    public class NearestBuildFindService : INearestBuildFindService
    {
        public List<TutorialHints> Stones { get; }
        public List<TutorialHints> Trees { get; set; }
        public List<TutorialHints> Towers { get; set; }

        private readonly IFlagTrackerService _flagTrackerService;

        public NearestBuildFindService(IFlagTrackerService flagTrackerService)
        {
            _flagTrackerService = flagTrackerService;

            Stones = new List<TutorialHints>();
            Trees = new List<TutorialHints>();
            Towers = new List<TutorialHints>();
        }

        public TutorialHints GetNearestStone()
        {
            if (Stones.Count <= 1)
                return null;

            Stones.Sort((stone1, stone2) =>
            {
                float distanceToPlayerX1 =
                    Mathf.Abs(stone1.transform.position.x - _flagTrackerService.GetMainFlag().position.x);
                float distanceToPlayerX2 =
                    Mathf.Abs(stone2.transform.position.x - _flagTrackerService.GetMainFlag().position.x);

                return distanceToPlayerX1.CompareTo(distanceToPlayerX2);
            });

            return Stones[0];
        }

        public TutorialHints GetNearestTree()
        {
            if (Trees.Count <= 1)
                return null;

            Trees.Sort((tree1, tree2) =>
            {
                float distanceToPlayerX1 =
                    Mathf.Abs(tree1.transform.position.x - _flagTrackerService.GetMainFlag().position.x);
                float distanceToPlayerX2 =
                    Mathf.Abs(tree2.transform.position.x - _flagTrackerService.GetMainFlag().position.x);

                return distanceToPlayerX1.CompareTo(distanceToPlayerX2);
            });

            return Trees[0];
        }

        public TutorialHints GetNearestTower()
        {
            if (Towers.Count <= 1)
                return null;

            Towers.Sort((tower1, tower2) =>
            {
                float distanceToPlayerX1 =
                    Mathf.Abs(tower1.transform.position.x - _flagTrackerService.GetMainFlag().position.x);
                float distanceToPlayerX2 =
                    Mathf.Abs(tower2.transform.position.x - _flagTrackerService.GetMainFlag().position.x);

                return distanceToPlayerX1.CompareTo(distanceToPlayerX2);
            });

            return Towers[0];
        }
    }
}