/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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
