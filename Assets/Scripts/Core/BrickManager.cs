using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Prez.Data;
using Prez.Enums;
using Prez.Utilities;
using UnityEngine;
using UnityEngine.UI;
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

        [Tab("Other")] [SerializeField]
        private Image _cooldownIndicatorImage;
        
        private GameData _data;
        private EventManager _event;
        private List<Brick> _bricks = new();
        private Vector2Int _gridSize;

        private void Awake()
        {
            _event = EventManager.I;
            _data = GameManager.I.Data;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
            _event.OnBrickDestroyed += OnBrickDestroyed;
        }

        private void OnDisable()
        {
            _event.OnBrickDestroyed -= OnBrickDestroyed;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveBricksDown();
                SpawnBrickRow(0);
            }
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.NewGame)
                SetupNewGame();
        }
        
        private void OnBrickDestroyed(Brick brick, long maxHealth)
        {
            DestroyBrick(brick);
        }

        /// <summary>
        /// Sets up a new game.
        /// </summary>
        private void SetupNewGame()
        {
            CalculateGridSize();
            SetRandomNoiseSeed();
            StartCoroutine(FillGrid());
            StartCoroutine(AutoSpawnBricks());
        }

        /// <summary>
        /// Sets a random noise seed
        /// if there is not a seed available.
        /// </summary>
        private void SetRandomNoiseSeed()
        {
            if (_data.BrickNoiseSeed != 0)
                return;

            _data.BrickNoiseSeed = Random.Range(1000, 999999);
        }
        
        /// <summary>
        /// Calculates the grid size
        /// based on start and end positions.
        /// </summary>
        private void CalculateGridSize()
        {
            _gridSize.x = (int)((_gridEnd.x - _gridStart.x) / _brickSize.x);
            _gridSize.y = (int)((_gridStart.y - _gridEnd.y) / _brickSize.y);
        }

        /// <summary>
        /// Fills the grid with bricks.
        /// </summary>
        private IEnumerator FillGrid()
        {
            for (var y = _gridSize.y; y >= 0; y--)
            {
                MoveBricksDown();
                yield return new WaitForSeconds(0.1f);
                SpawnBrickRow(0);
            }
        }

        /// <summary>
        /// Automatically spawns new bricks after some time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoSpawnBricks()
        {
            while (true)
            {
                _cooldownIndicatorImage.DOKill();
                _cooldownIndicatorImage.fillAmount = 1f;
                _cooldownIndicatorImage.DOFillAmount(0, _data.BrickRowSpawnCooldownBase)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(_data.BrickRowSpawnCooldownBase);
                
                MoveBricksDown();
                SpawnBrickRow(0);
            }
        }

        /// <summary>
        /// Spawns a row of bricks at given grid row.
        /// </summary>
        /// <param name="y"></param>
        private void SpawnBrickRow(int y)
        {
            for (var x = 0; x <= _gridSize.x; x++)
            {
                var noise = Mathf.PerlinNoise(x / (float)_gridSize.x * _data.BrickNoiseScale, 
                    _data.BrickNoiseSeed / (float)_gridSize.y + _data.BrickRowsSpawned * _data.BrickNoiseScale);

                if (noise >= _data.BrickNoiseThresholdBase)
                    continue;
                
                SpawnBrick(x, y);
            }
            
            _data.BrickRowsSpawned++;
        }

        /// <summary>
        /// Spawns a brick at given grid position.
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        private void SpawnBrick(int gridX, int gridY)
        {
            if (_bricks.Any(b => b.GridPosition == new Vector2Int(gridX, gridY)))
                return;
            
            var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
            var gridPosition = new Vector2Int(gridX, gridY);
            brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
            brick.SetMaxHealth(new NumberData((long)Mathf.Max(1f, _data.BrickRowsSpawned / _data.BrickHealthIncreaseRate)));
            _bricks.Add(brick);
            brick.gameObject.SetActive(true);
        }

        /// <summary>
        /// Moves all bricks down.
        /// Does not go past the grid row limits.
        /// </summary>
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

        /// <summary>
        /// Destroys the given brick.
        /// </summary>
        /// <param name="brick"></param>
        private void DestroyBrick(Brick brick)
        {
            SpawnBrickDestroyedEffect(brick);
            
            if (_bricks.Contains(brick))
                _bricks.Remove(brick);
            
            _brickPool.ReleasePooledObject(brick.gameObject);
        }

        /// <summary>
        /// Spawns a brick destroy effect.
        /// </summary>
        /// <param name="brick"></param>
        private void SpawnBrickDestroyedEffect(Brick brick)
        {
            var effect = _brickDestroyEffectsPool.GetPooledObject().GetComponent<ParticleSystem>();
            effect.transform.position = brick.transform.position;
            var main = effect.main;
            main.startColor = brick.Color;
            effect.gameObject.SetActive(true);
        }

        /// <summary>
        /// Calculates the world position of provided grid position.
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        private Vector2 GridToWorldPosition(Vector2Int gridPosition)
        {
            return new Vector2(_gridStart.x + gridPosition.x * _brickSize.x,
                _gridStart.y - gridPosition.y * _brickSize.y);
        }
    }
}