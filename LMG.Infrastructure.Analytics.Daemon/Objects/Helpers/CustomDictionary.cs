using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public class CustomDictionary<T, U, V>
    {
        Dictionary<T, Dictionary<U, List<V>>> _dict = new Dictionary<T, Dictionary<U, List<V>>>();
        public CustomDictionary()
        {
        }

        public List<V> this[T i, U j]
        {
            get
            {
                return _dict[i][j];
            }
            set
            {
                if (!_dict.ContainsKey(i))
                    _dict.Add(i, new Dictionary<U, List<V>>());

                if(!_dict[i].ContainsKey(j))
                    _dict[i].Add(j, new List<V>());
                
                _dict[i][j] = value;
            }
        }
    }
}
