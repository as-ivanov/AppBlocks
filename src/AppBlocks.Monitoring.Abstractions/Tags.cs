using System;
using System.Linq;
using System.Text;

namespace AppBlocks.Monitoring.Abstractions
{
	public readonly struct Tags : IEquatable<Tags>
	{
		private static readonly string[] _emptyArray = new string[0];
		public static readonly Tags Empty = new Tags(_emptyArray, _emptyArray);
		private readonly string _key;
		private readonly string[] _keys;
		private readonly string _value;
		private readonly string[] _values;

		public Tags(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentOutOfRangeException(nameof(key));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_key = key;
			_value = value;
			_keys = null;
			_values = null;
		}

		public Tags(string[] keys, string[] values)
		{
			if (keys == null)
			{
				throw new ArgumentNullException(nameof(keys), "keys cannot be null");
			}

			if (values == null)
			{
				throw new ArgumentNullException(nameof(keys), "values cannot be null");
			}

			if (keys.Length != values.Length)
			{
				throw new InvalidOperationException("keys length must be equal to values length");
			}

			if (keys.Length == 1)
			{
				throw new InvalidOperationException("Use constructor overload for single key/value");
			}

			if (keys.Any(t => t == null))
			{
				throw new InvalidOperationException("keys keys cannot contains nulls");
			}

			if (values.Any(t => t == null))
			{
				throw new InvalidOperationException("values values cannot contains nulls");
			}

			if (keys.Any(string.IsNullOrWhiteSpace))
			{
				throw new InvalidOperationException("keys keys cannot contains empty or whitespace strings");
			}

			if (values.Any(string.IsNullOrWhiteSpace))
			{
				throw new InvalidOperationException("values keys cannot contains empty or whitespace strings");
			}

			_key = null;
			_value = null;
			_keys = keys;
			_values = values;
		}

		public string Key => _key;

		public string[] Keys => _keys;

		public string Value => _value;

		public string[] Values => _values;

		public bool Equals(Tags other)
		{
			return _key == other._key && _value == other._value &&
			       (_values == null && _keys == null ||
			        _values.SequenceEqual(other._values) && _keys.SequenceEqual(other._keys));
		}

		public override bool Equals(object obj)
		{
			return obj is Tags other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_key, _keys, _value, _values);
		}

		public static bool operator ==(Tags left, Tags right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Tags left, Tags right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			if (_key != null)
			{
				return $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}";
			}

			if (_keys == _emptyArray)
			{
				return "(empty)";
			}

			var sb = new StringBuilder();
			for (int i = 0; i < _keys.Length; i++)
			{
				sb.Append($"{_keys[i]}:{_values[i]}");
			}

			return sb.ToString();
		}
	}
}