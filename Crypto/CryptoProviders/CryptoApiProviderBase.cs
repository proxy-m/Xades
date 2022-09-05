using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Crypto.CryptoProviders.MicrosoftCryptoApi;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Базовый класс представляющий криптопровайдера с использованием CryptoApi
	/// </summary>
	public abstract class CryptoApiProviderBase : CryptoProviderBase
		{
		/// <summary>
		/// Хэндл криптопровайдера HCRYPTPROV
		/// </summary>
		private SafeCryptProvHandle m_CryptProvHandle = new SafeCryptProvHandle(IntPtr.Zero);

		#region Свойства

		/// <summary>
		/// Хэндл криптопровайдера HCRYPTPROV
		/// </summary>
		public SafeCryptProvHandle CryptoProviderHandle
			{
			get
				{
				return m_CryptProvHandle;
				}
			}

		#endregion Свойства

		#region Вспомогательные функции

		/// <summary>
		/// Метод возвращает свойство криптопровайдера в виде строки
		/// </summary>
		/// <param name="CryptProvHandle">Хэндл криптопровайдера HCRYPTPROV</param>
		/// <param name="dwParam">Идентификатор свойства</param>
		/// <returns></returns>
		public static string GetProvParam(SafeCryptProvHandle CryptProvHandle, uint dwParam)
			{
			bool bres;
			byte[] pbData = new byte[CAPI.MAX_CONTAINER_NAME_LEN + 1];
			uint cbData = new uint();
			cbData = CAPI.MAX_CONTAINER_NAME_LEN;

			using (AutoPinner apcbData = new AutoPinner((object) cbData))
			using (AutoPinner ap = new AutoPinner(pbData))
				{
				bres = CAPI.CAPISafe.CryptGetProvParam(CryptProvHandle, dwParam, ap, apcbData, 0);
				if (!bres)
					{
					Win32Exception ex = GetLastWin32Exception();
					throw ex;
					}
				cbData = (uint) apcbData.Target;
				}

			byte[] NewData = new byte[cbData - 1];
			Array.Copy(pbData, NewData, NewData.Length);
			string s = Encoding.ASCII.GetString(NewData);
			return s;
			}

		/// <summary>
		/// Получить имя провайдера
		/// </summary>
		/// <param name="CryptProvHandle">Хэндл криптопровайдера HCRYPTPROV</param>
		/// <returns></returns>
		public static string GetProviderName(MicrosoftCryptoApi.SafeCryptProvHandle CryptProvHandle)
			{
			return GetProvParam(CryptProvHandle, MicrosoftCryptoApi.CAPI.PP_NAME);
			}

		/// <summary>
		/// Получить имя ключевого контейнера
		/// </summary>
		/// <param name="CryptProvHandle">Хэндл криптопровайдера HCRYPTPROV</param>
		/// <returns></returns>
		public static string GetProviderContainer(MicrosoftCryptoApi.SafeCryptProvHandle CryptProvHandle)
			{
			return GetProvParam(CryptProvHandle, MicrosoftCryptoApi.CAPI.PP_CONTAINER);
			}

		#endregion Вспомогательные функции

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название криптопровайдера</param>
		/// <param name="Container">Название контейнера с приватным ключом</param>
		/// <param name="ProvType">Тип криптопровайдера</param>
		/// <param name="Flags">Флаги создания экземпляра</param>
		public CryptoApiProviderBase(X509Certificate2 Certificate, string Provider, string Container, uint ProvType, uint Flags) : base(Certificate, Provider, Container, ProvType, Flags)
			{
			bool bres;
			bres = CAPI.CryptAcquireContext(ref m_CryptProvHandle, Container, Provider, ProvType, Flags);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			}

		#endregion Конструкторы

		#region Реализация вызываемых методов интерфейса IDisposable

		/// <summary>
		/// Освободить управляемые ресурсы
		/// </summary>
		protected override void DisposeManagedResources()
			{
			if (m_CryptProvHandle != null)
				{
				m_CryptProvHandle.Dispose();
				m_CryptProvHandle = null;
				}
			}

		/// <summary>
		/// Освободить неуправляемые ресурсы
		/// </summary>
		//protected override void DisposeUnmanagedResources()
		//	{
		//	}

		#endregion Реализация вызываемых методов интерфейса IDisposable

		/// <summary>
		/// Создать хэш
		/// </summary>
		/// <param name="AlgID">Идентификатор алгоритма расчета</param>
		/// <param name="HashHandle">Экземпляр хэша</param>
		protected void CreateHash(uint AlgID, ref SafeCryptHashHandle HashHandle)
			{
			bool bres;
			SafeCryptKeyHandle KeyHandle = new MicrosoftCryptoApi.SafeCryptKeyHandle(IntPtr.Zero);

			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptCreateHash(CryptoProviderHandle, AlgID, KeyHandle, 0, ref HashHandle);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			}

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="HashHandle">Экземпляр хэша</param>
		/// <param name="Data">Входной массив байт</param>
		protected void HashData(SafeCryptHashHandle HashHandle, byte[] Data)
			{
			bool bres;
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptHashData(HashHandle, Data, (uint) Data.Length, 0);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			}

		/// <summary>
		/// Создать хэш и посчитать его
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="AlgID">Идентификатор алгоритма расчета</param>
		public SafeCryptHashHandle ComputeHash(byte[] Data, uint AlgID)
			{
			lock (Locker)
				{
				SafeCryptHashHandle HashHandle = new MicrosoftCryptoApi.SafeCryptHashHandle(IntPtr.Zero);

				CreateHash(AlgID, ref HashHandle);
				HashData(HashHandle, Data);

				return HashHandle;
				}
			}

		/// <summary>
		/// Получить хэш в виде массива байт
		/// </summary>
		/// <param name="HashHandle">Хэндл хэша</param>
		/// <param name="HashLength">Длина массива хэша</param>
		/// <returns>Массив байт содержащий хэш</returns>
		public byte[] GetHashValue(SafeCryptHashHandle HashHandle, uint HashLength)
			{
			lock (Locker)
				{
				bool bres;
				byte[] hash = new byte[HashLength];
				uint cbData = new uint();
				cbData = HashLength;

				bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptGetHashParam(HashHandle, MicrosoftCryptoApi.CAPI.HashParameters.HP_HASHVAL, hash, ref cbData, 0);
				if (!bres)
					{
					Win32Exception ex = GetLastWin32Exception();
					throw ex;
					}

				return hash;
				}
			}

		/// <summary>
		/// Получить хэш в виде массива байт
		/// </summary>
		/// <param name="HashHandle">Хэндл хэша</param>
		/// <returns>Массив байт содержащий хэш</returns>
		public byte[] GetHashValue(SafeCryptHashHandle HashHandle)
			{
			lock (Locker)
				{
				bool bres;
				byte[] HashLength = new byte[4];
				uint cbHashLength = new uint();
				cbHashLength = 4;

				bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptGetHashParam(HashHandle, MicrosoftCryptoApi.CAPI.HashParameters.HP_HASHSIZE, HashLength, ref cbHashLength, 0);
				if (!bres)
					{
					Win32Exception ex = GetLastWin32Exception();
					throw ex;
					}

				int iHashLength = BitConverter.ToInt32(HashLength, 0);
				byte[] hash = new byte[iHashLength];
				uint cbData = new uint();
				cbData = (uint) iHashLength;

				bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptGetHashParam(HashHandle, MicrosoftCryptoApi.CAPI.HashParameters.HP_HASHVAL, hash, ref cbData, 0);
				if (!bres)
					{
					Win32Exception ex = GetLastWin32Exception();
					throw ex;
					}

				return hash;
				}
			}

		/// <summary>
		/// Установить ранее посчитанное значение Hash в объект HashHandle
		/// </summary>
		/// <param name="HashHandle">Хэндл хэша</param>
		/// <param name="HashData">Ранее посчитанное значение Hash</param>
		public static void SetHash(SafeCryptHashHandle HashHandle, byte[] HashData)
			{
			bool bres;
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptSetHashParam(HashHandle, CAPI.HashParameters.HP_HASHVAL, HashData, 0);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public MicrosoftCryptoApi.SafeCryptKeyHandle GetUserKey()
			{
			bool bres;
			MicrosoftCryptoApi.SafeCryptKeyHandle KeyHandle = new MicrosoftCryptoApi.SafeCryptKeyHandle(IntPtr.Zero);
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptGetUserKey(CryptoProviderHandle, MicrosoftCryptoApi.CAPI.AT_KEYEXCHANGE, ref KeyHandle);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			return KeyHandle;
			}

		/// <summary>
		/// Проверить подпись
		/// </summary>
		/// <param name="hHash">хэндл хэша</param>
		/// <param name="Signature">Подпись</param>
		/// <returns></returns>
		public bool VerifySignature(SafeCryptHashHandle hHash, byte[] Signature)
			{
			bool bres;
			MicrosoftCryptoApi.SafeCryptKeyHandle PublicKey;
			PublicKey = GetUserKey();
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptVerifySignature(hHash, Signature, (uint) Signature.Length, PublicKey, IntPtr.Zero, 0);
			if (!bres)
				{
				Win32Exception ex = GetLastWin32Exception();
				throw ex;
				}
			return true;
			}

		/// <summary>
		/// Посчитать подпись хэша
		/// </summary>
		/// <param name="HashHandle"></param>
		/// <returns></returns>
		public static byte[] SignHash(SafeCryptHashHandle HashHandle)
			{
			bool bres;
			uint dwSigLen = new uint();
			dwSigLen = 0;

			// Определяем размер
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptSignHash(HashHandle, MicrosoftCryptoApi.CAPI.AT_KEYEXCHANGE, IntPtr.Zero, 0, null, ref dwSigLen);
			if (!bres)
				{
				Win32Exception ex = CryptoProviderBase.GetLastWin32Exception();
				throw ex;
				}

			byte[] Signature = new byte[dwSigLen];

			// Считаем подпись
			bres = MicrosoftCryptoApi.CAPI.CAPIUnsafe.CryptSignHash(HashHandle, MicrosoftCryptoApi.CAPI.AT_KEYEXCHANGE, IntPtr.Zero, 0, Signature, ref dwSigLen);
			if (!bres)
				{
				Win32Exception ex = CryptoProviderBase.GetLastWin32Exception();
				throw ex;
				}

			return Signature;
			}

		/// <summary>
		/// Посчитать хэш и подпись к нему
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="Hash">хэш</param>
		/// <param name="Signature">подпись хэша</param>
		/// <returns>true если все хорошо</returns>
		protected bool InternalComputeHashAndSignature(byte[] Data, uint AlgID, out byte[] Hash, out byte[] Signature)
			{
			SafeCryptHashHandle HashHandle = new MicrosoftCryptoApi.SafeCryptHashHandle(IntPtr.Zero);
			SafeCryptHashHandle VerifyHashHandle = new MicrosoftCryptoApi.SafeCryptHashHandle(IntPtr.Zero);
			try
				{
				lock (Locker)
					{
					Hash = null;
					Signature = null;

					CreateHash(AlgID, ref HashHandle);
					HashData(HashHandle, Data);
					Hash = GetHashValue(HashHandle);
					Signature = SignHash(HashHandle);

					// Проверка подписи
					CreateHash(AlgID, ref VerifyHashHandle);
					SetHash(VerifyHashHandle, Hash);
					bool bres = VerifySignature(VerifyHashHandle, Signature);
					return bres;
					} // end lock
				}
			finally
				{
				if (HashHandle != null)
					{
					HashHandle.Dispose();
					}
				if (VerifyHashHandle != null)
					{
					VerifyHashHandle.Dispose();
					}
				}
			}
		}
	}
