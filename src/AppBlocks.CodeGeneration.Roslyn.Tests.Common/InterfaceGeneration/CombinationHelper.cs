using System.Collections.Generic;
using System.Linq;

namespace AppBlocks.CodeGeneration.Roslyn.Tests.Common.InterfaceGeneration
{
	public static class CombinationHelper
	{
		public static IEnumerable<IEnumerable<T>> GetPossibleCombinations<T>(this IList<T> elements, int k)
		{
			if (k == 0)
			{
				return Enumerable.Empty<IEnumerable<T>>();
			}

			var size = elements.Count();

			IEnumerable<IEnumerable<T>> Runner(IEnumerable<T> list, int n)
			{
				var skip = 1;
				foreach (var headList in list.Take(size - k + 1).Select(h => new[] {h}))
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