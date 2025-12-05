using System;
using System.Collections;
using System.Collections.Generic;
using Infastructure;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Bonfire.Builds
{
    public class BuildBonfire : IBuildBonfire
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private Coroutine _buildCoroutine;
        private Coroutine _finishBuildCoroutine;

        private Action _onCompleted;

        public BuildBonfire(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void Build(BonfireInfo bonfireInfo, GameObject previousBuild, Action OnCompleted)
        {
            _onCompleted = OnCompleted;

            RegisterBuild(bonfireInfo);
            StartBuild(bonfireInfo, previousBuild);
        }


        private void RegisterBuild(BonfireInfo bonfireInfo)
        {
            SpriteRenderer spriteRenderer = bonfireInfo.NextBuild.GetComponent<SpriteRenderer>();

            ScaffoldBuildBonfire scaffoldsBuild = bonfireInfo.GetComponentInChildren<ScaffoldBuildBonfire>();
            scaffoldsBuild.Initialize(spriteRenderer);
            scaffoldsBuild.BuildScaffolds(bonfireInfo);

            WoodBuildBonfire woodBuild = bonfireInfo.GetComponentInChildren<WoodBuildBonfire>();
            woodBuild.Initialize(spriteRenderer);

            bonfireInfo.CurrentWoodsCount = bonfireInfo.WoodsList.Count;

            BuildingProgressBonfire buildingProgress = bonfireInfo.GetComponent<BuildingProgressBonfire>();
            BuildingProgressBonfire nextBuildingProgress =
                bonfireInfo.NextBuild.GetComponent<BuildingProgressBonfire>();

            buildingProgress.ProgressValue = nextBuildingProgress.ProgressValue;
        }

        private void StartBuild(BonfireInfo bonfireInfo, GameObject previousBuild)
        {
            BuildingProgressBonfire progressBonfire = bonfireInfo.GetComponent<BuildingProgressBonfire>();

            StopCoroutine(ref _buildCoroutine);
            _buildCoroutine =
                _coroutineRunner.StartCoroutine(StartBuildCoroutine(progressBonfire, bonfireInfo, previousBuild));
        }

        private IEnumerator StartBuildCoroutine(
            BuildingProgressBonfire progressBonfire,
            BonfireInfo bonfireInfo,
            GameObject previousBuild)
        {
            while (bonfireInfo.CurrentWoodsCount > 0)
            {
                progressBonfire.BuildWoods();
                
                yield return new WaitForSeconds(0.25f);
            }

            DestroyPreviousBuild(previousBuild);
            ShowNextBuild(bonfireInfo);
            StartAnimationCoroutine(bonfireInfo);
        }


        private void StartAnimationCoroutine(BonfireInfo bonfireInfo)
        {
            StopCoroutine(ref _finishBuildCoroutine);
            _finishBuildCoroutine = _coroutineRunner.StartCoroutine(FinishBuildAnimationCoroutine(bonfireInfo));
        }

        private IEnumerator FinishBuildAnimationCoroutine(BonfireInfo bonfireInfo)
        {
            float centerOfWoodsX = bonfireInfo.GetComponentInChildren<WoodBuildBonfire>().transform.localPosition.x;
            float centerOfScaffoldsX =
                bonfireInfo.GetComponentInChildren<ScaffoldBuildBonfire>().transform.localPosition.x;

            List<GameObject> woodsList = new List<GameObject>(bonfireInfo.WoodsList);
            List<GameObject> scaffoldsList = new List<GameObject>(bonfireInfo.ScaffoldsList);

            yield return AnimateBuildObjects(woodsList, centerOfWoodsX);
            yield return AnimateBuildObjects(scaffoldsList, centerOfScaffoldsX);

            DeleteMarkingBuild(bonfireInfo);

            _onCompleted?.Invoke();
        }

        private void DeleteMarkingBuild(BonfireInfo bonfireInfo)
        {
            BuildingProgressBonfire progressBonfire = bonfireInfo.GetComponent<BuildingProgressBonfire>();

            List<GameObject> currentWoodsList = new List<GameObject>(bonfireInfo.WoodsList);
            List<GameObject> currentScaffoldsList = new List<GameObject>(bonfireInfo.ScaffoldsList);

            for (int i = 0; i < currentWoodsList.Count; i++)
                Object.Destroy(currentWoodsList[i].gameObject); //TODO:

            for (int i = 0; i < currentScaffoldsList.Count; i++)
                Object.Destroy(currentScaffoldsList[i].gameObject);

            bonfireInfo.WoodsList.Clear();
            bonfireInfo.ScaffoldsList.Clear();
            progressBonfire.Clear();

            bonfireInfo.CurrentWoodsCount = 0;
        }

        private IEnumerator AnimateBuildObjects(IEnumerable<GameObject> objects, float centerX)
        {
            foreach (GameObject obj in objects)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    AddRandomMovement(rb, centerX);
            }

            yield return new WaitForSeconds(0.75f);
        }

        private void AddRandomMovement(Rigidbody2D woodRb, float centerOfBuildX)
        {
            float deltaX = woodRb.transform.localPosition.x - centerOfBuildX;
            int randomHeight = Random.Range(3, 7);
            int randomX = Random.Range(-2, 2);

            woodRb.gravityScale = 1.5f;
            woodRb.AddRelativeForce(new Vector2(deltaX + randomX, randomHeight), ForceMode2D.Impulse);
        }

        private void StopCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                _coroutineRunner.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private void ShowNextBuild(BonfireInfo bonfireInfo)
        {
            bonfireInfo.NextBuild.SetActive(true);
        }

        private void DestroyPreviousBuild(GameObject previousBuild)
        {
            if (previousBuild != null)
                Object.Destroy(previousBuild);
        }
    }
}