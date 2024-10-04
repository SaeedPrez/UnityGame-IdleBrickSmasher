using System.Collections.Generic;
using System.Linq;
using Prez.Data;
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

        private GameData _data;
        private List<Brick> _bricks = new();
        private Vector2Int _gridSize;

        private void OnEnable()
        {
            _data = GameManager.I.Data;
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
            {
                MoveBricksDown();
                SpawnBrickRow(0);
            }
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
            for (var y = _gridSize.y; y >= 0; y--)
            {
                SpawnBrickRow(y);
            }
        }

        private void SpawnBrickRow(int y)
        {
            for (var x = 0; x <= _gridSize.x; x++)
            {
                var noise = Mathf.PerlinNoise(x / (float)_gridSize.x * _data.BrickNoiseScale, 
                    _data.BrickNoiseSeed / (float)_gridSize.y + _data.BrickRowsSpawned * _data.BrickNoiseScale);

                if (noise < _data.BrickNoiseThreshold)
                    continue;
                
                SpawnBrick(x, y);
            }
            
            _data.BrickRowsSpawned++;
        }

        private void SpawnBrick(int gridX, int gridY)
        {
            if (_bricks.Any(b => b.GridPosition == new Vector2Int(gridX, gridY)))
                return;
            
            var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
            var gridPosition = new Vector2Int(gridX, gridY);
            brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
            brick.SetMaxHealth((int)Mathf.Max(1f, _data.BrickRowsSpawned / _data.BrickHealthIncreaseRate));
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
                    
                    if (!brick || !brick.IsActive) 
                        continue;
                    
                    var gridPosition = brick.GridPosition + Vector2Int.up;
                    var brickBelow = _bricks.FirstOrDefault(b => b.GridPosition == gridPosition);
                    
                    if (brickBelow && brickBelow.IsActive)
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