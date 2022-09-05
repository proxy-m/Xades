using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Windows.Forms;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Вспомогательный класс описывает одну конечную точку
	/// </summary>
	public class WcfEndpointElement
		{
		/// <summary>
		/// Название конечной точки в файле конфигурации
		/// </summary>
		public string Name
			{
			get; set;
			}

		/// <summary>
		/// Адрес конечной точки
		/// </summary>
		public string Address
			{
			get; set;
			}

		public override string ToString()
			{
			return string.Format(CultureInfo.InvariantCulture, "{0} - {1}", Name, Address);
			}
		}

	/// <summary>
	/// Класс для разбора конфигурации ServiceModel из стороннего файла
	/// </summary>
	public sealed class WcfProxyConfiguration
		{
		/// <summary>
		/// Файл конфигурации
		/// </summary>
		private GisBustedProxyConfigFile m_ConfigFile;

		/// <summary>
		/// Конфигурация прочитанная из отдельного файла
		/// </summary>
		private Configuration m_ProxyConfiguration;

		/// <summary>
		/// Секция system.serviceModel
		/// </summary>
		private ServiceModelSectionGroup m_ServiceModel;

		/// <summary>
		/// Секция system.serviceModel.bindings
		/// </summary>
		private BindingsSection m_Bindings;

		/// <summary>
		/// Секция system.serviceModel.client
		/// </summary>
		private ChannelEndpointElementCollection m_Endpoints;

		/// <summary>
		/// Конструктор
		/// </summary>
		public WcfProxyConfiguration()
			{
			}

		#region Свойства

		/// <summary>
		/// Имя файла конфигурации прокси
		/// </summary>
		public GisBustedProxyConfigFile ConfigFile
			{
			get
				{
				return m_ConfigFile;
				}
			}

		/// <summary>
		/// Конфигурация прочитанная из отдельного файла
		/// </summary>
		public Configuration ProxyConfiguration
			{
			get
				{
				return m_ProxyConfiguration;
				}
			}

		/// <summary>
		/// Секция system.serviceModel
		/// </summary>
		public ServiceModelSectionGroup ServiceModel
			{
			get
				{
				return m_ServiceModel;
				}
			}

		/// <summary>
		/// Секция system.serviceModel.bindings
		/// </summary>
		public BindingsSection Bindings
			{
			get
				{
				return m_Bindings;
				}
			}

		/// <summary>
		/// Секция system.serviceModel.client
		/// </summary>
		public ChannelEndpointElementCollection Endpoints
			{
			get
				{
				return m_Endpoints;
				}
			}

		/// <summary>
		/// Список конечных тоек
		/// </summary>
		public List<WcfEndpointElement> EndpointElements
			{
			get
				{
				if (Endpoints == null)
					{
					throw new InvalidOperationException("Класс не инициализирован");
					}

				var x = Endpoints.Cast<ChannelEndpointElement>();

				IEnumerable<WcfEndpointElement> y =
				from f in x
				select new WcfEndpointElement
					{
					Address = (string) f.Address.AbsoluteUri,
					Name = (string) f.Name
					};

				List<WcfEndpointElement> z = y.OrderBy(f => f.Name).ToList();
				return z;
				}
			}

		#endregion Свойства

		/// <summary>
		/// Создать имя файла конфигурации (полный путь)
		/// </summary>
		/// <returns>имя файла конфигурации</returns>
		/// <param name="FileName">короткое название файла конфигурации</param>
		private static string ParseProxyConfigFileName(string FileName)
			{
			Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
			string AssemblyLocation = ExecutingAssembly.Location;
			string DirectoryName = Path.GetDirectoryName(AssemblyLocation);
			string ConfigFileName = Path.Combine(DirectoryName, FileName);

			return ConfigFileName;
			}

		/// <summary>
		/// Полное имя файла конфигурации
		/// </summary>
		private string GetProxyConfigFileName()
			{
			return ParseProxyConfigFileName(ConfigFile.Name);
			}

		/// <summary>
		/// Разобрать файл конфигурации
		/// </summary>
		/// <returns>true если все хорошо</returns>
		private bool LoadConfigFile()
			{
			ExeConfigurationFileMap ConfigurationFileMap = new ExeConfigurationFileMap();
			ConfigurationFileMap.ExeConfigFilename = GetProxyConfigFileName();

			m_ProxyConfiguration = ConfigurationManager.OpenMappedExeConfiguration(ConfigurationFileMap, ConfigurationUserLevel.None);
			if (m_ProxyConfiguration == null)
				{
				throw new InvalidOperationException("Ошибка открытия файла конфигурации");
				}

			m_ServiceModel = ServiceModelSectionGroup.GetSectionGroup(ProxyConfiguration);
			if (m_ServiceModel == null)
				{
				throw new InvalidOperationException("Ошибка получения m_ServiceModel");
				}

			m_Bindings = ServiceModel.Bindings;
			if (m_Bindings == null)
				{
				throw new InvalidOperationException("Ошибка получения m_Bindings");
				}

			m_Endpoints = ServiceModel.Client.Endpoints;
			if (m_Endpoints == null)
				{
				throw new InvalidOperationException("Ошибка получения m_Endpoints");
				}

			// ProxyConfiguration.SaveAs("test.xml");
			return true;
			}

		/// <summary>
		/// Найти Endpoint по названию
		/// </summary>
		/// <param name="Name">Название конечной точки</param>
		/// <returns>конечную точку или null если найти не удалось</returns>
		public System.ServiceModel.Configuration.ChannelEndpointElement FindEndpoint(string Name)
			{
			int nCount;
			nCount = Endpoints.Count;

			for (int i = 0; i < nCount; i++)
				{
				ChannelEndpointElement Endpoint = Endpoints[i];

				if (Endpoint.Name.Equals(Name))
					{
					return Endpoint;
					}
				}

			return null;
			}

		/// <summary>
		/// Найти Binding по конечной точке Endpoint
		/// </summary>
		/// <param name="Endpoint">Название конечной точки</param>
		/// <returns>null если не удалось найти</returns>
		public System.ServiceModel.Channels.Binding FindBinding(ChannelEndpointElement Endpoint)
			{
			System.ServiceModel.Channels.Binding TargetBinding = null;

			foreach (BindingCollectionElement Element in Bindings.BindingCollections)
				{
				foreach (IBindingConfigurationElement ConfiguredBinding in Element.ConfiguredBindings)
					{
					if (Element.BindingName.Equals(Endpoint.Binding) &&
						ConfiguredBinding.Name.Equals(Endpoint.BindingConfiguration)
						)
						{
						TargetBinding = (System.ServiceModel.Channels.Binding) Activator.CreateInstance(Element.BindingType);
						TargetBinding.Name = ConfiguredBinding.Name;
						ConfiguredBinding.ApplyConfiguration(TargetBinding);
						break;
						}
					}
				}

			return TargetBinding;
			}

		/// <summary>
		/// Загрузить файл конфигурации
		/// </summary>
		/// <returns>true если файл конфигурации существует и успешно загружен</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		public bool Init(GisBustedProxyConfigFile ConfigFile)
			{
			bool bres;

			if (ConfigFile == null)
				{
				throw new ArgumentNullException("ConfigFile", "ConfigFile не может быть null");
				}

			m_ConfigFile = ConfigFile;

			string ProxyConfigFileName = GetProxyConfigFileName();

			if (!File.Exists(ProxyConfigFileName))
				{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Файл {0} не найден", ProxyConfigFileName);
				MessageBox.Show(sb.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
				}

			bres = LoadConfigFile();
			if (!bres)
				{
				string s = string.Format("Ошибка чтения файла конфигурации {0}", ProxyConfigFileName);
				GisGlobals.ErrorMessageBox(s);
				return bres;
				}

			return bres;
			}

		/// <summary>
		/// Получить Binding и EndpointAddress по имени Endpoint
		/// </summary>
		/// <param name="EndpointName">Название Endpoint</param>
		/// <param name="SenderCredentials">Атрибуты отправителя для подписи запроса</param>
		/// <param name="TargetBinding">Результирующий Binding</param>
		/// <param name="TargetEndpointAddress">Результирующий EndpointAddress</param>
		/// <returns>true если все получилось</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
		public bool GetBindingByEndpointName(string EndpointName, SenderCredentials SenderCredentials, out System.ServiceModel.Channels.Binding TargetBinding, out System.ServiceModel.EndpointAddress TargetEndpointAddress)
			{
			TargetBinding = null;
			TargetEndpointAddress = null;

			#region Поиск

			ChannelEndpointElement TargetEndpoint = FindEndpoint(EndpointName);

			if (TargetEndpoint == null)
				{
				return false;
				}

			TargetBinding = FindBinding(TargetEndpoint);

			if (TargetBinding == null)
				{
				return false;
				}

			#endregion Поиск

			#region Создание EndpointAddress

			// заменяем http на https
			Uri OriginalAddress = TargetEndpoint.Address;
			UriBuilder builder = new UriBuilder(OriginalAddress);
			builder.Scheme = "https";

			Uri modifiedAddress = builder.Uri;

			TargetEndpointAddress = new EndpointAddress(modifiedAddress);

			#endregion Создание EndpointAddress

			#region Настройка Binding

			BasicHttpBinding basicBinding = TargetBinding as BasicHttpBinding;
			if (basicBinding == null)
				{
				throw new InvalidOperationException("Не удается привести к BasicHttpBinding");
				}

			basicBinding.UseDefaultWebProxy = false;

			// шифрование на уровне транспорта
			basicBinding.Security.Mode = BasicHttpSecurityMode.Transport;
			basicBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

			// дополнительные настройки
			ConfigureBinding(TargetBinding);

			#endregion Настройка Binding

			return true;
			}

		/// <summary>
		/// Дополнительные настройки Binding из-за кривой конфигурации файлов конфигурации Ланита В
		/// частности, увеличение размера буферов для принимаемого сообщения
		/// </summary>
		private static void ConfigureBinding(System.ServiceModel.Channels.Binding TargetBinding)
			{
			if (TargetBinding == null)
				{
				throw new ArgumentNullException("TargetBinding", "TargetBinding не может быть null");
				}

			BasicHttpBinding basicBinding = TargetBinding as BasicHttpBinding;
			if (basicBinding == null)
				{
				throw new InvalidOperationException("Не удается привести к BasicHttpBinding");
				}

			const int INCREASE_SIZE = 5;

			basicBinding.MaxReceivedMessageSize = basicBinding.MaxReceivedMessageSize * INCREASE_SIZE;
			basicBinding.MaxBufferSize = basicBinding.MaxBufferSize * INCREASE_SIZE;

			basicBinding.ReaderQuotas.MaxArrayLength = basicBinding.ReaderQuotas.MaxArrayLength * INCREASE_SIZE;
			basicBinding.ReaderQuotas.MaxStringContentLength = basicBinding.ReaderQuotas.MaxStringContentLength * INCREASE_SIZE;
			basicBinding.ReaderQuotas.MaxNameTableCharCount = basicBinding.ReaderQuotas.MaxNameTableCharCount * INCREASE_SIZE;
			}
		}
	}
