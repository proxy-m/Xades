using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Класс описывает один файл конфигурации найденный в каталоге
	/// </summary>
	public class GisBustedProxyConfigFile
		{
		/// <summary>
		/// Рабочий сервер
		/// </summary>
		public const string Production = "Production";

		/// <summary>
		/// Стенд СИТ1
		/// </summary>
		public const string SIT1 = "SIT1";

		/// <summary>
		/// Стенд СИТ2
		/// </summary>
		public const string SIT2 = "SIT2";

		/// <summary>
		/// Короткое имя файла
		/// </summary>
		public string Name
			{
			get; set;
			}

		/// <summary>
		/// Файл конфигурации по умолчанию
		/// </summary>
		public bool IsDefault
			{
			get; set;
			}

		/// <summary>
		/// Описание файла
		/// </summary>
		public string Title
			{
			get; set;
			}

		/// <summary>
		/// Тип файла конфигурации - Production, SIT1, SIT2
		/// </summary>
		public string Type
			{
			get; set;
			}

		/// <summary>
		/// Текстовое описание объекта
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			if (IsDefault)
				{
				return string.Format("{0}        - {1} (Тип - {2}), по умолчанию", Title, Name, Type);
				}

			return string.Format("{0}        - {1} (Тип - {2})", Title, Name, Type);
			}

		/// <summary>
		/// Это рабочий сервер
		/// </summary>
		public bool IsProduction
			{
			get
				{
				if (Type.Equals(Production, StringComparison.OrdinalIgnoreCase))
					{
					return true;
					}
				return false;
				}
			}

		/// <summary>
		/// Это тестовый сервер СИТ1
		/// </summary>
		public bool IsSIT1
			{
			get
				{
				if (Type.Equals(SIT1, StringComparison.OrdinalIgnoreCase))
					{
					return true;
					}
				return false;
				}
			}

		/// <summary>
		/// Это тестовый сервер СИТ2
		/// </summary>
		public bool IsSIT2
			{
			get
				{
				if (Type.Equals(SIT2, StringComparison.OrdinalIgnoreCase))
					{
					return true;
					}
				return false;
				}
			}

		/// <summary>
		/// Конфигурация правильная
		/// </summary>
		public bool IsValidConfig
			{
			get
				{
				if (IsProduction || IsSIT1 || IsSIT2)
					{
					return true;
					}
				return false;
				}
			}
		}

	/// <summary>
	/// Вспомогательный класс для сравнения экземпляров
	/// </summary>
	public class GisBustedProxyConfigFileComparer : IEqualityComparer<GisBustedProxyConfigFile>
		{
		#region Реализация интерфейса IEqualityComparer

		public bool Equals(GisBustedProxyConfigFile x, GisBustedProxyConfigFile y)
			{
			return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
			}

		public int GetHashCode(GisBustedProxyConfigFile g)
			{
			return g.Name.GetHashCode();
			}

		#endregion Реализация интерфейса IEqualityComparer
		}

	/// <summary>
	/// Вспомогательный класс для хранения списка файлов конфигурации
	/// </summary>
	public sealed class WcfConfigFiles
		{
		/// <summary>
		/// Список найденных файлов конфигурации
		/// </summary>
		private readonly List<GisBustedProxyConfigFile> m_List;

		/// <summary>
		/// Имя каталога с файлами конфигурации
		/// </summary>
		private string m_DirectoryName;

		/// <summary>
		/// Конструктор
		/// </summary>
		public WcfConfigFiles()
			{
			m_List = new List<GisBustedProxyConfigFile>();
			}

		#region Свойства

		/// <summary>
		/// Список найденных файлов конфигурации
		/// </summary>
		public List<GisBustedProxyConfigFile> ConfigFiles
			{
			get
				{
				return m_List;
				}
			}

		/// <summary>
		/// Имя каталога с файлами конфигурации
		/// </summary>
		public string DirectoryName
			{
			get
				{
				return this.m_DirectoryName;
				}
			}

		#endregion Свойства

		/// <summary>
		/// Инициализация
		/// </summary>
		public bool Init()
			{
			StringBuilder sb = new StringBuilder();
			Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
			string AssemblyLocation = ExecutingAssembly.Location;
			m_DirectoryName = Path.GetDirectoryName(AssemblyLocation);

			string ConfigFileName = Path.Combine(DirectoryName, "GisBustedConfigFiles.xml");

			if (!File.Exists(ConfigFileName)) // файла конфигурации не существует
				{
				bool b = AddMainConfigFile(DirectoryName);
				return b;
				}

			bool bres;
			bres = Parse(DirectoryName, ConfigFileName);
			if (!bres)
				{
				sb.AppendFormat("Ошибка разбора файла {0}", ConfigFileName);
				GisGlobals.ErrorMessageBox(sb.ToString());
				return false;
				}

			int DefaultFilesCount = ConfigFiles.Count(s => s.IsDefault);

			if (DefaultFilesCount == 0)
				{
				sb.AppendFormat("Не указан файл конфигурации по умолчанию в файле {0}\r\n\r\nДля продолжения работы исправьте файл.", ConfigFileName);
				GisGlobals.ErrorMessageBox(sb.ToString());
				return false;
				}

			if (DefaultFilesCount > 1)
				{
				sb.AppendFormat("Указано более одного файла конфигурации по умолчанию в файле {0}.\r\n\r\nДля продолжения работы исправьте файл.", ConfigFileName);
				GisGlobals.ErrorMessageBox(sb.ToString());
				return false;
				}

			return true;
			}

		/// <summary>
		/// Добавить единственный файл конфигурации
		/// </summary>
		/// <param name="DirectoryName">Каталог</param>
		private bool AddMainConfigFile(string DirectoryName)
			{
			GisBustedProxyConfigFile d = new GisBustedProxyConfigFile();
			d.Type = GisBustedProxyConfigFile.Production;
			d.Title = "Рабочий сервер";
			d.Name = "GisBusted_production.config";
			d.IsDefault = true;

			if (!File.Exists(Path.Combine(DirectoryName, d.Name)))
				{
				return false;
				}

			ConfigFiles.Clear();
			ConfigFiles.Add(d);
			return true;
			}

		/// <summary>
		/// Возвращает файл конфигурации по умолчанию
		/// </summary>
		public GisBustedProxyConfigFile DefaultProxyConfigFile
			{
			get
				{
				GisBustedProxyConfigFile d = ConfigFiles.First(cf => cf.IsDefault);
				return d;
				}
			}

		/// <summary>
		/// Разобрать строку как bool
		/// </summary>
		/// <param name="s">строка</param>
		/// <returns></returns>
		private bool ParseBool(string s)
			{
			if (string.IsNullOrEmpty(s))
				{
				return false;
				}

			if (
				s.Equals("true", StringComparison.OrdinalIgnoreCase) ||
				s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
				s.Equals("да", StringComparison.OrdinalIgnoreCase) ||
				s.Equals("1", StringComparison.OrdinalIgnoreCase)
				)
				{
				return true;
				}

			return false;
			}

		/// <summary>
		/// Разобрать файл конфигурации
		/// </summary>
		/// <param name="DirectoryName">Каталог</param>
		/// <param name="ConfigFileName">Имя файла конфигурации</param>
		/// <returns>true если файл конфигурации разобран успешно</returns>
		private bool Parse(string DirectoryName, string ConfigFileName)
			{
			XElement doc = XElement.Load(ConfigFileName);

			IEnumerable<GisBustedProxyConfigFile> Files =
			from f in doc.Elements("File")
			select new GisBustedProxyConfigFile
				{
				Name = (string) f.Attribute("Name"),
				Title = (string) f.Attribute("Title"),
				Type = (string) f.Attribute("Type"),
				IsDefault = ParseBool((string) f.Attribute("Default"))
				};

			GisBustedProxyConfigFileComparer Comparer = new GisBustedProxyConfigFileComparer();
			Files = Files.Where(cf => File.Exists(Path.Combine(DirectoryName, cf.Name)));
			Files = Files.Distinct(Comparer);
			Files = Files.Except(m_List, Comparer);
			Files = Files.OrderBy(cf => cf.Title);

			m_List.AddRange(Files.ToList().Where(t => t.IsValidConfig));

			if (m_List.Any())
				{
				return true;
				}

			return false;
			}
		}
	}
