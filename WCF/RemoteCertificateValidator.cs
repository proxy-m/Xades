using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Вспомогательный класс-валидатор сертификата сервера
	/// </summary>
	internal static class RemoteCertificateValidator
		{
		/// <summary>
		/// Функция проверки проверки сертификатов удаленного сервера
		/// </summary>
		/// <param name="sender">Объект, содержащий сведения о состоянии для данной проверки</param>
		/// <param name="Certificate">
		/// Сертификат, используемый для проверки подлинности удаленной стороны
		/// </param>
		/// <param name="chain">Цепочка центров сертификации, связанная с удаленным сертификатом</param>
		/// <param name="sslPolicyErrors">Одна или более ошибок, связанных с удаленным сертификатом</param>
		/// <returns>true если указанный сертификат принимается для проверки подлинности</returns>
		internal static bool ValidateRemoteCertificate(Object sender, X509Certificate Certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
			if (sslPolicyErrors == SslPolicyErrors.None)
				{
				return true;
				}

			return true;
			}

		/// <summary>
		/// Глобальные настройки WCF
		/// </summary>
		internal static void ConfigureServicePointManager()
			{
			ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

			// Разрешаем только TLS
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
			ServicePointManager.CheckCertificateRevocationList = false;
			}
		}
	}
