namespace GisBusted.Debuggers
	{
	/// <summary>
	/// Настройки отладчика
	/// </summary>
	public static class DebuggerSettings
		{
		static DebuggerSettings()
			{
			SoapShowMessageBeforeSendBeforeFiltering = false;
			SoapShowMessageBeforeSendAfterFiltering = true;
			SoapShowMessageAfterReceive = true;
			}

		/// <summary>
		/// Показать отправляемое сообщение до преобразования
		/// </summary>
		public static bool SoapShowMessageBeforeSendBeforeFiltering
			{
			get; set;
			}

		/// <summary>
		/// Показать отправляемое сообщение после преобразования
		/// </summary>
		public static bool SoapShowMessageBeforeSendAfterFiltering
			{
			get; set;
			}

		/// <summary>
		/// Показать полученное сообщение
		/// </summary>
		public static bool SoapShowMessageAfterReceive
			{
			get; set;
			}
		}
	}
