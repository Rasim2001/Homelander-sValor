using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.Forest
{
    public class ForestGrokGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject[] treePrefabs;
        [SerializeField] private float minDistanceBetweenTrees = 1f;
        [SerializeField] private int treeCount = 30;

        private BoxCollider2D _generationArea;
        private List<GameObject> _spawnedTrees;

        void Start()
        {
            _generationArea = GetComponentInParent<BoxCollider2D>();
            _spawnedTrees = new List<GameObject>();

            if (_generationArea == null)
                return;

            GenerateForest();
        }

        private void GenerateForest()
        {
            int treesPlaced = 0;
            int attempts = 0;
            int maxAttempts = treeCount * 2;

            Vector2 areaSize = _generationArea.size;
            Vector2 areaCenter = (Vector2)transform.position + _generationArea.offset;

            while (treesPlaced < treeCount && attempts < maxAttempts)
            {
                float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
                float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);
                Vector2 position = new Vector2(x, y);

                if (IsPositionValid(position))
                {
                    GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                    GameObject newTree = Instantiate(treePrefab, position, Quaternion.identity, transform);
                    _spawnedTrees.Add(newTree);
                    treesPlaced++;

                    if (Random.value < 0.3f)
                        SpawnCluster(position);
                }

                attempts++;
            }
        }

        private void SpawnCluster(Vector2 center)
        {
            int clusterSize = Random.Range(1, 4);
            for (int i = 0; i < clusterSize; i++)
            {
                float offsetX = Random.Range(0.1f, 0.5f);
                Vector2 offset = Random.insideUnitCircle * offsetX;
                Vector2 newPos = center + offset;

                if (IsPositionValid(newPos))
                {
                    GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                    GameObject newTree = Instantiate(treePrefab, newPos, Quaternion.identity, transform);
                    
                    _spawnedTrees.Add(newTree);
                }
            }
        }

        private bool IsPositionValid(Vector2 position)
        {
            foreach (GameObject tree in _spawnedTrees)
            {
                Vector2 treePos = tree.transform.position;

                float distance = Mathf.Abs(position.x - treePos.x);

                if (distance < minDistanceBetweenTrees)
                    return false;
            }

            return true;
        }
    }
}