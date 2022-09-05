using System;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Уникальный идентификатор криптопровайдера
	/// </summary>
	public class CryptoProviderUniqueIdentifier : IEquatable<CryptoProviderUniqueIdentifier>
		{
		/// <summary>
		/// Название провайдера
		/// </summary>
		private readonly string m_Provider;

		/// <summary>
		/// Название контейнера
		/// </summary>
		private readonly string m_Container;

		/// <summary>
		/// Тип провайдера
		/// </summary>
		private readonly uint m_ProvType;

		/// <summary>
		/// Флаги создания провайдера
		/// </summary>
		private readonly uint m_Flags;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		/// <param name="Container">Название контейнера</param>
		/// <param name="ProvType">Тип провайдера</param>
		/// <param name="Flags">Флаги создания провайдера</param>
		public CryptoProviderUniqueIdentifier(string Provider, string Container, uint ProvType, uint Flags)
			{
			if (Provider == null)
				{
				throw new ArgumentNullException("Provider");
				}

			if (Container == null)
				{
				Container = string.Empty;
				}

			this.m_Provider = Provider;
			this.m_Container = Container;
			this.m_ProvType = ProvType;
			this.m_Flags = Flags;
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		/// <param name="Container">Название контейнера</param>
		public CryptoProviderUniqueIdentifier(string Provider, string Container) : this(Provider, Container, 0, 0)
			{
			}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Provider">Название провайдера</param>
		public CryptoProviderUniqueIdentifier(string Provider) : this(Provider, null, 0, 0)
			{
			}

		#endregion Конструкторы

		#region Реализация интерфейса IEquatable

		public bool Equals(CryptoProviderUniqueIdentifier other)
			{
			if (other == null)
				{
				return false;
				}

			return
				other.Provider.Equals(Provider) &&
				other.ProvType.Equals(ProvType) &&
				other.Container.Equals(Container) &&
				other.Flags.Equals(Flags)
				;
			}

		#endregion Реализация интерфейса IEquatable

		#region Вспомогательные функции

		public override bool Equals(object other)
			{
			if (other == null)
				{
				return false;
				}

			var с = other as CryptoProviderUniqueIdentifier;

			if (с == null)
				{
				return false;
				}

			return Equals(с);
			}

		public override int GetHashCode()
			{
			return this.Provider.GetHashCode() ^ this.ProvType.GetHashCode() ^ this.Container.GetHashCode();
			}

		public override string ToString()
			{
			string s = null;
			if (!string.IsNullOrEmpty(Container))
				{
				s = string.Format("Provider='{0}', Container='{1}', ProvType={2}", Provider, Container, ProvType);
				}
			else
				{
				s = string.Format("Provider='{0}', ProvType={1}", Provider, ProvType);
				}
			return s;
			}

		#endregion Вспомогательные функции

		#region Свойства

		/// <summary>
		/// Название провайдера
		/// </summary>
		public string Provider
			{
			get
				{
				return m_Provider;
				}
			}

		/// <summary>
		/// Название контейнера
		/// </summary>
		public string Container
			{
			get
				{
				return m_Container;
				}
			}

		/// <summary>
		/// Тип провайдера
		/// </summary>
		public uint ProvType
			{
			get
				{
				return m_ProvType;
				}
			}

		/// <summary>
		/// Флаги создания провайдера
		/// </summary>
		public uint Flags
			{
			get
				{
				return m_Flags;
				}
			}

		#endregion Свойства

		#region Операторы

		public static bool operator ==(CryptoProviderUniqueIdentifier s, CryptoProviderUniqueIdentifier s2)
			{
			if (((object) s) == null || ((object) s2) == null)
				{
				return Object.Equals(s, s2);
				}

			return s2.Equals(s);
			}

		public static bool operator !=(CryptoProviderUniqueIdentifier s, CryptoProviderUniqueIdentifier s2)
			{
			if (((object) s) == null || ((object) s2) == null)
				{
				return !Object.Equals(s, s2);
				}

			return !s2.Equals(s);
			}

		#endregion Операторы
		}
	}
