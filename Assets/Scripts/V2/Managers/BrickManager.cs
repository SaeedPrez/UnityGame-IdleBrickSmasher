using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prez.V2.Data;
using Prez.V2.Enums;
using Prez.V2.Obstacles;
using Prez.V2.Utilities;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class BrickManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _brickPool;
        [SerializeField] private BoxCollider2D _bricksContainer;
        [SerializeField] private Vector2 _brickSize;

        private EventManager _event;
        private GameManager _game;
        private readonly List<Brick> _bricks = new();
        private Vector2Int _gridSize;
        private Vector2 _gridStart;
        private Vector2 _gridEnd;
        private Coroutine _autoSpawnCoroutine;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
            _game = RefManager.Game;
        }

        private void OnEnable()
        {
            _event.OnGameStateChanged += OnGameStateChanged;
            _event.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            _event.OnGameStateChanged -= OnGameStateChanged;
            _event.OnBrickDamaged -= OnBrickDamaged;
        }

        private void Update()
        {
#if UNITY_EDITOR
            // TODO: Remove
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveBricksDown();
                SpawnBrickRow();
            }
#endif            
        }
        #endregion

        #region Observers

        private void OnGameStateChanged(EGameState newState, EGameState oldState)
        {
            if (newState is EGameState.BricksLoading)
                StartCoroutine(SetupGrid());
        }

        private void OnBrickDamaged(DamageData data)
        {
            if (data.BrickDestroyed && _bricks.Contains(data.Brick))
                _bricks.Remove(data.Brick);
        }

        #endregion
        
        #region Grid

        /// <summary>
        /// Sets up the grid with bricks.
        /// </summary>
        private IEnumerator SetupGrid()
        {
            CalculateGridSize();
            SetRandomNoiseSeed();
            yield return StartCoroutine(SetupBricks());
            _autoSpawnCoroutine = StartCoroutine(AutoSpawnBricks());

            yield return null;
        }

        /// <summary>
        /// Calculates the grid size
        /// based on start and end positions.
        /// </summary>
        private void CalculateGridSize()
        {
            _gridStart.x = _bricksContainer.bounds.center.x - _bricksContainer.bounds.extents.x;
            _gridEnd.x = _bricksContainer.bounds.center.x + _bricksContainer.bounds.extents.x;
            _gridStart.y = _bricksContainer.bounds.center.y + _bricksContainer.bounds.extents.y;
            _gridEnd.y = _bricksContainer.bounds.center.y - _bricksContainer.bounds.extents.y;
            
            _gridSize.x = (int)((_gridEnd.x - _gridStart.x) / _brickSize.x) + 1;
            _gridSize.y = (int)((_gridStart.y - _gridEnd.y) / _brickSize.y);
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
        
        #endregion

        #region Noise

        /// <summary>
        /// Sets a random noise seed
        /// if there is not a seed available.
        /// </summary>
        private void SetRandomNoiseSeed()
        {
            if (_game.Data.BricksData.NoiseSeed != 0)
                return;

            _game.Data.BricksData.NoiseSeed = Random.Range(1000, 999999);
        }

        #endregion

        #region Bricks

        private IEnumerator SetupBricks()
        {
            for (var y = _gridSize.y; y >= 0; y--)
            {
                MoveBricksDown();
                yield return new WaitForSeconds(0.1f);
                SpawnBrickRow();
            }
            
            _event.TriggerBricksLoaded();
        }

        /// <summary>
        /// Spawns a row of bricks at given grid row.
        /// </summary>
        private void SpawnBrickRow()
        {
            if (_bricks.Any(b => b.GridPosition.y == 0))
                return;

            var bricksSpawned = false;

            var y = ++_game.Data.BricksData.NoiseOffsetY;

            for (var x = 0; x < _gridSize.x; x++)
            {
                if (ShouldSpawnBrick(x, y))
                {
                    SpawnBrick(x);
                    bricksSpawned = true;
                }
            }

            if (bricksSpawned)
                _game.Data.BricksData.RowsSpawned++;
        }
        
        /// <summary>
        /// Spawns a brick at given grid position.
        /// </summary>
        /// <param name="gridX"></param>
        private void SpawnBrick(int gridX)
        {
            if (_bricks.Any(b => b.GridPosition == new Vector2Int(gridX, 0)))
                return;

            var brick = _brickPool.GetPooledObject().GetComponent<Brick>();
            var gridPosition = new Vector2Int(gridX, 0);
            
            brick.SetPosition(gridPosition, GridToWorldPosition(gridPosition));
            brick.SetMaxHealth(_game.Data.BricksData.GetMaxHealth());
            brick.SetRowId(_game.Data.BricksData.RowsSpawned);
            brick.gameObject.SetActive(true);
            _bricks.Add(brick);
        }
        
        /// <summary>
        /// Checks if the noise value is over the threshold.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool ShouldSpawnBrick(int x, long y)
        {
            var xCoords = x / (float)_gridSize.x * _game.Data.BricksData.NoiseScale + _game.Data.BricksData.NoiseSeed;
            var yCoords = y / (float)_gridSize.y * _game.Data.BricksData.NoiseScale + _game.Data.BricksData.NoiseSeed;
            var noise = Mathf.PerlinNoise(xCoords, yCoords);
            
            return !(noise >= _game.Data.BricksData.GetNoiseSpawnThreshold());
        }
        
        /// <summary>
        /// Moves all bricks down.
        /// Does not go past the grid row limits.
        /// </summary>
        private void MoveBricksDown()
        {
            for (var y = _gridSize.y - 1; y >= 0; y--)
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
        ///     Automatically spawns new bricks after some time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoSpawnBricks()
        {
            while (true)
            {
                var cooldown = _game.Data.BricksData.GetSpawnCooldown();

                yield return new WaitForSeconds(cooldown + 0.01f);

                MoveBricksDown();
                SpawnBrickRow();
            }
        }
        
        #endregion
    }
}