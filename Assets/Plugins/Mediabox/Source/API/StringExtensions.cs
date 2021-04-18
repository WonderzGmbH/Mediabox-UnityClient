using System;
using System.IO;
using System.Linq;

namespace Mediabox.API {
	public static class StringExtensions {
		public static string LowerCaseFirst(this string str) {
			if (str.Length == 0)
				return str;
			return str.Substring(0, 1).ToLower() + str.Substring(1);
		}

		public static string Remove(this string str, string instancesOf) {
			return str.Replace(instancesOf, string.Empty);
		}
		public static string Remove(this string str, params string[] instancesOf) {
			return instancesOf.Aggregate(str, (current, instanceOf) => current.Remove(instanceOf));
		}
	}
}