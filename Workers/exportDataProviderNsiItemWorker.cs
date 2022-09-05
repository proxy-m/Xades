using System;
using GisBustedWsdlWrapper;

namespace GisBusted.Workers
	{
	/// <summary>
	/// Экспортировать данные справочников поставщика информации (1,51,59)
	/// </summary>
	public class exportDataProviderNsiItemWorker : WorkerBase
		{
		/// <summary>
		/// Номер справочника - может быть 1, 51, 59
		/// </summary>
		private readonly exportDataProviderNsiItemRequestRegistryNumber m_RegistryNumber;

		/// <summary>
		/// Номер справочника в виде строки - может быть 1, 51, 59
		/// </summary>
		private readonly string m_sRegistryNumber;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="SenderCredentials">Атрибуты отправителя для подписи запроса</param>
		/// <param name="sRegistryNumber">Номер справочника в виде строки - может быть 1, 51, 59</param>
		public exportDataProviderNsiItemWorker(SenderCredentials SenderCredentials, string sRegistryNumber)
			: base(SenderCredentials)
			{
			m_sRegistryNumber = sRegistryNumber;

			if (m_sRegistryNumber.Equals("1"))
				{
				m_RegistryNumber = exportDataProviderNsiItemRequestRegistryNumber.Item1;
				}
			else
			if (m_sRegistryNumber.Equals("51"))
				{
				m_RegistryNumber = exportDataProviderNsiItemRequestRegistryNumber.Item51;
				}
			else
			if (m_sRegistryNumber.Equals("59"))
				{
				m_RegistryNumber = exportDataProviderNsiItemRequestRegistryNumber.Item59;
				}
			else
				{
				throw new ArgumentException("Неверный номер справочника");
				}
			}

		#endregion Конструкторы

		#region Набор методов для переопределения

		/// <summary>
		/// Перегружаемая функция для выполнения некоторых действий
		/// </summary>
		protected override void InternalRun()
			{
			exportDataProviderNsiItemRequest Request = null;
			exportNsiItemResult Result = null;

			Request = SoapRequest_Get<exportDataProviderNsiItemRequest>();

			RequestHeader header = SoapHeader_Get<RequestHeader>();

			System.ServiceModel.Channels.Binding TargetBinding;
			System.ServiceModel.EndpointAddress TargetEndpointAddress;

			if (!SoapGetBindingByEndpointName("NsiPort", out TargetBinding, out TargetEndpointAddress))
				{
				return;
				}

			NsiPortsTypeClient Proxy = new NsiPortsTypeClient(TargetBinding, TargetEndpointAddress);

			// настраиваем экземпляр класса клиента для работы
			SoapPrepareQuery(Proxy.Endpoint, Proxy.ClientCredentials);

			Request.RegistryNumber = m_RegistryNumber;

			// Обращение к службе
			try
				{
				Proxy.exportDataProviderNsiItem(header, Request, out Result);
				}
			catch (Exception e)
				{
				GisGlobals.ErrorMessageBox(e.ToString());
				}
			}

		#endregion Набор методов для переопределения
		}
	}
