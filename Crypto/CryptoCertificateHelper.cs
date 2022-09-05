using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Crypto
	{
	/// <summary>
	/// Вспомогательный класс для выбора сертификата
	/// </summary>
	public static class CryptoCertificateHelper
		{
		public const string Title = "Сертификат для ГИС ЖКХ";
		public const string DescriptionT = "Выберите транспортный сертификат (сертификат организации)";
		public const string DescriptionS = "Выберите сертификат уполномоченного специалиста";

		/// <summary>
		/// Глобальный объект для синхронизации доступа
		/// </summary>
		private static System.Object s_Locker = new System.Object();

		#region Свойства

		/// <summary>
		/// Глобальный объект для синхронизации доступа
		/// </summary>
		public static System.Object Locker
			{
			get
				{
				return s_Locker;
				}
			}

		#endregion Свойства

		/// <summary>
		/// Используемые хранилища
		/// </summary>
		private static List<StoreName> c_UsedStores = new List<StoreName>
			{
			StoreName.My // -- Personal

			// StoreName.TrustedPeople, -- Trusted people StoreName.AddressBook -- Other people
			};

		#region Вспомогательные функции

		/// <summary>
		/// Открыть хранилище сертификатов
		/// </summary>
		/// <param name="Store">Название хранилища сертификатов</param>
		/// <returns>Открытое для чтения хранилище сертификатов</returns>
		private static X509Store OpenStore(StoreName Store)
			{
			X509Store certificateStore = new X509Store(Store, StoreLocation.CurrentUser);
			certificateStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			return certificateStore;
			}

		/// <summary>
		/// Получить коллекцию сертификатов
		/// </summary>
		/// <returns>Коллекцию из сертификатов из указанных в c_UsedStores хранилищ</returns>
		private static X509Certificate2Collection GetСertificatesCollection()
			{
			X509Certificate2Collection UnitedCollection = new X509Certificate2Collection();

			foreach (StoreName Name in c_UsedStores)
				{
				X509Store s = null;

				try
					{
					s = OpenStore(Name);
					}
				catch (CryptographicException)
					{
					}

				if (s != null)
					{
					foreach (X509Certificate2 x2 in s.Certificates)
						{
						X509Certificate2Collection tmpCollection;
						string certificateThumbprint = x2.Thumbprint;
						tmpCollection = UnitedCollection.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
						if (tmpCollection.Count == 0)
							{
							UnitedCollection.Add(x2);
							}
						}
					}
				}

			return UnitedCollection;
			}

		/// <summary>
		/// Показать MessageBox с ошибкой
		/// </summary>
		/// <param name="Text">Текст сообщения</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		internal static void ErrorMessageBox(string Text)
			{
			GisBusted.GisGlobals.ErrorMessageBox(Text);
			}

		/// <summary>
		/// Удаляет из коллекции лишние сертификаты
		/// </summary>
		/// <param name="collection">Коллекция сертификатов</param>
		internal static void FilterCollection(X509Certificate2Collection collection)
			{
			// Что в OID'е тебе моём? https://www.sysadmins.lv/blog-ru/chto-v-oide-tebe-moem.aspx

			// справочник OID http://www.alvestrand.no/cgi-bin/hta/oidwordsearch?text=encryp

			// справочник OID http://www.2410000.ru/p_45_obektnyj_identifikator_oid.html

			// пока фильтра нет return;

			X509Certificate2Collection CertificatesToRemove = new X509Certificate2Collection();

			foreach (X509Certificate2 Certificate in collection)
				{
				bool AddKeyToList = false;

				const string CryptoPro_SignatureAlgorithmGost3411 = "1.2.643.2.2.3"; // Маркер КриптоПро - открытый ключ СКП, сформированный по алгоритму ГОСТ Р 34.11/34.10-2001

				if (Certificate.SignatureAlgorithm.Value == CryptoPro_SignatureAlgorithmGost3411)
					{
					AddKeyToList = true;
					}
				else
					foreach (X509Extension Extension in Certificate.Extensions)
						{
						X509EnhancedKeyUsageExtension EKU = Extension as X509EnhancedKeyUsageExtension;
						if (EKU == null)
							{
							continue;
							}

						const string ClientAuthentication = "1.3.6.1.5.5.7.3.2"; // Аутентификация клиента

						foreach (Oid OID in EKU.EnhancedKeyUsages)
							{
							switch (OID.Value)
								{
								case ClientAuthentication:
										{
										AddKeyToList = true;
										break;
										}
								}
							} // foreach (Oid OID
						} // foreach (X509Extension Extension

				if (
					(!AddKeyToList) ||
					(!Certificate.HasPrivateKey)
					)
					{
					CertificatesToRemove.Add(Certificate);
					}
				} // foreach (X509Certificate2

			collection.RemoveRange(CertificatesToRemove);
			}

		/// <summary>
		/// Проверить сертификат
		/// </summary>
		/// <param name="Certificate">Сертификат</param>
		private static bool VerifyCertificate(X509Certificate2 Certificate)
			{
			X509Chain chain = new X509Chain();

			try
				{
				bool chainBuilt = false;
				string Thumbprint = Certificate.Thumbprint;
				chainBuilt = chain.Build(Certificate);

				if (chainBuilt == false)
					{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("Отпечаток сертификата (Thumbprint): {0}\r\n", Thumbprint);
					foreach (X509ChainStatus chainStatus in chain.ChainStatus)
						{
						sb.AppendFormat("    Chain error: {0} {1}", chainStatus.Status, chainStatus.StatusInformation);
						}

					sb.AppendFormat("\r\n");

					// ErrorMessageBox(sb.ToString());
					} // end if (chainBuilt == false
				else  // if (chainBuilt == true)
					{
					}

				return chainBuilt;
				}
			catch (Exception)
				{
				}
			return false;
			}

		/// <summary>
		/// Подготовить строку для поиска по отпечатку сертификата
		/// </summary>
		/// <param name="certificateThumbprint">Отпечаток сертификата для поиска</param>
		/// <returns>Подготовленный отпечаток сертификата</returns>
		private static string FilterThumbprint(string certificateThumbprint)
			{
			StringBuilder sb = new StringBuilder(certificateThumbprint);
			sb.Replace(" ", "");
			sb.Replace("\t", "");

			string Thumbprint = sb.ToString();
			Thumbprint = Thumbprint.ToUpper(CultureInfo.CurrentCulture);
			return sb.ToString();
			}

		#endregion Вспомогательные функции

		#region Выбор сертификатов

		/// <summary>
		/// Показывает окно выбора сертификата
		/// </summary>
		/// <param name="OutputCertificate">Выбранный сертификат</param>
		/// <returns>true если сертификат выбран, false если отказ</returns>
		internal static bool SelectCertificate(out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputCertificate)
			{
			OutputCertificate = null;
			const string Description = "Выберите сертификат из списка ниже для начала работы с ГИС ЖКХ";

			bool bres = SelectCertificate(out OutputCertificate, Title, Description);

			return bres;
			}

		/// <summary>
		/// Показывает окно выбора сертификата
		/// </summary>
		/// <param name="OutputCertificate">Выбранный сертификат</param>
		/// <param name="Title">Заголовок окна</param>
		/// <param name="Description">Описание</param>
		/// <returns>true если сертификат выбран, false если отказ</returns>
		internal static bool SelectCertificate(out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputCertificate, string Title, string Description)
			{
			OutputCertificate = null;

			X509Certificate2 FoundCertificate;

			X509Certificate2Collection collection = GetСertificatesCollection();
			X509Certificate2Collection fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

			X509Certificate2Collection FinalCollection = fcollection;
			FilterCollection(FinalCollection);

			if (FinalCollection.Count == 0)
				{
				ErrorMessageBox("Не обнаружено подходящих сертификатов");
				return false;
				}

			X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(FinalCollection, Title, Description, X509SelectionFlag.SingleSelection);
			if (scollection.Count == 0)
				{
				return false;
				}

			FoundCertificate = scollection[0];

			OutputCertificate = FoundCertificate;

			return true;
			}

		/// <summary>
		/// Возвращает сертификат по его отпечатку из личного хранилища
		/// </summary>
		/// <param name="certificateThumbprint">Отпечаток сертификата</param>
		/// <param name="OutputCertificate">Найденный сертификат</param>
		/// <returns>Сертификат</returns>
		internal static bool FindCertificateByThumbprint(string certificateThumbprint, out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputCertificate)
			{
			OutputCertificate = null;

			if (string.IsNullOrEmpty(certificateThumbprint))
				{
				throw new ArgumentException("certificateThumbprint", "certificateThumbprint не может быть равен null");
				}

			X509Certificate2Collection FullCollection = GetСertificatesCollection();

			foreach (X509Certificate2 X in FullCollection)
				{
				if (!VerifyCertificate(X))
					{
					continue;
					}

				string Thumbprint = X.Thumbprint;
				if (Thumbprint.Equals(certificateThumbprint, StringComparison.OrdinalIgnoreCase))
					{
					OutputCertificate = X;
					return true;
					}
				}
			return false;
			}

		/// <summary>
		/// Найти сертификат или выбрать его из списка
		/// </summary>
		/// <param name="certificateThumbprint">Отпечаток сертификата</param>
		/// <param name="OutputCertificate">Сертификат</param>
		/// <returns>true если нашли или выбрали что-нибудь</returns>
		internal static bool FindOrSelectCertificate(string certificateThumbprint, out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputCertificate)
			{
			bool bres;
			OutputCertificate = null;

			string Thumbprint = FilterThumbprint(certificateThumbprint);

			if (!string.IsNullOrEmpty(Thumbprint))
				{
				bres = FindCertificateByThumbprint(Thumbprint, out OutputCertificate);
				if (bres)
					{
					return true;
					}
				}

			bres = SelectCertificate(out OutputCertificate);

			return bres;
			}

		/// <summary>
		/// Найти сертификат или выбрать его из списка
		/// </summary>
		/// <param name="certificateThumbprint">Отпечаток сертификата</param>
		/// <param name="OutputCertificate">Сертификат</param>
		/// <param name="Title">Заголовок окна</param>
		/// <param name="Description">Описание</param>
		/// <returns>true если нашли или выбрали что-нибудь</returns>
		internal static bool FindOrSelectCertificate
			(
			string certificateThumbprint,
			out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputCertificate,
			string Title,
			string Description
			)
			{
			bool bres;
			OutputCertificate = null;

			string Thumbprint = FilterThumbprint(certificateThumbprint);

			if (!string.IsNullOrEmpty(Thumbprint))
				{
				bres = FindCertificateByThumbprint(Thumbprint, out OutputCertificate);
				if (bres)
					{
					return true;
					}
				}

			bres = SelectCertificate(out OutputCertificate, Title, Description);

			return bres;
			}

		/// <summary>
		/// Найти два сертификата (транспортный или для подписи) или выбрать их из списка
		/// </summary>
		/// <param name="TransportCertificateThumbprint">Отпечаток транспортного сертификата</param>
		/// <param name="SigningCertificateThumbprint">Отпечаток сертификата для подписи</param>
		/// <param name="OutputTransportCertificate">Транспортный сертификат</param>
		/// <param name="OutputSigningCertificate">Сертификат для подписи</param>
		/// <returns>true если выбрали все что нужно</returns>
		public static bool FindOrSelectCertificates
			(
			string TransportCertificateThumbprint,
			string SigningCertificateThumbprint,
			out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputTransportCertificate,
			out System.Security.Cryptography.X509Certificates.X509Certificate2 OutputSigningCertificate
			)
			{
			bool bres;
			System.Security.Cryptography.X509Certificates.X509Certificate2 certT;
			System.Security.Cryptography.X509Certificates.X509Certificate2 certS;

			OutputTransportCertificate = null;
			OutputSigningCertificate = null;

			X509Certificate2Collection collection = GetСertificatesCollection();
			X509Certificate2Collection fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

			X509Certificate2Collection FinalCollection = fcollection;
			FilterCollection(FinalCollection);

			bool HasOnlyOneCert = FinalCollection.Count == 1 ? true : false;

			// есть только один сертификат
			if (HasOnlyOneCert)
				{
				bres = FindOrSelectCertificate(TransportCertificateThumbprint, out certT);
				if (!bres)
					{
					return false;
					}

				OutputTransportCertificate = certT;
				OutputSigningCertificate = certT;
				return true;
				}

			// есть несколько сертификатов

			bres = FindOrSelectCertificate(TransportCertificateThumbprint, out certT, Title, DescriptionT);
			if (!bres)
				{
				return false;
				}

			bres = FindOrSelectCertificate(SigningCertificateThumbprint, out certS, Title, DescriptionS);
			if (!bres)
				{
				return false;
				}

			OutputTransportCertificate = certT;
			OutputSigningCertificate = certS;

			return true;
			}

		#endregion Выбор сертификатов
		}
	}
