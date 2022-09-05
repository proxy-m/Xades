using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Базовый класс представляющий криптопровайдера
	/// </summary>
	public abstract class CryptoProviderBase : IDisposable, ICryptoProvider
		{
		/// <summary>
		/// Уникальный идентификатор провайдера
		/// </summary>
		private readonly CryptoProviderUniqueIdentifier m_ProviderUniqueIdentifier;

		/// <summary>
		/// Локер
		/// </summary>
		private object m_Locker = new object();

		/// <summary>
		/// Сертификат
		/// </summary>
		private readonly X509Certificate2 m_Certificate;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="ProviderUniqueIdentifier">Уникальный идентификатор криптопровайдера</param>
		public CryptoProviderBase(X509Certificate2 Certificate, CryptoProviderUniqueIdentifier ProviderUniqueIdentifier)
			{
			this.m_ProviderUniqueIdentifier = ProviderUniqueIdentifier;
			this.m_Certificate = Certificate;
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		/// <param name="Container">Название контейнера</param>
		/// <param name="ProvType">Тип провайдера</param>
		/// <param name="Flags">Флаги создания провайдера</param>
		public CryptoProviderBase(X509Certificate2 Certificate, string Provider, string Container, uint ProvType, uint Flags)
			{
			CryptoProviderUniqueIdentifier cuid = new CryptoProviderUniqueIdentifier(Provider, Container, ProvType, Flags);
			this.m_ProviderUniqueIdentifier = cuid;
			this.m_Certificate = Certificate;
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		/// <param name="Container">Название контейнера</param>
		public CryptoProviderBase(X509Certificate2 Certificate, string Provider, string Container) : this(Certificate, Provider, Container, 0, 0)
			{
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		/// <param name="ProvType">Тип провайдера</param>
		public CryptoProviderBase(X509Certificate2 Certificate, string Provider, uint ProvType) : this(Certificate, Provider, null, ProvType, 0)
			{
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		public CryptoProviderBase(X509Certificate2 Certificate, string Provider) : this(Certificate, Provider, null, 0, 0)
			{
			}

		#endregion Конструкторы

		#region Реализация интерфейса IDisposable

		/// <summary>
		/// Флаг для отслеживания того, что Dispose уже вызывался
		/// </summary>
		private bool m_Disposed;

		/// <summary>
		/// Свойство для флага m_Disposed
		/// </summary>
		protected bool Disposed
			{
			get
				{
				return m_Disposed;
				}

			private set
				{
				m_Disposed = value;
				}
			}

		/// <summary>
		/// Реализация Dispose()
		/// </summary>
		public void Dispose()
			{
			Dispose(true);

			// Объект будет освобожден при помощи метода Dispose, следовательно необходимо вызвать
			// GC.SupressFinalize чтобы убрать этот объект из очереди финализатора и предотвратить
			// вызов финализатора во второй раз
			GC.SuppressFinalize(this);
			}

		/// <summary>
		/// Метод Dispose(bool disposing) выполняется в двух различных вариантах: Если
		/// disposing=true, то этот метод вызывается (прямо или косвенно) из пользовательского кода.
		/// Управляемые и неуправляемые ресурсы могут быть освобождены. Если disposing=false, то этот
		/// метод вызывается рантаймом из финализатора и могут освобождаться только неуправляемые ресурсы.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
			{
			// Проверка что Dispose(bool) уже вызывался
			if (this.Disposed)
				{
				return;
				}

			// Если disposing=true, то освобождаем все (управляемые и неуправляемые) ресурсы
			if (disposing)
				{
				// Освобождаем управляемые ресурсы
				DisposeManagedResources();
				}

			// Вызываем соответствующие методы для освобождения неуправляемых ресурсов
			DisposeUnmanagedResources();

			// Ставим флаг завершения
			Disposed = true;
			}

		#endregion Реализация интерфейса IDisposable

		#region Реализация вызыываемых методов интерфейса IDisposable

		/// <summary>
		/// Освободить управляемые ресурсы
		/// </summary>
		protected virtual void DisposeManagedResources()
			{
			}

		/// <summary>
		/// Освободить неуправляемые ресурсы
		/// </summary>
		protected virtual void DisposeUnmanagedResources()
			{
			}

		#endregion Реализация вызыываемых методов интерфейса IDisposable

		#region Вспомогательные функции

		/// <summary>
		/// Возвращает последний код ошибки
		/// </summary>
		/// <returns>последний код ошибки</returns>
		public static int GetLastWin32Error()
			{
			return Marshal.GetLastWin32Error();
			}

		/// <summary>
		/// Возвращает последний код ошибки в виде исключения
		/// </summary>
		/// <returns>последний код ошибки в виде исключения</returns>
		public static Win32Exception GetLastWin32Exception()
			{
			int Error = GetLastWin32Error();
			Win32Exception ex = new Win32Exception(Error);
			return ex;
			}

		/// <summary>
		/// Сравнить два байтовых массива
		/// </summary>
		/// <param name="b1">Первый байтовый массив</param>
		/// <param name="b2">Второй байтовый массив</param>
		/// <param name="Length">Длина сравнения</param>
		/// <returns>true если равны</returns>
		public static bool Compare(byte[] b1, byte[] b2, int Length)
			{
			bool b;
			string s1 = System.Text.Encoding.ASCII.GetString(b1);
			string s2 = System.Text.Encoding.ASCII.GetString(b2, 0, Length);

			b = (s1 == s2);

			//if (!b)
			//	{
			//	Debug.Assert(b);
			//	}
			return b;
			}

		/// <summary>
		/// Отконвертировать строку в ASCII и далее в набор байт
		/// </summary>
		/// <param name="str">Строка</param>
		/// <returns></returns>
		public static byte[] StringToBytes(string str)
			{
			if (str == null)
				{
				throw new ArgumentNullException("str");
				}

			byte[] toBytes = Encoding.ASCII.GetBytes(str);
			return toBytes;
			}

		/// <summary>
		/// Отконвертировать набор байт в строку
		/// </summary>
		/// <param name="ba">набор байт</param>
		/// <returns></returns>
		public static string BytesToString(byte[] ba)
			{
			if (ba == null)
				{
				throw new ArgumentNullException("ba");
				}

			string hex = BitConverter.ToString(ba);
			return hex.Replace("-", "");
			}

		/// <summary>
		/// Перевернуть массив байт
		/// </summary>
		/// <param name="ba">набор байт</param>
		/// <returns></returns>
		public static byte[] ReverseArray(byte[] ba)
			{
			byte[] result = new byte[ba.Length];
			Array.Copy(ba, result, ba.Length);
			Array.Reverse(result);
			return result;
			}

		#endregion Вспомогательные функции

		#region Свойства

		/// <summary>
		/// Локер
		/// </summary>
		protected object Locker
			{
			get
				{
				return m_Locker;
				}
			}

		public CryptoProviderUniqueIdentifier ProviderUniqueIdentifier
			{
			get
				{
				return m_ProviderUniqueIdentifier;
				}
			}

		#endregion Свойства

		#region Реализация интерфейса ICryptoProvider

		/// <summary>
		/// Посчитать хэш
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>хэш в виде массива байт</returns>
		public virtual byte[] ComputeHash(byte[] Data)
			{
			throw new NotImplementedException();
			}

		/// <summary>
		/// Посчитать подпись хэша
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>подпись в виде массива байт</returns>
		public virtual byte[] ComputeSignature(byte[] Data)
			{
			throw new NotImplementedException();
			}

		/// <summary>
		/// Посчитать хэш и подпись к нему
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <param name="Hash">Хэш</param>
		/// <param name="Signature">Подпись хэша</param>
		/// <returns>true если все хорошо</returns>
		public virtual bool ComputeHashAndSignature(byte[] Data, out byte[] Hash, out byte[] Signature)
			{
			throw new NotImplementedException();
			}

		#endregion Реализация интерфейса ICryptoProvider

		#region Прочие функции

		/// <summary>
		/// Посчитать хэш и вернуть его как строку
		/// </summary>
		/// <param name="Data">Входной массив байт</param>
		/// <returns>хэш в виде строки</returns>
		public string ComputeHashAndGetAsString(byte[] Data)
			{
			byte[] Hash = ComputeHash(Data);
			string s = BytesToString(Hash);
			return s;
			}

		/// <summary>
		/// Сертификат соответствующий открытому контейнеру
		/// </summary>
		public X509Certificate2 Certificate
			{
			get
				{
				return m_Certificate;
				}
			}

		/// <summary>
		/// Нужно ли переворачивать подпись .Net криптопровайдер и CryptoApi провайдеры ведут себя
		/// противоположным образом
		/// </summary>
		public abstract bool ReverseSignature
			{
			get;

			//{
			//throw new NotImplementedException();
			//}
			}

		#endregion Прочие функции
		}
	}
