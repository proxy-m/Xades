using System.Security.Cryptography.X509Certificates;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Интерфейс абстрактного криптопровайдера
	/// </summary>
	public interface ICryptoProvider
		{
		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>хэш в виде массива байт</returns>
		byte[] ComputeHash(byte[] Data);

		/// <summary>
		/// Посчитать подпись хэша
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>подпись в виде массива байт</returns>
		byte[] ComputeSignature(byte[] Data);

		/// <summary>
		/// Посчитать хэш и подпись к нему
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="Hash">Хэш</param>
		/// <param name="Signature">Подпись хэша</param>
		/// <returns>true если все хорошо</returns>
		bool ComputeHashAndSignature(byte[] Data, out byte[] Hash, out byte[] Signature);

		/// <summary>
		/// Сертификат соответствующий открытому контейнеру
		/// </summary>
		X509Certificate2 Certificate
			{
			get;
			}

		/// <summary>
		/// Нужно ли переворачивать подпись .Net криптопровайдер и CryptoApi провайдеры ведут себя
		/// противоположным образом
		/// </summary>
		bool ReverseSignature
			{
			get;
			}
		}
	}
