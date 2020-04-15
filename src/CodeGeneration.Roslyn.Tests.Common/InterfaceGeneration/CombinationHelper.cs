using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public static class CombinationHelper
	{
		public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> elements, int k)
		{
			if (k == 0)
			{
				return Enumerable.Empty<IEnumerable<T>>();
			}
			int size = elements.Count();

			IEnumerable<IEnumerable<T>> Runner(IEnumerable<T> list, int n)
			{
				int skip = 1;
				foreach (var headList in list.Take(size - k + 1).Select(h => new T[] { h }))
				{
					if (n == 1)
					{
						yield return headList;
					}
					else
					{
						foreach (var tailList in Runner(list.Skip(skip), n - 1))
						{
							yield return headList.Concat(tailList);
						}
						skip++;
					}
				}
			}

			return Runner(elements, k);
		}
	}
}