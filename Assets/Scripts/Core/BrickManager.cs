using System;
using System.Collections.Generic;
using System.Linq;
using Prez.Utilities;
using UnityEngine;

namespace Prez.Core
{
    public class BrickManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _brickPool;
        [SerializeField] private ObjectPool _brickDestroyEffectsPool;
        [SerializeField] private Vector2 _brickSize;
        [SerializeField] private Vector2 _gridStart;
        [SerializeField] private Vector2 _gridEnd;
        [SerializeField] private float _randomSeed;

        private List<Brick> _bricks = new();
        private Vector2Int _gridSize;

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

        private void CalculateGridSize()
        {
            _gridSize.x = (int)((_gridEnd.x - _gridStart.x) / _brickSize.x);
            _gridSize.y = (int)((_gridStart.y - _gridEnd.y) / _brickSize.y);
        }

        private void FillGrid()
        {
            for (var x = 0; x <= _gridSize.x; x++)
            {
                for (var y = 0; y <= _gridSize.y; y++)
                {
                    var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
                    var gridPosition = new Vector2Int(x, y);
                    brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
                    brick.gameObject.SetActive(true);
                    _bricks.Add(brick);
                }
            }
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

        private void OnBrickDestroyed(Brick brick)
        {
            var effect = _brickDestroyEffectsPool.GetPooledObject().GetComponent<ParticleSystem>();
            effect.transform.position = brick.transform.position;
            var main = effect.main;
            main.startColor = brick.Color;
            effect.gameObject.SetActive(true);
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