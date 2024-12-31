using System.Collections.Generic;
using Prez.V2.Obstacles;
using Prez.V2.Utilities;
using UnityEngine;
using VInspector;

namespace Prez.V2.Managers
{
    public class BrickManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _brickPool;
        [SerializeField] private BoxCollider2D _bricksContainer;
        [SerializeField] private Vector2 _brickSize;

        private readonly List<Brick> _bricks = new();
        private Vector2Int _gridSize;
        private Vector2 _gridStart;
        private Vector2 _gridEnd;

        private void Start()
        {
            CalculateGridSize();
            FillGrid();
        }

        #region Grid

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
        /// Fills the grid with bricks.
        /// </summary>
        private void FillGrid()
        {
            for (var y = _gridSize.y; y >= 0; y--)
            {
                // MoveBricksDown();
                // yield return new WaitForSeconds(0.1f);
                // SpawnBrickRow();

                for (var x = 0; x < _gridSize.x; x++)
                {
                    var brick = _brickPool.GetPooledObject();
                    brick.transform.position = new Vector2(_gridStart.x + x * _brickSize.x, _gridStart.y - y * _brickSize.y);
                    brick.gameObject.SetActive(true);
                    
                    _bricks.Add(brick.GetComponent<Brick>());
                }
            }
        }

        [Button]
        private void ReBrick()
        {
            foreach (var brick in _bricks)
                Destroy(brick.gameObject);
            
            _bricks.Clear();
            
            CalculateGridSize();
            FillGrid();
        }
        
        #endregion
    }
}