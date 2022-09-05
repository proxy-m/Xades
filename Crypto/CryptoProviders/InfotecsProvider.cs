using System.Security.Cryptography.X509Certificates;
using Crypto.CryptoProviders.MicrosoftCryptoApi;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Класс представляющий криптопровайдера с использованием Infotecs Cryptographic Service Provider
	/// </summary>
	public class InfotecsProvider : CryptoApiProviderBase
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
		public const string InitProviderName = "Infotecs Cryptographic Service Provider";

		/// <summary>
		/// Идентификатор провайдера в реестре
		/// </summary>
		public const uint InitProviderType = 2;

		#endregion Константы инициализации

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Container">The key container name</param>
		/// <param name="Flags">Flag values. This parameter is usually set to zero</param>
		public InfotecsProvider(X509Certificate2 Certificate, string Container, uint Flags) : base(Certificate, InitProviderName, Container, InitProviderType, Flags)
			{
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		public InfotecsProvider(X509Certificate2 Certificate) : this(Certificate, "", MicrosoftCryptoApi.CAPI.CRYPT_VERIFYCONTEXT)
			{
			}

		#endregion Конструкторы

		#region Реализация интерфейса ICryptoProvider

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns></returns>
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
		/// Посчитать хэш и подпись к нему
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="Hash">хэш</param>
		/// <param name="Signature">подпись хэша</param>
		/// <returns>true если все хорошо</returns>
		public override bool ComputeHashAndSignature(byte[] Data, out byte[] Hash, out byte[] Signature)
			{
			Hash = null;
			Signature = null;
			return InternalComputeHashAndSignature(Data, CALG_GR3411, out Hash, out Signature);
			}

		/// <summary>
		/// Нужно ли переворачивать подпись .Net криптопровайдер и CryptoApi провайдеры ведут себя
		/// противоположным образом
		/// </summary>
		public override bool ReverseSignature
			{
			get
				{
				// throw new NotImplementedException(); надо проверять
				return true;
				}
			}

		#endregion Реализация интерфейса ICryptoProvider
		}
	}
