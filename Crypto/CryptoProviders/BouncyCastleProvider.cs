using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.X509;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Класс представляющий криптопровайдера с использованием BouncyCastle 
	/// </summary>
	public class BouncyCastleProvider : CryptoProviderBase
		{

		#region Конструктор
		public BouncyCastleProvider() : base("BouncyCastle")
			{
			} 
		#endregion // Конструктор

		#region Полезные функции

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Массив байт</param>
		/// <returns></returns>
		public override byte[] ComputeHash(byte[] Data)
			{
			Org.BouncyCastle.Crypto.Digests.Gost3411Digest digEng = new Org.BouncyCastle.Crypto.Digests.Gost3411Digest();

			digEng.BlockUpdate(Data, 0, Data.Length);

			byte[] digest = new byte[digEng.GetDigestSize()];
			digEng.DoFinal(digest, 0);
			return digest;
			}

		#endregion // Полезные функции
		}

	}
