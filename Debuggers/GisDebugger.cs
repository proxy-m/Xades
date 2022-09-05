using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace GisBusted.Debuggers
	{
	/// <summary>
	/// Класс для внутренней отладки
	/// </summary>
	public static class GisDebugger
		{
		#region Свойства

		#endregion Свойства

		#region Вспомогательные функции

		/// <summary>
		/// Показать отладочное окно с моноширинным текстом
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="Context">Содержимое окна</param>
		/// <param name="bModal">Модальное окно</param>
		private static void InternalShowText(string Caption, string Context, bool bModal)
			{
			DebugTextForm dtf = null;

			var t = new Thread(() =>
			{
				Application.Run(dtf = new DebugTextForm(Caption, Context));
			});
			t.Start();

			if (dtf != null)
				{
				if (bModal)
					{
					dtf.ShowDialog();
					}
				else
					{
					dtf.Show();
					}
				}
			}

		/// <summary>
		/// Показать отладочное окно с моноширинным текстом, содержимое сообщения
		/// </summary>
		/// <param name="Caption">Заголовок</param>
		/// <param name="msgbuf">Буфер сообщения</param>
		/// <param name="bModal">Модальное окно</param>
		private static void InternalShowMessage(string Caption, MessageBuffer msgbuf, bool bModal)
			{
			string Text;
			Text = GetMessageBufferText(msgbuf);
			InternalShowText(Caption, Text, bModal);
			}

		/// <summary>
		/// Функция для переформатирования тектса
		/// </summary>
		/// <param name="Text">Исходный текст</param>
		/// <returns>Форматированный текст</returns>
		public static string ReformatXml(string Text)
			{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(Text);

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.CheckCharacters = true;
			xws.Indent = true;
			xws.IndentChars = "    ";
			xws.NewLineHandling = NewLineHandling.Replace;
			xws.NewLineChars = "\r\n";

			//xws.CloseOutput = true;

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, xws))
				{
				doc.Save(writer);
				}

			string Result = sb.ToString();
			return Result;
			}

		#endregion Вспомогательные функции

		#region Функции показа - текст

		/// <summary>
		/// Показать модальное отладочное окно с моноширинным текстом - текст
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="Context">Содержимое окна</param>
		public static void ShowModal(string Caption, string Context)
			{
			InternalShowText(Caption, Context, true);
			}

		/// <summary>
		/// Показать отладочное окно с моноширинным текстом - текст
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="Context">Содержимое окна</param>
		public static void Show(string Caption, string Context)
			{
			InternalShowText(Caption, Context, false);
			}

		#endregion Функции показа - текст

		#region Функции показа - сообщение

		/// <summary>
		/// Показать модальное отладочное окно с моноширинным текстом - содержимое таблицы
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="msgbuf">Буфер сообщения</param>
		public static void ShowModal(string Caption, MessageBuffer msgbuf)
			{
			InternalShowMessage(Caption, msgbuf, true);
			}

		/// <summary>
		/// Показать отладочное окно с моноширинным текстом - содержимое таблицы
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="msgbuf">Буфер сообщения</param>
		public static void Show(string Caption, MessageBuffer msgbuf)
			{
			InternalShowMessage(Caption, msgbuf, false);
			}

		#endregion Функции показа - сообщение

		#region Функции преобразования

		/// <summary>
		/// Преобразовать сообщение типа MessageBuffer в текст
		/// </summary>
		/// <param name="msgbuf">сообщение типа MessageBuffer</param>
		/// <returns>Текст сообщения</returns>
		public static string GetMessageBufferText(MessageBuffer msgbuf)
			{
			string Text;
			XPathNavigator nav = msgbuf.CreateNavigator();
			using (MemoryStream ms = new MemoryStream())
				{
				using (XmlWriter xw = XmlWriter.Create(ms))
					{
					nav.WriteSubtree(xw);
					Text = nav.InnerXml;
					}
				}
			return Text;
			}

		/// <summary>
		/// Преобразовать сообщение System.ServiceModel.Channels.Message в текст
		/// </summary>
		/// <param name="request">запрос</param>
		/// <returns>Текст сообщения</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public static string GetMessageText(ref System.ServiceModel.Channels.Message request)
			{
			MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
			System.ServiceModel.Channels.Message msgCopy = buffer.CreateMessage();

			request = buffer.CreateMessage();

			// Get the SOAP XML content.
			System.ServiceModel.Channels.Message m = buffer.CreateMessage();

			string Text = m.ToString();

			// Get the SOAP XML body content.
			System.Xml.XmlDictionaryReader xrdr = msgCopy.GetReaderAtBodyContents();
			string bodyData = xrdr.ReadOuterXml();

			// Replace the body placeholder with the actual SOAP body.
			Text = Text.Replace("... stream ...", bodyData);
			Text = Text.Replace("... поток ...", bodyData);
			Text = ReformatXml(Text);
			return Text;
			}

		#endregion Функции преобразования
		}
	}
