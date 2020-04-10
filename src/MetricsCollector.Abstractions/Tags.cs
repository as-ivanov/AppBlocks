using System;
using System.Collections.Generic;
using System.Linq;

namespace MetricsCollector.Abstractions
{
	public readonly struct Tags
	{
		private static readonly string[] EmptyArray = new string[0];
		public static readonly Tags Empty = new Tags(Tags.EmptyArray, Tags.EmptyArray);
		private readonly string _key;
		private readonly string[] _keys;
		private readonly string _value;
		private readonly string[] _values;

		public Tags(string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentOutOfRangeException(nameof(key));
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentOutOfRangeException(nameof(value));
			this._key = key;
			this._value = value;
			this._keys = (string[]) null;
			this._values = (string[]) null;
		}

		public Tags(string[] keys, string[] values)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys), "keys cannot be null");
			if (values == null)
				throw new ArgumentNullException(nameof(keys), "values cannot be null");
			if (keys.Length != values.Length)
				throw new InvalidOperationException("keys length must be equal to values length");
			if (((IEnumerable<string>) keys).Any((Func<string, bool>) (t => t == null)))
				throw new InvalidOperationException("keys keys cannot contains nulls");
			if (((IEnumerable<string>) values).Any((Func<string, bool>) (t => t == null)))
				throw new InvalidOperationException("values values cannot contains nulls");
			if (((IEnumerable<string>) keys).Any(new Func<string, bool>(string.IsNullOrWhiteSpace)))
				throw new InvalidOperationException("keys keys cannot contains empty or whitespace strings");
			if (((IEnumerable<string>) values).Any(new Func<string, bool>(string.IsNullOrWhiteSpace)))
				throw new InvalidOperationException("values keys cannot contains empty or whitespace strings");
			this._key = (string) null;
			this._value = (string) null;
			this._keys = keys;
			this._values = values;
		}

		public string Key => _key;

		public string[] Keys => _keys;

		public string Value => _value;

		public string[] Values => _values;
	}
}