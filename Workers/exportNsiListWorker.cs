using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GisBustedWsdlWrapper;

namespace GisBusted.Workers
	{
	public class exportNsiListWorker : WorkerBase
		{
		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="SenderCredentials">Атрибуты отправителя для подписи запроса</param>
		public exportNsiListWorker(SenderCredentials SenderCredentials)
			: base(SenderCredentials)
			{
			}

		#endregion Конструкторы

		#region Набор методов для переопределения

		/// <summary>
		/// Перегружаемая функция для выполнения некоторых действий
		/// </summary>
		protected override void InternalRun()
			{
			System.ServiceModel.Channels.Binding TargetBinding;
			System.ServiceModel.EndpointAddress TargetEndpointAddress;

			if (!SoapGetBindingByEndpointName("NsiPort1", out TargetBinding, out TargetEndpointAddress))
				{
				GisGlobals.ErrorMessageBox("Не удалось получить привязки для NsiPort1");
				return;
				}

			exportNsiListResult Result;
			HeaderType Header = SoapHeader_Get<HeaderType>();

			NsiPortsType1Client Proxy = new NsiPortsType1Client(TargetBinding, TargetEndpointAddress);

			// настраиваем экземпляр класса клиента для работы
			SoapPrepareQuery(Proxy.Endpoint, Proxy.ClientCredentials);

			exportNsiListRequest Request;
			Request = SoapRequest_Get<exportNsiListRequest>();

			// используем группу NSI
			Request.ListGroup = ListGroup.NSI;
			Request.ListGroupSpecified = true;

			Proxy.exportNsiList(Header, Request, out Result);

			NsiListType NsiList = Result.Item as NsiListType;

			}

		#endregion Набор методов для переопределения

		}
	}
