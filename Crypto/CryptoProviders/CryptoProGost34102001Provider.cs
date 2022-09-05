using System;
using Crypto.CryptoProviders.MicrosoftCryptoApi;
using System.Security.Cryptography.X509Certificates;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Класс представляющий криптопровайдера с использованием КриптоПро Crypto-Pro GOST R 34.10-2001
	/// Cryptographic Service Provider
	/// </summary>
	public class CryptoProGost34102001Provider : CryptoApiProviderBase
		{
		/// <summary>
		/// Алгоритм хэширования в соответствии с ГОСТ Р 34.11-94
		/// </summary>
		private const int CALG_GR3411 = 32798;

		/// <summary>
		/// Длина хэша в байтах
		/// </summary>
		private const int GR3411LEN = 32;

		#region Константы инициализации

		/// <summary>
		/// Название провайдера в реестре
		/// </summary>
		public const string InitProviderName = "Crypto-Pro GOST R 34.10-2001 Cryptographic Service Provider";

		/// <summary>
		/// Идентификатор провайдера в реестре
		/// </summary>
		public const uint InitProviderType = 75;

		#endregion Константы инициализации

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Container">The key container name</param>
		/// <param name="Flags">Flag values. This parameter is usually set to zero</param>
		public CryptoProGost34102001Provider(X509Certificate2 Certificate, string Container, uint Flags) : base(Certificate, InitProviderName, Container, InitProviderType, Flags)
			{
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		public CryptoProGost34102001Provider(X509Certificate2 Certificate) : this(Certificate, "", MicrosoftCryptoApi.CAPI.CRYPT_VERIFYCONTEXT)
			{
			}

		#endregion Конструкторы

		#region Реализация интерфейса ICryptoProvider

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>Хэш если все хорошо</returns>
		public override byte[] ComputeHash(byte[] Data)
			{
			SafeCryptHashHandle HashHandle = null;
			try
				{
				HashHandle = ComputeHash(Data, CALG_GR3411);
				byte[] Hash = GetHashValue(HashHandle, GR3411LEN);
				return Hash;
				}
			finally
				{
				if (HashHandle != null)
					{
					HashHandle.Dispose();
					}
				}
			}


		/// <summary>
		/// Посчитать подпись хэша
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>подпись в виде массива байт</returns>
		public override byte[] ComputeSignature(byte[] Data)
			{
			SafeCryptHashHandle HashHandle = new MicrosoftCryptoApi.SafeCryptHashHandle(IntPtr.Zero);
			try
				{
				CreateHash(CALG_GR3411, ref HashHandle);
				SetHash(HashHandle, Data);
				byte[] Signature;
				Signature = SignHash(HashHandle);
				return Signature;
				}
			finally
				{
				if (HashHandle != null)
					{
					HashHandle.Dispose();
					}
				}
			}


		/// <summary>
		/// Посчитать хэш и подпись к нему
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="Hash">Хэш</param>
		/// <param name="Signature">Подпись хэша</param>
		/// <returns>true если все хорошо</returns>
		public override bool ComputeHashAndSignature(byte[] Data, out byte[] Hash, out byte[] Signature)
			{
			Hash = null;
			Signature = null;
			return InternalComputeHashAndSignature(Data, CALG_GR3411,out Hash, out Signature);
			}


		/// <summary>
		/// Нужно ли переворачивать подпись 
		/// .Net криптопровайдер и CryptoApi провайдеры ведут себя противоположным образом 
		/// </summary>
		public override bool ReverseSignature
			{
			get
				{
				return true;
				}
			}

		#endregion Реализация интерфейса ICryptoProvider
		}
	}
