namespace Mediabox.API {
	public static class StringExtensions {
		public static string LowerCaseFirst(this string str) {
			if (str.Length == 0)
				return str;
			return str.Substring(0, 1).ToLower() + str.Substring(1);
		}
	}
}