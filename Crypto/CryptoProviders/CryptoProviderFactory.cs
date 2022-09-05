using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Фабрика криптопровайдеров CryptoAPI
	/// </summary>
	public class CryptoProviderFactory
		{
		#region Статические мемберы

		/// <summary>
		/// Экземпляр синглтона
		/// </summary>
		private static CryptoProviderFactory m_Instance = null;

		/// <summary>
		/// Статический локер
		/// </summary>
		private static object StaticLocker = new object();

		#endregion Статические мемберы

		#region Мемберы

		/// <summary>
		/// Локер
		/// </summary>
		private object Locker = new object();

		/// <summary>
		/// Набор открытых контейнеров
		/// </summary>
		private Dictionary<CryptoProviderUniqueIdentifier, CryptoProviderBase> m_Instances = new Dictionary<CryptoProviderUniqueIdentifier, CryptoProviderBase>();

		/// <summary>
		/// Кэш для быстрого поиска открытого провайдера по сертификату
		/// </summary>
		private Dictionary<X509Certificate2, ICryptoProvider> m_InstancesCache = new Dictionary<X509Certificate2, ICryptoProvider>();

		#endregion	Мемберы

		#region Статические методы

		/// <summary>
		/// Получить экземпляр фабрики
		/// </summary>
		/// <returns></returns>
		public static CryptoProviderFactory GetInstance()
			{
			lock (StaticLocker)
				{
				if (m_Instance == null)
					{
					m_Instance = new CryptoProviderFactory();
					}
				return m_Instance;
				}
			}

		/// <summary>
		/// Получить информацию о контейнере ключа и прочее
		/// </summary>
		/// <param name="certificate">Сертификат</param>
		/// <returns></returns>
		public static MicrosoftCryptoApi.CAPI.CRYPT_KEY_PROV_INFO GetKeyContainerInformation(X509Certificate2 certificate)
			{
			bool bres;
			MicrosoftCryptoApi.SafeCertContextHandle CertContextHandle = new MicrosoftCryptoApi.SafeCertContextHandle(certificate.Handle);
			MicrosoftCryptoApi.SafeLocalAllocHandle ZeroPtr = new MicrosoftCryptoApi.SafeLocalAllocHandle(IntPtr.Zero);
			uint cbData = new uint();
			cbData = 0;
			bres = MicrosoftCryptoApi.CAPI.CAPISafe.CertGetCertificateContextProperty(CertContextHandle, MicrosoftCryptoApi.CAPI.CERT_KEY_PROV_INFO_PROP_ID, ZeroPtr, ref cbData);
			if (!bres)
				{
				Win32Exception ex = CryptoProviderBase.GetLastWin32Exception();
				throw ex;
				}

			MicrosoftCryptoApi.SafeLocalAllocHandle Ptr;
			Ptr = MicrosoftCryptoApi.CAPI.CAPISafe.LocalAlloc(0, new IntPtr(cbData));

			bres = MicrosoftCryptoApi.CAPI.CAPISafe.CertGetCertificateContextProperty(CertContextHandle, MicrosoftCryptoApi.CAPI.CERT_KEY_PROV_INFO_PROP_ID, Ptr, ref cbData);
			if (!bres)
				{
				Win32Exception ex = CryptoProviderBase.GetLastWin32Exception();
				throw ex;
				}

			MicrosoftCryptoApi.CAPI.CRYPT_KEY_PROV_INFO KeyProvInfo = (MicrosoftCryptoApi.CAPI.CRYPT_KEY_PROV_INFO) Marshal.PtrToStructure(Ptr.DangerousGetHandle(), typeof(MicrosoftCryptoApi.CAPI.CRYPT_KEY_PROV_INFO));

			MicrosoftCryptoApi.CAPI.CAPISafe.LocalFree(Ptr.DangerousGetHandle());
			Ptr.SetHandleAsInvalid();

			return KeyProvInfo;
			}

		#endregion Статические методы

		#region CryptoProviderCPB

		/// <summary>
		/// Получить экземпляр криптопровайдера из кэша
		/// </summary>
		/// <param name="Provider">Название криптопровайдера</param>
		/// <param name="Container">Название контейнера с приватным ключом</param>
		/// <param name="ProvType">Тип криптопровайдера</param>
		/// <param name="Flags">Флаги создания экземпляра</param>
		/// <returns></returns>
		private CryptoProviderBase GetCryptoProviderCPB(X509Certificate2 Certificate, string Provider, string Container, uint ProvType, uint Flags)
			{
			lock (Locker)
				{
				CryptoProviderUniqueIdentifier UniqueIdentifier = new CryptoProviderUniqueIdentifier(Provider, Container, ProvType, Flags);
				if (m_Instances.ContainsKey(UniqueIdentifier))
					{
					return m_Instances[UniqueIdentifier];
					}

				CryptoProviderBase cpb = CreateCryptoProvider(Certificate, Provider, Container, ProvType, Flags);
				m_Instances.Add(UniqueIdentifier, cpb);
				return cpb;
				}
			}

		/// <summary>
		/// Получить экземпляр криптопровайдера из кэша
		/// </summary>
		/// <param name="Certificate">Сертификат</param>
		/// <param name="Flags">Флаги создания экземпляра</param>
		/// <returns></returns>
		public CryptoProviderBase GetCryptoProviderCPB(X509Certificate2 Certificate, uint Flags)
			{
			MicrosoftCryptoApi.CAPI.CRYPT_KEY_PROV_INFO KeyContainerInformation;
			KeyContainerInformation = GetKeyContainerInformation(Certificate);
			CryptoProviderBase cpb = GetCryptoProviderCPB(Certificate, KeyContainerInformation.pwszProvName, KeyContainerInformation.pwszContainerName, KeyContainerInformation.dwProvType, Flags);
			return cpb;
			}

		#endregion CryptoProviderCPB

		#region GetCryptoProvider

		/// <summary>
		/// Получить экземпляр криптопровайдера из кэша
		/// </summary>
		/// <param name="Certificate">Сертификат</param>
		/// <param name="Flags">Флаги создания экземпляра</param>
		/// <returns></returns>
		public ICryptoProvider GetCryptoProvider(X509Certificate2 Certificate, uint Flags)
			{
			lock (Locker)
				{
				if (m_InstancesCache.ContainsKey(Certificate)) // первый поиск
					{
					return m_InstancesCache[Certificate];
					}

				CryptoProviderBase cpb = GetCryptoProviderCPB(Certificate, Flags);
				if (cpb != null)
					{
					ICryptoProvider icp = cpb;
					m_InstancesCache.Add(Certificate, icp);
					return icp;
					}
				return null;
				} // end lock
			}

		/// <summary>
		/// Получить экземпляр криптопровайдера из кэша
		/// </summary>
		/// <param name="Certificate">Сертификат</param>
		/// <returns></returns>
		public ICryptoProvider GetCryptoProvider(X509Certificate2 Certificate)
			{
			return GetCryptoProvider(Certificate, 0);
			}

		#endregion GetCryptoProvider

		/// <summary>
		/// Создать экземпляр криптопровайдера
		/// </summary>
		/// <param name="Provider">Название криптопровайдера</param>
		/// <param name="Container">Название контейнера с приватным ключом</param>
		/// <param name="ProvType">Тип криптопровайдера</param>
		/// <param name="Flags">Флаги создания экземпляра</param>
		/// <returns></returns>
		private CryptoProviderBase CreateCryptoProvider(X509Certificate2 Certificate, string Provider, string Container, uint ProvType, uint Flags)
			{
			CryptoProviderBase cpb = null;

			if (Container == null)
				{
				Container = string.Empty;
				}

			#region КриптоПро

			if (
				(Provider == CryptoProGost34102001Provider.InitProviderName) &&
				(ProvType == CryptoProGost34102001Provider.InitProviderType)
				)
				{
				cpb = new CryptoProGost34102001Provider(Certificate, Container, Flags);
				return cpb;
				}

			#endregion КриптоПро

			#region Инфотекс

			if (
				(Provider == InfotecsProvider.InitProviderName) &&
				(ProvType == InfotecsProvider.InitProviderType)
				)
				{
				cpb = new InfotecsProvider(Certificate, Container, Flags);
				return cpb;
				}

			#endregion Инфотекс

			throw new NotImplementedException();

			//return null;
			} // end
		}
	}
