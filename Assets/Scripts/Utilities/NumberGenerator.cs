using System.Globalization;
using UnityEngine;
using VInspector;

namespace Prez.Utilities
{
    public class NumberGenerator : MonoBehaviour
    {
        [Tab("Brick")]
        [SerializeField] private int _brickMaxLevel;
        [SerializeField] private float _brickIncrementStart;
        [SerializeField] private float _brickIncrementPercent;
        [SerializeField] private int _brickIncrementPercentRate;

        [Tab("Result")]
        [SerializeField, TextArea(4, 50)] private string _result;
        
        [Button]
        [Tab("Brick")]
        private void GenerateBrickExperience()
        {
            _result = string.Empty;
            var increment = _brickIncrementStart;
            var experience = 0f;
            
            for (var i = 1; i <= _brickMaxLevel; i++)
            {
                if (i % _brickIncrementPercentRate == 0)
                    increment *= _brickIncrementPercent;

                experience += increment;

                _result += experience.ToString("0.00", CultureInfo.InvariantCulture) + ",\n";
            }
        }
    }
}