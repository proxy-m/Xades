using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GisBusted.Helpers
	{
	/// <summary>
	/// Вспомогательный класс для различных расширений
	/// </summary>
	public static class Extensions
		{
		/// <summary>
		/// Метод расширения для форматирования GUID в формате, воспринимаемом Ланитом
		/// </summary>
		/// <param name="str">Входная строка содержащая GUID</param>
		/// <returns>GUID в формате, воспринимаемом Ланитом</returns>
		public static string LanitGuid(this string str)
			{
			if (string.IsNullOrEmpty(str))
				{
				throw new ArgumentException("Строка не содержит GUID в требуемом формате - строка пустая");
				}
			Guid g = new Guid(str);
			string ret = g.ToString("D");
			return ret;
			}

		}
	}