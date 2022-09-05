using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace GisBusted.WCF
	{
	/// <summary>
	/// Расширитель поведения в различных проявлениях
	/// </summary>
	internal class WcfEndpointBehavior : IEndpointBehavior
		{
		/// <summary>
		/// Инспектор сообщений
		/// </summary>
		private readonly WcfClientMessageInspector m_WcfClientMessageInspector;

		#region Конструкторы

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="SigningCertificate">Сертификат подписи Xades</param>
		/// <param name="SignRequest">Нужно ли подписывать запрос</param>
		public WcfEndpointBehavior(X509Certificate2 SigningCertificate, bool SignRequest)
			{
			m_WcfClientMessageInspector = new WcfClientMessageInspector(SigningCertificate, SignRequest);
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
				return m_WcfClientMessageInspector.SignRequest;
				}
			}

		/// <summary>
		/// Инспектор сообщений
		/// </summary>
		public WcfClientMessageInspector MessageInspector
			{
			get
				{
				return m_WcfClientMessageInspector;
				}
			}

		#endregion Свойства

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
			{
			clientRuntime.MessageInspectors.Add(MessageInspector);
			}

		/// <summary>
		/// Реализуйте для подтверждения соответствия конечной точки намеченным критериям
		/// </summary>
		/// <param name="endpoint"></param>
		public void Validate(ServiceEndpoint endpoint)
			{
			}

		/// <summary>
		/// Метод используется для предоставления элементов
		/// привязки с пользовательскими данными в среде выполнения для разрешения привязок для поддержки настраиваемых поведений
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="bindingParameters"></param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
			{
			}

		/// <summary>
		/// Метод используется для изменения, проверки или вставки оснасток расширения в выполнение всей конечной точки в приложении службы
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
			{
			}

		}
	}