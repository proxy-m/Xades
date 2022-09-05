using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GisBustedWsdlWrapper;

namespace GisBusted.Workers
	{
	/// <summary>
	/// Базовый класс для SOAP клиентов
	/// </summary>
	public abstract class WorkerBase
		{
		/// <summary>
		/// Инспектор сообщений
		/// </summary>
		private WCF.WcfClientMessageInspector m_MessageInspector;



		/// <summary>
		/// Атрибуты отправителя для подписи запроса
		/// </summary>
		private SenderCredentials m_SenderCredentials;

		/// <summary>
		/// Атрибуты отправителя для подписи запроса
		/// </summary>
		public SenderCredentials SenderCredentials
			{
			get
				{
				return m_SenderCredentials;
				}

			protected set
				{
				m_SenderCredentials = value;
				}
			}



		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="sc">Атрибуты отправителя для подписи запроса</param>
		protected WorkerBase(SenderCredentials sc)
			{
			if (sc == null)
				{
				throw new ArgumentNullException("m_SenderCredentials не может быть null");
				}

			this.SenderCredentials = sc;
			}

		#endregion Конструкторы

		#region Методы - фильтры сообщений

		/// <summary>
		/// Добавляет фильтр сообщений в конечную точку Endpoint
		/// </summary>
		/// <param name="SignRequest">Нужно ли подписывать запрос</param>
		/// <param name="Endpoint">Конечная точка</param>
		private void SoapAddMessageFilter(bool SignRequest, System.ServiceModel.Description.ServiceEndpoint Endpoint)
			{
			if (m_MessageInspector != null)
				{
				throw new InvalidOperationException("Инспектор уже добавлен");
				}

			X509Certificate2 SigningCertificate = GetSigningCertificate();

			WCF.WcfEndpointBehavior Behavior = new WCF.WcfEndpointBehavior(SigningCertificate, SignRequest);
			m_MessageInspector = Behavior.MessageInspector;

			Endpoint.Behaviors.Add(Behavior);
			}

		#endregion Методы - фильтры сообщений

		#region Методы - заголовки

		/// <summary>
		/// Возвращает запрос класса T
		/// </summary>
		/// <typeparam name="T">класс запроса</typeparam>
		/// <returns>заголовок класса T</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		protected T SoapRequest_Get<T>() where T : class
			{
			object tInstance = Activator.CreateInstance(typeof(T));

			PropertyInfo[] props = tInstance.GetType().GetProperties();

			foreach (PropertyInfo prop in props)
				{
				switch (prop.Name)
					{
					case "Id":
							{
							if (MessageInspector != null)
								{
								if (MessageInspector.SignRequest)
									{
									prop.SetValue(tInstance, Crypto.XadesBesSigner.XADES_SIGNED_DATA_CONTAINER, null);
									}
								}
							else
								{
								prop.SetValue(tInstance, Crypto.XadesBesSigner.XADES_SIGNED_DATA_CONTAINER, null);
								}
							break;
							}
					}
				}

			return tInstance as T;
			}

		/// <summary>
		/// Возвращает заголовок класса T
		/// </summary>
		/// <typeparam name="T">класс заголовка</typeparam>
		/// <returns>заголовок класса T</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		protected T SoapHeader_Get<T>() where T : class
			{
			object tInstance = Activator.CreateInstance(typeof(T));

			PropertyInfo[] props = tInstance.GetType().GetProperties();

			// http://www.cyberforum.ru/web-services-wcf/thread1615223-page33.html#post9322155
			foreach (PropertyInfo prop in props)
				{
				switch (prop.Name)
					{
					case "Item":
							{
							if (SenderCredentials == null)
								{
								throw new InvalidOperationException("SenderCredentials == null");
								}

							//if (SenderCredentials.IsSenderID)
							//	{
							//	prop.SetValue(tInstance, SenderCredentials.SenderID, null);
							//	}
							//else
							if (SenderCredentials.IsorgPPAGUID)
								{
								prop.SetValue(tInstance, SenderCredentials.orgPPAGUID, null);
								}
							else
								{
								throw new InvalidOperationException("Не понятно, что подставлять в Item");
								}
							break;
							}
					case "ItemElementName":
							{
							if (SenderCredentials == null)
								{
								throw new InvalidOperationException("SenderCredentials == null");
								}

							//if (SenderCredentials.IsSenderID)
							//	{
							//	prop.SetValue(tInstance, ItemChoiceType3.SenderID, null);
							//	}
							//else
							if (SenderCredentials.IsorgPPAGUID)
								{
								prop.SetValue(tInstance, ItemChoiceType2.orgPPAGUID, null);
								}
							else
								{
								throw new InvalidOperationException("Не понятно, что подставлять в ItemElementName");
								}
							break;
							}

					// уже вроде как стало и не нужно
					//case "SenderID": // уже вроде как стало и не нужно
					//			{
					//			prop.SetValue(tInstance, SenderID, null);
					//			break;
					//			}
					case "MessageGUID":
							{
							prop.SetValue(tInstance, Guid.NewGuid().ToString(), null);
							break;
							}
					case "Date":
							{
							prop.SetValue(tInstance, DateTime.Now, null);
							break;
							}
					case "IsOperatorSighnature":
					case "IsOperatorSignature":
							{
							prop.SetValue(tInstance, true, null);
							break;
							}
					case "IsOperatorSighnatureSpecified":
					case "IsOperatorSignatureSpecified":
							{
							prop.SetValue(tInstance, true, null);
							break;
							}
					}
				}

			return tInstance as T;
			}

		#endregion Методы - заголовки

		#region Методы - конечные точки и шифрование

		/// <summary>
		/// Получить Binding и EndpointAddress по имени конечной точки EndpointName
		/// </summary>
		/// <param name="EndpointName">Название Endpoint</param>
		/// <param name="TargetBinding">Результирующий Binding</param>
		/// <param name="TargetEndpointAddress">Результирующий EndpointAddress</param>
		/// <returns>true если все получилось</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
		protected bool SoapGetBindingByEndpointName(string EndpointName, out System.ServiceModel.Channels.Binding TargetBinding, out System.ServiceModel.EndpointAddress TargetEndpointAddress)
			{
			if (!GisGlobals.ProxyConfiguration.GetBindingByEndpointName(EndpointName, SenderCredentials, out TargetBinding, out TargetEndpointAddress))
				{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat(("Не удалось получить привязки для конечной точки {0}"), EndpointName);
				GisGlobals.ErrorMessageBox(sb.ToString());
				return false;
				}

			return true;
			}

		/// <summary>
		/// Устанавливает набор свойств для самодельного шифрованного соединения - без внешнего прокси
		/// </summary>
		/// <param name="ClientCredentials">'Верительные грамоты' клиентской стороны</param>
		/// <param name="TransportCertificate">Транспортный сертификат</param>
		protected static void SoapSetClientCredentials(System.ServiceModel.Description.ClientCredentials ClientCredentials, X509Certificate2 TransportCertificate)
			{
			ClientCredentials.ClientCertificate.Certificate = TransportCertificate;
			}

		#endregion Методы - конечные точки и шифрование

		#region Настройки перед вызовом

		/// <summary>
		/// Настроить экземпляр класса используемого для запроса. Запрос - подписать
		/// </summary>
		/// <param name="Endpoint">Конечная точка</param>
		/// <param name="ClientCredentials">'Верительные грамоты' клиентской стороны</param>
		protected void SoapPrepareQuery(System.ServiceModel.Description.ServiceEndpoint Endpoint, System.ServiceModel.Description.ClientCredentials ClientCredentials)
			{
			// С подписанием запроса
			SoapAddMessageFilter(true, Endpoint);

			// если прокси не используется, то установить данные клиента
			SetClientCredentials(ClientCredentials);
			}

		private void SetClientCredentials(System.ServiceModel.Description.ClientCredentials ClientCredentials)
			{
			X509Certificate2 TransportCertificate = GetTransportCertificate();
			SoapSetClientCredentials(ClientCredentials, TransportCertificate);
			}

		/// <summary>
		/// Получить транспортный сертификат
		/// </summary>
		/// <returns></returns>
		private X509Certificate2 GetTransportCertificate()
			{
			return GisGlobals.TransportCertificate;
			}

		/// <summary>
		/// Получить сертификат подписи
		/// </summary>
		/// <returns></returns>
		private X509Certificate2 GetSigningCertificate()
			{
			return GisGlobals.SigningCertificate;
			}

		#endregion Настройки перед вызовом

		#region Свойства

		/// <summary>
		/// Инспектор сообщений
		/// </summary>
		protected WCF.WcfClientMessageInspector MessageInspector
			{
			get
				{
				return m_MessageInspector;
				}
			}

		#endregion Свойства

		/// <summary>
		/// Команда запуска
		/// </summary>
		public void Run()
			{
			InternalRun();
			}

		/// <summary>
		/// C функция для выполнения некоторых действий
		/// </summary>
		protected abstract void InternalRun();
		}
	}
