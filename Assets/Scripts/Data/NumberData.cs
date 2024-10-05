using Prez.Utilities;

namespace Prez.Data
{
    public class NumberData
    {
        public long AsLong => _number / 100;
        public decimal AsDecimal => Helper.GetNumberAsDecimal(_number);
        public string AsString => Helper.GetNumberAsString(_number);

        private long _number;

        public NumberData(long number = 0)
        {
            Set(number);
        }

        /// <summary>
        /// Set to a NumberData.
        /// </summary>
        /// <param name="data"></param>
        public void Set(NumberData data)
        {
            Set(data.AsLong);
        }
        
        /// <summary>
        /// Set to an ulong number.
        /// </summary>
        /// <param name="number"></param>
        public void Set(long number)
        {
            number *= 100;
            _number = number;
        }
        
        /// <summary>
        /// Set to a decimal number.
        /// </summary>
        /// <param name="number"></param>
        public void Set(decimal number)
        {
            number *= 100m;
            _number = (long)number;
        }

        /// <summary>
        /// Add number by a NumberData.
        /// </summary>
        /// <param name="data"></param>
        public void Add(NumberData data)
        {
            Add(data.AsLong);
        }
        
        /// <summary>
        /// Add number by an ulong number.
        /// </summary>
        /// <param name="number"></param>
        public void Add(long number)
        {
            number *= 100;
            _number += number;
        }

        /// <summary>
        /// ADd number by a decimal number.
        /// </summary>
        /// <param name="number"></param>
        public void Add(decimal number)
        {
            number *= 100m;
            _number += (long)number;
        }

        /// <summary>
        /// Subtract number by NumberData.
        /// </summary>
        /// <param name="data"></param>
        public void Subtract(NumberData data)
        {
            Subtract(data.AsLong);
        }
        
        /// <summary>
        /// Subtract number by an ulong number.
        /// </summary>
        /// <param name="number"></param>
        public void Subtract(long number)
        {
            number *= 100;
            _number -= number;
        }

        /// <summary>
        /// Subtract number by a decimal number.
        /// </summary>
        /// <param name="number"></param>
        public void Subtract(decimal number)
        {
            number *= 100m;
            _number -= (long)number;
        }
    }
}