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
        [SerializeField] private ObjectPool _brickHitEffectsPool;
        
        [Tab("Grid")]
        [SerializeField] private Vector2 _brickSize;
        [SerializeField] private Vector2 _gridStart;
        [SerializeField] private Vector2 _gridEnd;

        [Tab("Other")] 
        [SerializeField] private Image _cooldownIndicatorImage;
        
        [Tab("Debug")]
        [SerializeField] private Image _debugImage;
        
        private readonly List<Brick> _bricks = new();
        private Vector2Int _gridSize;
        private Coroutine _autoSpawnCoroutine;

        private void OnEnable()
        {
            EventManager.I.OnGameStateChanged += OnGameStateChanged;
            EventManager.I.OnBallCollidedWithBrick += OnBallCollidedWithBrick;
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
        }
        
        private void OnDisable()
        {
            EventManager.I.OnGameStateChanged -= OnGameStateChanged;
            EventManager.I.OnBallCollidedWithBrick -= OnBallCollidedWithBrick;
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveBricksDown();
                SpawnBrickRow();
            }
        }

        private void OnGameStateChanged(EGameState state)
        {
            if (state is EGameState.Loaded)
                StartCoroutine(SetupNewGame());
        }

        private void OnBallCollidedWithBrick(Ball ball, Brick brick, Vector2 point)
        {
            SpawnBrickHitEffect(brick, point);
        }
        
        private void OnBrickDamaged(DamageData data)
        {
            if (!data.BrickDestroyed)
                return;
            
            DestroyBrick(data.Brick);
            StartCoroutine(SpawnBricksAtThreshold());
        }

        /// <summary>
        /// Sets up a new game.
        /// </summary>
        private IEnumerator SetupNewGame()
        {
            CalculateGridSize();
            SetRandomNoiseSeed();
            yield return StartCoroutine(FillGrid());
            _autoSpawnCoroutine = StartCoroutine(AutoSpawnBricks());
        }

        /// <summary>
        /// Sets a random noise seed
        /// if there is not a seed available.
        /// </summary>
        private void SetRandomNoiseSeed()
        {
            if (GameManager.Data.BrickNoiseSeed != 0)
                return;

            GameManager.Data.BrickNoiseSeed = Random.Range(1000, 999999);
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
                SpawnBrickRow();
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
                var cooldown = GameManager.Data.GetBrickSpawnCooldown();
                
                _cooldownIndicatorImage.DOKill();
                _cooldownIndicatorImage.fillAmount = 1f;
                _cooldownIndicatorImage.DOFillAmount(0, cooldown)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(cooldown + 0.01f);
                
                MoveBricksDown();
                SpawnBrickRow();
            }
        }

        /// <summary>
        /// Spawn bricks if number of bricks
        /// are lower than the threshold.
        /// </summary>
        private IEnumerator SpawnBricksAtThreshold()
        {
            while (_bricks.Count < GameManager.Data.GetBrickMinimumSpawnThreshold())
            {
                if (_bricks.Any(b => b.GridPosition.y == 0))
                    MoveBricksDown();
    
                SpawnBrickRow();

                StopCoroutine(_autoSpawnCoroutine);
                _autoSpawnCoroutine = StartCoroutine(AutoSpawnBricks());
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        /// <summary>
        /// Spawns a row of bricks at given grid row.
        /// </summary>
        private void SpawnBrickRow()
        {
            if (_bricks.Any(b => b.GridPosition.y == 0))
                return;

            var bricksSpawned = false;
            
            var y = (int)GameManager.Data.BrickNoiseOffsetY++;
            
            for (var x = 0; x <= _gridSize.x; x++)
            {
                if (ShouldSpawnBrick(x, y))
                {
                    SpawnBrick(x);
                    bricksSpawned = true;
                }
            }
            
            if (bricksSpawned)
                GameManager.Data.BrickRowLevel++;
        }

        /// <summary>
        /// Spawns a brick at given grid position.
        /// </summary>
        /// <param name="gridX"></param>
        private void SpawnBrick(int gridX)
        {
            var gridY = 0;
            
            if (_bricks.Any(b => b.GridPosition == new Vector2Int(gridX, gridY)))
                return;
            
            var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
            var gridPosition = new Vector2Int(gridX, gridY);
            brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
            brick.SetMaxHealth(GameManager.Data.GetBrickMaxHealth());
            brick.SetSpawnedRowNumber(GameManager.Data.BrickRowLevel);
            _bricks.Add(brick);
            brick.gameObject.SetActive(true);
        }

        /// <summary>
        /// Checks if the noise value is over the threshold.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool ShouldSpawnBrick(int x, int y)
        {
            var xCoords = x / (float)_gridSize.x * GameManager.Data.BrickNoiseScale + GameManager.Data.BrickNoiseSeed;
            var yCoords = y / (float)_gridSize.y * GameManager.Data.BrickNoiseScale + GameManager.Data.BrickNoiseSeed;

            var noise = Mathf.PerlinNoise(xCoords, yCoords);
            
            // var noise = Mathf.PerlinNoise(x / (float)_gridSize.x * GameManager.Data.BrickNoiseScale, 
            //     y / GameManager.Data.BrickRowsSpawned * GameManager.Data.BrickNoiseScale);

            return !(noise >= GameManager.Data.GetBrickNoiseSpawnThreshold());
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
        /// Spawns a brick hit effect.
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="point"></param>
        private void SpawnBrickHitEffect(Brick brick, Vector2 point)
        {
            var effect = _brickHitEffectsPool.GetPooledObject().GetComponent<ParticleSystem>();
            effect.transform.position = point;
            var main = effect.main;
            main.startColor = brick.FillColor;
            effect.gameObject.SetActive(true);
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
            main.startColor = brick.FillColor;
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

        /// <summary>
        /// Debug mode
        /// </summary>
        [Tab("Debug")]
        [Button("Toggle noise image")]
        private void DebugNoise()
        {
#if UNITY_EDITOR
            if (_debugImage.gameObject.activeInHierarchy)
            {
                _debugImage.gameObject.SetActive(false);
                return;
            }
            else
            {
                _debugImage.gameObject.SetActive(true);
            
                var texture = new Texture2D(_gridSize.x, _gridSize.y);

                for (var y = 0; y < _gridSize.y; y++)
                {
                    for (var x = 0; x < _gridSize.x; x++)
                        texture.SetPixel(x, y, ShouldSpawnBrick(x, y) ? Color.white : Color.black);
                }

                texture.filterMode = FilterMode.Point;
                texture.Apply();
                _debugImage.material.mainTexture = texture;
            }
#else
            _debugImage.gameObject.SetActive(false);
#endif            
        }
    }
}