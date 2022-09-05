using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Вспомогательный класс - инспектор сообщений
	/// </summary>
	public class WcfClientMessageInspector : IClientMessageInspector
		{
		/// <summary>
		/// Сертификат подписи Xades
		/// </summary>
		private readonly X509Certificate2 SigningCertificate;

		/// <summary>
		/// Нужно ли подписывать запрос
		/// </summary>
		private bool m_SignRequest = true;

		#region Конструкторы

		/// <summary> Конструктор </summary> <param name="SigningCertificate">Сертификат подписи
		/// Xades</param> <param name="SignRequest">Нужно ли подписывать запрос</param>
		/// Вспомогательный класс для сбора информации о запросах и ответах сервера </param>
		public WcfClientMessageInspector(X509Certificate2 SigningCertificate, bool SignRequest)
			{
			this.SigningCertificate = SigningCertificate;
			m_SignRequest = SignRequest;
			}

		#endregion Конструкторы

		#region Свойства

		/// <summary>
		/// Нужно ли подписывать запрос
		/// </summary>
		public bool SignRequest
			{
			get
				{
				return m_SignRequest;
				}
			}

		#endregion Свойства

		#region Реализация интерфейса IClientMessageInspector

		/// <summary>
		/// Разрешает проверку или изменение сообщения до того, как сообщение запроса отправляется службе
		/// </summary>
		/// <param name="request">Сообщение, отправляемое службе</param>
		/// <param name="channel"></param>
		/// <returns>
		/// Объект, который возвращается как аргумент correlationState метода AfterReceiveReply. Если
		/// состояние корреляции не используется, то его значение — null
		/// </returns>
		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
			{
			#region Отладчик

			if (Debuggers.DebuggerSettings.SoapShowMessageBeforeSendBeforeFiltering)
				{
				string Text;

				Text = Debuggers.GisDebugger.GetMessageText(ref request);

				if (Debuggers.DebuggerSettings.SoapShowMessageBeforeSendBeforeFiltering)
					{
					Show("BeforeSendRequest - до подписи", Text);
					}
				}

			#endregion Отладчик

			string MessageText = TextOfMessage(ref request);

			if (SignRequest)
				{
				string SignedText;
				SignedText = Crypto.XadesBesSigner.GetSignedRequestXades(MessageText, SigningCertificate);
				request = CreateMessageFromString(SignedText, request.Version);
				}

			#region Отладчик

			if (Debuggers.DebuggerSettings.SoapShowMessageBeforeSendAfterFiltering)
				{
				string Text;

				Text = Debuggers.GisDebugger.GetMessageText(ref request);

				if (Debuggers.DebuggerSettings.SoapShowMessageBeforeSendAfterFiltering)
					{
					Show("BeforeSendRequest - после подписи", Text);
					}
				}

			#endregion Отладчик

			#region Вставка заголовка аутентификации Ланита

			if (!GisGlobals.ProxyConfiguration.ConfigFile.IsProduction)
				{
				WCF.BasicAuthenticationHelper.SetBasicAuthenticationHeader(request);
				}

			#endregion Вставка заголовка аутентификации Ланита

			return null;
			}

		/// <summary>
		/// Разрешает проверку или изменение сообщения после получения ответного сообщения, но перед
		/// его передачей клиентскому приложению
		/// </summary>
		/// <param name="reply">Ответ сервера</param>
		/// <param name="correlationState"></param>
		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
			{
			if (Debuggers.DebuggerSettings.SoapShowMessageAfterReceive)
				{
				string Text;

				Text = Debuggers.GisDebugger.GetMessageText(ref reply);

				if (Debuggers.DebuggerSettings.SoapShowMessageAfterReceive)
					{
					Show("AfterReceiveReply - после получения", Text);
					}
				}
			}

		#endregion Реализация интерфейса IClientMessageInspector

		#region Вспомогательные функции

		/// <summary>
		/// Нужно ли подписывать запрос
		/// </summary>
		/// <param name="SignRequest">true если запрос нужно подписать</param>
		/// <returns>Предыдущее значение флага подписывания запроса</returns>
		public bool SetSignRequest(bool SignRequest)
			{
			bool bPreviousSignRequest = m_SignRequest;
			m_SignRequest = SignRequest;
			return bPreviousSignRequest;
			}

		/// <summary>
		/// Получить XML текст сообщения
		/// </summary>
		/// <param name="msg">Текст сообщения</param>
		/// <returns>XML текст сообщения</returns>
		private static string TextOfMessage(ref Message msg)
			{
			MessageBuffer mb = msg.CreateBufferedCopy(int.MaxValue);
			msg = mb.CreateMessage();

			Stream s = new MemoryStream();
			XmlWriter xw = XmlWriter.Create(s);
			mb.CreateMessage().WriteMessage(xw);
			xw.Flush();
			s.Position = 0;

			byte[] bXML = new byte[s.Length];
			s.Read(bXML, 0, (int) s.Length);

			// sometimes bXML[] starts with a BOM
			if (bXML[0] != (byte) '<')
				{
				return Encoding.UTF8.GetString(bXML, 3, bXML.Length - 3);
				}
			else
				{
				return Encoding.UTF8.GetString(bXML, 0, bXML.Length);
				}
			}

		/// <summary>
		/// Создает объект типа Message из текста всего сообщения SOAP
		/// </summary>
		/// <param name="MessageText">Текст всего сообщения SOAP</param>
		/// <param name="ver">Версия собщения</param>
		/// <returns>Сообщение</returns>
		public static Message CreateMessageFromString(string MessageText, MessageVersion ver)
			{
			XmlReader Reader = XmlReaderFromString(MessageText);
			return Message.CreateMessage(Reader, int.MaxValue, ver);
			}

		/// <summary>
		/// Создает XmlReader из строки содержащей текст XML
		/// </summary>
		/// <param name="xml">текст XML всего сообщения SOAP</param>
		/// <returns></returns>
		private static XmlReader XmlReaderFromString(string xml)
			{
			var stream = new MemoryStream();

			// NOTE: don't use using(var writer ...){...} because the end of the StreamWriter's using
			//       closes the Stream itself
			var writer = new System.IO.StreamWriter(stream);
			writer.Write(xml);
			writer.Flush();
			stream.Position = 0;
			return XmlReader.Create(stream);
			}

		/// <summary>
		/// Отладочная функция - показывает текст сообщения
		/// </summary>
		/// <param name="Caption">Заголовок</param>
		/// <param name="msgbuf">Буфер сообщения</param>
		protected static void Show(string Caption, MessageBuffer msgbuf)
			{
			Debuggers.GisDebugger.Show(Caption, msgbuf);
			}

		/// <summary>
		/// Отладочная функция - показывает текст сообщения
		/// </summary>
		/// <param name="Caption">Заголовок</param>
		/// <param name="Text">Текст сообщения</param>
		protected static void Show(string Caption, string Text)
			{
			Debuggers.GisDebugger.Show(Caption, Text);
			}

		#endregion Вспомогательные функции
		}
	}
