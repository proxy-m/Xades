namespace GisBusted.Config
	{
	public class ConfigFileSettings : ConfigFileSettingsBase
		{
		#region Константы

		/// <summary>
		/// Отпечаток транспортного сертификата
		/// </summary>
		private const string cTransportCertificateThumbprint = "TransportCertificateThumbprint";

		/// <summary>
		/// Отпечаток сертификата для сообщений
		/// </summary>
		private const string cSigningCertificateThumbprint = "SigningCertificateThumbprint";

		/// <summary>
		/// Идентификатор организации
		/// </summary>
		private const string сorgPPAGUID = "orgPPAGUID";

		#endregion Константы

		#region Свойства

		/// <summary>
		/// Отпечаток транспортного сертификата
		/// </summary>
		public string TransportCertificateThumbprint
			{
			get
				{
				return GetAppConfigString(cTransportCertificateThumbprint);
				}
			}

		/// <summary>
		/// Отпечаток сертификата для сообщений
		/// </summary>
		public string SigningCertificateThumbprint
			{
			get
				{
				return GetAppConfigString(cSigningCertificateThumbprint);
				}
			}

		/// <summary>
		/// Идентификатор организации
		/// </summary>
		public string orgPPAGUID
			{
			get
				{
				return GetAppConfigString(сorgPPAGUID);
				}
			}

		#endregion Свойства
		}
	}
