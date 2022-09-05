using System;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Вспомогательный класс для аутентификации на тестовом сервере
	/// </summary>
	public static class BasicAuthenticationHelper
		{
		/// <summary>
		/// Заголовок авторизации
		/// </summary>
		public const string cAuthorization = "Authorization";

		/// <summary>
		/// Имя пользователя
		/// </summary>
		private static string s_UserName;

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		private static string s_UserPassword;

		/// <summary>
		/// Собранная строка
		/// </summary>
		private static string s_CompleteString;

		/// <summary>
		/// Установить имя пользователя и пароль
		/// </summary>
		/// <param name="UserName">Имя пользователя</param>
		/// <param name="UserPassword">Пароль пользователя</param>
		public static void SetUserNameAndPassword(string UserName, string UserPassword)
			{
			s_UserName = UserName;
			s_UserPassword = UserPassword;
			}

		/// <summary>
		/// Есть ли данные (имя пользователя и пароль) для вставки в заголовок аутентификации
		/// </summary>
		public static bool HasAuthenticationData
			{
			get
				{
				bool bres = (!string.IsNullOrEmpty(s_UserName)) && (!string.IsNullOrEmpty(s_UserPassword));
				return bres;
				}
			}

		/// <summary>
		/// Получить строку аутентификации
		/// </summary>
		/// <returns>Строка аутентификации</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetBasicAuthenticationHeader()
			{
			if (string.IsNullOrEmpty(s_CompleteString))
				{
				string authInfo = s_UserName + ":" + s_UserPassword;
				byte[] bb = Encoding.GetEncoding("ISO-8859-1").GetBytes(authInfo);
				authInfo = Convert.ToBase64String(bb);
				s_CompleteString = "Basic " + authInfo;
				}

			return s_CompleteString;
			}

		/// <summary>
		/// Добавить строку аутентификации (имя пользователя и пароль) в запрос к серверу
		/// </summary>
		/// <param name="request">запрос к серверу</param>
		public static void SetBasicAuthenticationHeader(WebRequest request)
			{
			if (!HasAuthenticationData)
				{
				return;
				}

			request.Headers[cAuthorization] = GetBasicAuthenticationHeader();
			}

		/// <summary>
		/// Добавить строку аутентификации (имя пользователя и пароль) в запрос к серверу
		/// </summary>
		/// <param name="request">запрос к серверу</param>
		public static void SetBasicAuthenticationHeader(System.ServiceModel.Channels.Message request)
			{
			if (!HasAuthenticationData)
				{
				return;
				}

			HttpRequestMessageProperty httpRequestMessage;

			httpRequestMessage = GetHttpRequestMessageProperty(request);

			string CurrentAuthorizationValue = httpRequestMessage.Headers[cAuthorization];
			string Auth = GetBasicAuthenticationHeader();
			if (string.IsNullOrEmpty(CurrentAuthorizationValue))
				{
				httpRequestMessage.Headers.Add(cAuthorization, Auth);
				}
			else
				{
				httpRequestMessage.Headers[cAuthorization] = Auth;
				}
			}

		/// <summary>
		/// Возвращает набор заголовков http
		/// </summary>
		/// <param name="request">запрос к серверу</param>
		/// <returns>набор заголовков http</returns>
		public static HttpRequestMessageProperty GetHttpRequestMessageProperty(Message request)
			{
			if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
				{
				HttpRequestMessageProperty httpRequestMessage = new HttpRequestMessageProperty();
				request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
				}

			return request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
			}
		}
	}
