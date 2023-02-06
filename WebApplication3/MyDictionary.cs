using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace WebApplication3
{
    public class MyDictionary : IDictionary<string, IList<string>>
    {
        private IDictionary<string, IList<string>> _dictionary { get; set; }
        public MyDictionary()
        {
            _dictionary = new Dictionary<string, IList<string>>();
        }
        public IList<string> this[string key] { get => _dictionary[key]; set => _dictionary[key] = value; }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<IList<string>> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(string key, string value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key].Add(value);
            }
            else
            {
                _dictionary.Add(key, new List<string>() { value });
            }
        }

        public void Add(string key, IList<string> value)
        {
            if (_dictionary.ContainsKey(key))
            {
                foreach (var i in value)
                {
                    _dictionary[key].Add(i);
                }
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        public void Add(KeyValuePair<string, IList<string>> item)
        {
            if (_dictionary.ContainsKey(item.Key))
            {
                foreach (var i in item.Value)
                {
                    _dictionary[item.Key].Add(i);
                }
            }
            else
            {
                _dictionary.Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, IList<string>> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, IList<string>>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, IList<string>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, IList<string>> item)
        {
            return _dictionary.Remove(item);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("<table style='border: 1px solid black'><thead><tr><th>Key</th><th>Value</th></tr><tbody>");
            foreach (KeyValuePair<string, IList<string>> pair in _dictionary)
            {
                stringBuilder.Append($"<tr  style='border: 1px solid black'><td style='border: 1px solid black'>{pair.Key}</td><td style='border: 1px solid black'><ul>{string.Join(' ', pair.Value.Select(x => $"<li>{x}</li>"))}</ul></td></tr>");
            }
            stringBuilder.Append("</tbody></table>");
            return stringBuilder.ToString();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IList<string> value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
