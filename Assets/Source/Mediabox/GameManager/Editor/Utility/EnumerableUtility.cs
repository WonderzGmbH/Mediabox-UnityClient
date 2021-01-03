using System.Collections.Generic;

namespace Mediabox.GameManager.Editor.Utility {
	public static class EnumerableUtility {
		public static void ForEach<T>(this IEnumerable<T> enumerable, System.Action<T> method) {
			foreach (var item in enumerable)
				method(item);
		}
	}
}