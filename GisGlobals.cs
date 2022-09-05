using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace GisBusted
	{
	/// <summary>
	/// Модель используемого криптопровайдера
	/// </summary>
	public enum ProviderModel
		{
		dotNetFramework,
		CryptoApi
		}

	/// <summary>
	/// Глобальные переменные и настройки
	/// </summary>
	public static class GisGlobals
		{
		/// <summary>
		/// Список файлов конфигурации
		/// </summary>
		private static WCF.WcfConfigFiles s_ConfigFiles = new WCF.WcfConfigFiles();

		/// <summary>
		/// Настройки из файла конфигурации GisBusted.exe.config
		/// </summary>
		private static readonly Config.ConfigFileSettings s_ConfigFileSettings = new Config.ConfigFileSettings();

		/// <summary>
		/// Настройки из файла конфигурации GisBusted__XXX.config для создания точек подключения и привязок
		/// </summary>
		private static readonly WCF.WcfProxyConfiguration s_WcfProxyConfiguration = new WCF.WcfProxyConfiguration();

		/// <summary>
		/// Транспортный сертификат
		/// </summary>
		private static X509Certificate2 s_TransportCertificate;

		/// <summary>
		/// Cертификат для подписи
		/// </summary>
		private static X509Certificate2 s_SigningCertificate;

		#region Свойства

		/// <summary>
		/// Модель используемого криптопровайдера
		/// </summary>
		public static ProviderModel ProviderModel
			{
			get;
			set;
			}

		/// <summary>
		/// Список файлов конфигурации
		/// </summary>
		public static WCF.WcfConfigFiles ConfigFiles
			{
			get
				{
				return s_ConfigFiles;
				}
			}

		/// <summary>
		/// Настройки файла конфигурации
		/// </summary>
		public static Config.ConfigFileSettings ConfigFileSettings
			{
			get
				{
				return s_ConfigFileSettings;
				}
			}

		/// <summary>
		/// Настройки файла конфигурации веб-сервисов
		/// </summary>
		public static WCF.WcfProxyConfiguration ProxyConfiguration
			{
			get
				{
				return s_WcfProxyConfiguration;
				}
			}

		/// <summary>
		/// Транспортный сертификат
		/// </summary>
		public static X509Certificate2 TransportCertificate
			{
			get
				{
				return s_TransportCertificate;
				}
			}

		/// <summary>
		/// Сертификат подписи
		/// </summary>
		public static X509Certificate2 SigningCertificate
			{
			get
				{
				return s_SigningCertificate;
				}
			}

		/// <summary>
		/// Иконка приложения
		/// </summary>
		public static Icon ApplicationIcon
			{
			get;
			set;
			}

		#endregion Свойства

		/// <summary>
		/// Загрузить иконку из ресурсов библиотеки
		/// </summary>
		/// <param name="IconName">Имя файла иконки, например MainForm.ico</param>
		/// <returns>ресурс иконки</returns>
		internal static Icon LoadIcon(string IconName)
			{
			Icon IconToLoad;
			Stream st;
			Assembly a = Assembly.GetExecutingAssembly();

#if DEBUG
			var s = a.GetManifestResourceNames();
			var names = a.GetManifestResourceNames().Where(n => n.Contains(".Images."));
#endif // DEBUG

			st = a.GetManifestResourceStream("GisBusted.Images." + IconName);
			IconToLoad = new System.Drawing.Icon(st);

			return IconToLoad;
			}

		/// <summary>
		/// Показать MessageBox с ошибкой
		/// </summary>
		/// <param name="Text">Текст сообщения</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		internal static void ErrorMessageBox(string Text)
			{
			MessageBox.Show(Text, "GisBusted.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		/// <summary>
		/// Инициализация соединения с ГИС ЖКХ - выбор файла конфигурации
		/// </summary>
		/// <param name="ConfigFile">Файл конфигурации</param>
		/// <returns>true еслм все хорошо</returns>
		internal static bool InitProxyConfiguration(WCF.GisBustedProxyConfigFile ConfigFile)
			{
			bool bres = ProxyConfiguration.Init(ConfigFile);
			return bres;
			}

		/// <summary>
		/// Возвращает версию файла сборки (та, которая указывается в AssemblyFileVersion в файле AssemblyInfo.cs)
		/// </summary>
		/// <returns></returns>
		internal static string GetAssemblyFileVersion()
			{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			string version = fvi.FileVersion;
			return version;
			}

		/// <summary>
		/// Глобальные настройки WCF
		/// </summary>
		/// <returns></returns>
		internal static bool ConfigureWCF()
			{
			WCF.RemoteCertificateValidator.ConfigureServicePointManager();

			// Аутентификация на тестовом сервере
			// WCF.BasicAuthenticationHelper.SetUserNameAndPassword("lanit", "tv,n8!Ya");
			WCF.BasicAuthenticationHelper.SetUserNameAndPassword("sit", "rZ_GG72XS^Vf55ZW");
			return true;
			}

		/// <summary>
		/// Выбрать сертификаты
		/// </summary>
		/// <returns></returns>
		internal static bool SelectCertificates()
			{
			bool bres;

			string TransportCertificateThumbprint = ConfigFileSettings.TransportCertificateThumbprint;
			string SigningCertificateThumbprint = ConfigFileSettings.SigningCertificateThumbprint;
			bres = SelectCertificates(TransportCertificateThumbprint, SigningCertificateThumbprint);
			return bres;
			}

		/// <summary>
		/// Выбрать сертификаты
		/// </summary>
		/// <returns></returns>
		internal static bool SelectCertificates(string TransportCertificateThumbprint, string SigningCertificateThumbprint)
			{
			bool bres;
			X509Certificate2 tTransportCertificate;
			X509Certificate2 tSigningCertificate;

			bres = Crypto.CryptoCertificateHelper.FindOrSelectCertificates(TransportCertificateThumbprint, SigningCertificateThumbprint, out tTransportCertificate, out tSigningCertificate);
			if (!bres)
				{
				return false;
				}

			s_SigningCertificate = tSigningCertificate;
			s_TransportCertificate = tTransportCertificate;
			return true;
			}
		}
	}
