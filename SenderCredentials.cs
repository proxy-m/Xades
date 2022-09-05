using System;
using GisBusted.Helpers;

namespace GisBusted
	{
	/// <summary>
	/// Вспомогательный класс, несет в себе атрибуты отправителя для подписи запроса
	/// </summary>
	public sealed class SenderCredentials : IComparable<SenderCredentials>
		{
		/// <summary>
		/// Идентификатор поставщика данных (SenderID - уже не используется) или Идентификатор
		/// зарегистрированной организации (orgPPAGUID)
		/// </summary>
		private readonly string m_Credential;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Credential">Идентификатор зарегистрированной организации (orgPPAGUID)</param>
		public SenderCredentials(string Credential)
			{
			if (Credential == null)
				{
				throw new ArgumentNullException("Credential", "Credential не может быть null");
				}

			m_Credential = Credential.LanitGuid(); // На всякий случай переделываем GUID
			}

		#endregion Конструкторы

		#region Реализация интерфейса IComparable

		/// <summary>
		/// Функция сравнения
		/// </summary>
		/// <param name="other">Экземпляр для сравнения</param>
		/// <returns></returns>
		public int CompareTo(SenderCredentials other)
			{
			return m_Credential.CompareTo(other.Credential);
			}

		#endregion Реализация интерфейса IComparable

		#region Свойства

		/// <summary>
		/// Идентификатор зарегистрированной организации
		/// </summary>
		public string orgPPAGUID
			{
			get
				{
				if (IsorgPPAGUID)
					{
					return m_Credential;
					}
				throw new InvalidOperationException("Это не orgPPAGUID");
				}
			}

		/// <summary>
		/// Идентификатор зарегистрированной организации
		/// </summary>
		public bool IsorgPPAGUID
			{
			get
				{
				return true;
				}
			}

		/// <summary>
		/// Мандат - идентификатор поставщика данных (SenderID) или идентификатор зарегистрированной
		/// организации (orgPPAGUID)
		/// </summary>
		public string Credential
			{
			get
				{
				if (m_Credential == null)
					{
					throw new InvalidOperationException("m_Credential = null");
					}
				return m_Credential;
				}
			}

		#endregion Свойства

		#region Вспомогательные методы

		#endregion Вспомогательные методы
		}
	}
