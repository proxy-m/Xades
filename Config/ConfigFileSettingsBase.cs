﻿using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GisBusted.Config
	{
	/// <summary>
	/// Класс для чтения настроек конфигурации
	/// </summary>
	public class ConfigFileSettingsBase
		{
		/// <summary>
		/// Имя файла конфигурации сборки, например GisBusted.exe.config
		/// </summary>
		private string m_AssemblyConfigFileName;

		/// <summary>
		/// Полное имя файла конфигурации сборки, например E:\ГИС ЖКХ\GisBusted\GisBusted\bin\Debug\GisBusted.exe.config
		/// </summary>
		private string m_FullAssemblyConfigFileName;

		/// <summary>
		/// Имя файла сборки, например GisBusted.exe
		/// </summary>
		private string m_AssemblyFileName;

		/// <summary>
		/// Путь к файлу сборки, например E:\ГИС ЖКХ\GisBusted\GisBusted\bin\Debug
		/// </summary>
		private string m_AssemblyDirectoryName;

		/// <summary>
		/// Конфигурация прочитанная из файла конфигурации
		/// </summary>
		private Configuration m_Configuration;

		/// <summary>
		/// Секция  &lt;appSettings&gt; файла конфигурации
		/// </summary>
		private AppSettingsSection m_AppSettings;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		public ConfigFileSettingsBase()
			{
			GetAssemblyNames();
			}

		#endregion Конструкторы

		#region Вспомогательные функции

		/// <summary>
		/// Разобрать названия частей сборки
		/// </summary>
		private void GetAssemblyNames()
			{
			Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
			if (ExecutingAssembly == null)
				{
				throw new InvalidOperationException("Ошибка определения того, где мы находимся");
				}

			string AssemblyLocation = ExecutingAssembly.Location;

			m_AssemblyFileName = Path.GetFileName(AssemblyLocation);
			m_AssemblyDirectoryName = Path.GetDirectoryName(AssemblyLocation);
			m_AssemblyConfigFileName = m_AssemblyFileName + ".config";
			m_FullAssemblyConfigFileName = Path.Combine(m_AssemblyDirectoryName, m_AssemblyConfigFileName);
			}

		/// <summary>
		/// Показать MessageBox с ошибкой
		/// </summary>
		/// <param name="Text">Текст сообщения</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		internal void ErrorMessageBox(string Text)
			{
			MessageBox.Show(Text, AssemblyFileName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		/// <summary>
		/// Прочитать значение типа bool из настроек конфигурации
		/// </summary>
		/// <param name="key">Имя параметра в файле конфигурации</param>
		/// <returns>true если параметр найден и его значение равно 'true','yes','да' или '1'</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected bool GetAppConfigBool(string key)
			{
			string s;

			s = GetAppConfigString(key);

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
		/// Прочитать значение типа bool из настроек конфигурации
		/// </summary>
		/// <param name="key">Имя параметра в файле конфигурации</param>
		/// <param name="Value">Результат</param>
		/// <returns>true если параметр найден и его значение удалось преобразовать в int</returns>
		protected bool GetAppConfigInt(string key, out int Value)
			{
			string s;
			Value = 0;

			s = GetAppConfigString(key);

			if (string.IsNullOrEmpty(s))
				{
				return false;
				}

			int result;
			if (int.TryParse(s, out result))
				{
				Value = int.Parse(s, CultureInfo.CurrentCulture);
				return true;
				}

			return false;
			}

		/// <summary>
		/// Прочитать значение типа string из настроек конфигурации
		/// </summary>
		/// <param name="key">Имя параметра в файле конфигурации</param>
		/// <returns>строка если параметр найден или пустая строка если не найден</returns>
		protected string GetAppConfigString(string key)
			{
			string s;

			if (AppSettings == null)
				{
				throw new InvalidOperationException("Ошибка открытия файла конфигурации - нет секции <appSettings>");
				}

			KeyValueConfigurationCollection kv = AppSettings.Settings;

			if (kv == null)
				{
				return string.Empty;
				}

			if (kv.Count == 0)
				{
				return string.Empty;
				}

			bool bFound = false;

			for (int i = 0; i < kv.AllKeys.Length; i++)
				{
				if (kv.AllKeys[i] == key)
					{
					bFound = true;
					break;
					}
				}

			if (!bFound)
				{
				return string.Empty;
				}

			s = kv[key].Value;

			if (s == null)
				{
				s = string.Empty;
				}

			return s;
			}

		/// <summary>
		/// Разобрать файл конфигурации
		/// </summary>
		/// <returns>true если все хорошо</returns>
		private bool LoadConfigFile()
			{
			ExeConfigurationFileMap ConfigurationFileMap = new ExeConfigurationFileMap();
			ConfigurationFileMap.ExeConfigFilename = AssemblyConfigFileName;

			m_Configuration = ConfigurationManager.OpenMappedExeConfiguration(ConfigurationFileMap, ConfigurationUserLevel.None);
			if (m_Configuration == null)
				{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Ошибка открытия файла конфигурации {0}", FullAssemblyConfigFileName);
				ErrorMessageBox(sb.ToString());
				return false;
				}

			m_AppSettings = m_Configuration.AppSettings;
			if (m_AppSettings == null)
				{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Ошибка открытия файла конфигурации {0} - нет секции <appSettings>", FullAssemblyConfigFileName);
				ErrorMessageBox(sb.ToString());
				return false;
				}
			return true;
			}

		#endregion Вспомогательные функции

		/// <summary>
		/// Загрузить файл конфигурации
		/// </summary>
		/// <returns>true если файл конфигурации существует и успешно загружен</returns>
		public bool Init()
			{
			bool bres;

			if (!File.Exists(FullAssemblyConfigFileName))
				{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Файл {0} не найден", FullAssemblyConfigFileName);
				ErrorMessageBox(sb.ToString());
				return false;
				}

			bres = LoadConfigFile();
			return bres;
			}

		#region Свойства

		/// <summary>
		/// Конфигурация прочитанная из файла конфигурации
		/// </summary>
		public Configuration Configuration
			{
			get
				{
				return m_Configuration;
				}
			}

		/// <summary>
		/// Секция  appSettings файла конфигурации
		/// </summary>
		public AppSettingsSection AppSettings
			{
			get
				{
				return m_AppSettings;
				}
			}

		/// <summary>
		/// Имя файла конфигурации библиотеки
		/// </summary>
		public string AssemblyConfigFileName
			{
			get
				{
				return m_AssemblyConfigFileName;
				}
			}

		/// <summary>
		/// Имя файла сборки
		/// </summary>
		public string AssemblyFileName
			{
			get
				{
				return m_AssemblyFileName;
				}
			}

		/// <summary>
		/// Полное имя файла конфигурации сборки
		/// </summary>
		public string FullAssemblyConfigFileName
			{
			get
				{
				return m_FullAssemblyConfigFileName;
				}
			}

		/// <summary>
		/// Путь к файлу сборки
		/// </summary>
		public string AssemblyDirectoryName
			{
			get
				{
				return m_AssemblyDirectoryName;
				}
			}

		#endregion Свойства
		}
	}