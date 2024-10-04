using System.Collections.Generic;
using System.Linq;
using Prez.Utilities;
using UnityEngine;
using VInspector;

namespace Prez.Core
{
    public class BrickManager : MonoBehaviour
    {
        [Tab("Pools")]
        [SerializeField] private ObjectPool _brickPool;
        [SerializeField] private ObjectPool _brickDestroyEffectsPool;
        
        [Tab("Grid")]
        [SerializeField] private Vector2 _brickSize;
        [SerializeField] private Vector2 _gridStart;
        [SerializeField] private Vector2 _gridEnd;

        [Tab("Noise")]
        [SerializeField] private int _noiseSeed;
        [SerializeField] private float _noiseScale;
        [SerializeField, Range(0.1f, 0.9f)] private float _noiseThreshold;

        private List<Brick> _bricks = new();
        private Vector2Int _gridSize;
        private int _rowsSpawned;

        private void OnEnable()
        {
            EventManager.I.OnBrickDestroyed += OnBrickDestroyed;
            CalculateGridSize();
        }

        private void OnDisable()
        {
            EventManager.I.OnBrickDestroyed -= OnBrickDestroyed;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                MoveBricksDown();
        }

        private void Start()
        {
            FillGrid();
        }

        private void OnBrickDestroyed(Brick brick)
        {
            SpawnBrickDestroyedEffect(brick);
            DespawnBrick(brick);
        }
        
        private void CalculateGridSize()
        {
            _gridSize.x = (int)((_gridEnd.x - _gridStart.x) / _brickSize.x);
            _gridSize.y = (int)((_gridStart.y - _gridEnd.y) / _brickSize.y);
        }

        private void FillGrid()
        {
            for (var y = 0; y <= _gridSize.y; y++)
            {
                SpawnBrickRow(y);
            }
        }

        private void SpawnBrickRow(int y)
        {
            for (var x = 0; x <= _gridSize.x; x++)
            {
                var noise = Mathf.PerlinNoise(x / (float)_gridSize.x * _noiseScale, _noiseSeed / (float)_gridSize.y + y * _noiseScale);

                if (noise < _noiseThreshold)
                    continue;
                
                SpawnBrick(x, y);
            }

            _rowsSpawned++;
        }

        private void SpawnBrick(int gridX, int gridY)
        {
            var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
            var gridPosition = new Vector2Int(gridX, gridY);
            brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
            brick.gameObject.SetActive(true);
            _bricks.Add(brick);
        }

        private void MoveBricksDown()
        {
            for (var y = _gridSize.y - 1; y >= 0 ; y--)
            {
                for (var x = _gridSize.x; x >= 0; x--)
                {
                    var brick = _bricks.FirstOrDefault(b => b.GridPosition == new Vector2Int(x, y));
                    if (!brick) continue;
                    
                    var gridPosition = brick.GridPosition + Vector2Int.up;

                    if (_bricks.Any(b => b.GridPosition == gridPosition))
                        continue;
                    
                    brick.MoveDown(GridToWorldPosition(gridPosition));
                }
            }
        }

        private void SpawnBrickDestroyedEffect(Brick brick)
        {
            var effect = _brickDestroyEffectsPool.GetPooledObject().GetComponent<ParticleSystem>();
            effect.transform.position = brick.transform.position;
            var main = effect.main;
            main.startColor = brick.Color;
            effect.gameObject.SetActive(true);
        }

        private void DespawnBrick(Brick brick)
        {
            _brickPool.ReleasePooledObject(brick.gameObject);
            _bricks.Remove(brick);
        }

        private Vector2 GridToWorldPosition(Vector2Int gridPosition)
        {
            return new Vector2(_gridStart.x + gridPosition.x * _brickSize.x,
                _gridStart.y - gridPosition.y * _brickSize.y);
        }
    }
}