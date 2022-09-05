using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace GisBusted.WSDL
	{
	/// <summary>
	/// Вспомогательный класс для изучения wsdl на сервере
	/// </summary>
	public class WsdlExplorer : Workers.WorkerBase
		{
		/// <summary>
		/// Адрес конечной точки
		/// </summary>
		private readonly string m_EndpointAddress;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="EndpointAddress">Адрес конечной точки</param>
		/// <param name="SenderCredentials">Атрибуты отправителя для подписи запроса</param>
		public WsdlExplorer(string EndpointAddress, SenderCredentials SenderCredentials)
			: base(SenderCredentials)
			{
			m_EndpointAddress = EndpointAddress;
			}

		#endregion Конструкторы

		#region Набор методов для переопределения

		/// <summary>
		/// Перегружаемая функция для выполнения некоторых действий
		/// </summary>
		protected override void InternalRun()
			{
			try
				{
				HttpWebRequest request;
				string Result;
				string Url = m_EndpointAddress + "?wsdl";

				Url = Url.Replace("http://", "https://");
				UriBuilder builder = new UriBuilder(Url);
				builder.Scheme = "https";
				Uri modifiedAddress = builder.Uri;

				request = (HttpWebRequest) HttpWebRequest.Create(modifiedAddress);
				request.ClientCertificates.Add(GisGlobals.TransportCertificate);

				if (WCF.BasicAuthenticationHelper.HasAuthenticationData)
					{
					WCF.BasicAuthenticationHelper.SetBasicAuthenticationHeader(request);
					}

				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = 0;

				HttpWebResponse response = request.GetResponse() as HttpWebResponse;

				using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8, true))
					{
					Result = sr.ReadToEnd();
					sr.Close();
					}

				Result = ReformatXml(Result);

				Debuggers.GisDebugger.Show("WsdlExplorer - " + Url, Result);

				return;
				}
			catch (Exception e)
				{
				GisGlobals.ErrorMessageBox(e.ToString());
				}
			}

		#endregion Набор методов для переопределения

		/// <summary>
		/// Функция для переформатирования тектса
		/// </summary>
		/// <param name="Text">Исходный текст</param>
		/// <returns>Форматированный текст</returns>
		private static string ReformatXml(string Text)
			{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(Text);

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.CheckCharacters = true;
			xws.Indent = true;
			xws.IndentChars = "    ";
			xws.NewLineHandling = NewLineHandling.Replace;

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, xws))
				{
				doc.Save(writer);
				}

			string Result = sb.ToString();
			return Result;
			}
		}
	}
