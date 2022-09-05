using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Класс представляющий криптопровайдера с использованием .Net Framework
	/// </summary>
	public class NetFrameworkProvider : CryptoProviderBase
		{
		#region Конструктор

		public NetFrameworkProvider(X509Certificate2 Certificate) : base(Certificate, "NetFrameworkProvider")
			{
			}

		#endregion Конструктор

		#region Реализация интерфейса ICryptoProvider

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Массив байт</param>
		/// <returns></returns>
		public override byte[] ComputeHash(byte[] Data)
			{
			HashAlgorithm pkHash = HashAlgorithm.Create("GOST3411");
			byte[] hashValue = pkHash.ComputeHash(Data);
			return hashValue;
			}

		#endregion Реализация интерфейса ICryptoProvider

		#region Реализация интерфейса ICryptoProvider

		/// <summary>
		/// Посчитать подпись хэша
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>подпись в виде массива байт</returns>
		public override byte[] ComputeSignature(byte[] Data)
			{
			SignatureDescription description;
			description = null;

			const string signatureMethod = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";
			description = CryptoConfig.CreateFromName(signatureMethod) as SignatureDescription;

			AsymmetricSignatureFormatter asf = description.CreateFormatter(Certificate.PrivateKey);
			byte[] bSignature;
			bSignature = asf.CreateSignature(Data);
			return bSignature;
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

			Hash = ComputeHash(Data);
			Signature = ComputeSignature(Hash);
			return true;
			}

		/// <summary>
		/// Нужно ли переворачивать подпись .Net криптопровайдер и CryptoApi провайдеры ведут себя
		/// противоположным образом
		/// </summary>
		public override bool ReverseSignature
			{
			get
				{
				return false;
				}
			}

		#endregion Реализация интерфейса ICryptoProvider
		}
	}
