using System;
using System.Windows.Forms;
using GisBusted.Helpers;
using GisBusted.Workers;

namespace GisBusted
	{
	public partial class MainForm : Form
		{
		public MainForm()
			{
			InitializeComponent();
			}

		#region Вспомогательные функции

		/// <summary>
		/// Атрибуты отправителя для подписи запроса
		/// </summary>
		/// <returns>Атрибуты отправителя для подписи запроса</returns>
		private SenderCredentials GetSenderCredentials()
			{
			string Credential = orgPPAGUIDTextBox.Text;
			SenderCredentials sc = new SenderCredentials(Credential);
			return sc;
			}

		private void FillEndpoints()
			{
			EndPointsComboBox.DataSource = GisGlobals.ProxyConfiguration.EndpointElements;
			}

		#endregion Вспомогательные функции

		#region Инициализация

		/// <summary>
		/// Инициализация
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
			{
			#region Caption

			string Caption = this.Text;
			Caption = Caption + " - v. " + GisGlobals.GetAssemblyFileVersion();
			this.Text = Caption;

			#endregion Caption

			#region ConfigComboBox

			WCF.GisBustedProxyConfigFile CurrentConfig = GisGlobals.ProxyConfiguration.ConfigFile;
			ConfigComboBox.Sorted = false;
			ConfigComboBox.DataSource = GisGlobals.ConfigFiles.ConfigFiles;

			if (CurrentConfig != ConfigComboBox.SelectedValue)
				{
				ConfigComboBox.SelectedItem = CurrentConfig;
				}

			#endregion ConfigComboBox

			#region ProviderModelComboBox

			OneItem[] ProviderModels = new OneItem[2]
				{
				new OneItem() {Name="CryptoAPI - используются криптопровайдеры через System.Runtime.InteropServices",Value=ProviderModel.CryptoApi.ToString()},
				new OneItem() {Name="System.Security.Cryptography - т.е. косвенные вызовы КриптоПро .NET",Value=ProviderModel.dotNetFramework.ToString()}
				};

			ProviderModelComboBox.DataSource = ProviderModels;

			ProviderModelComboBox.SelectedItem = ProviderModels[0];


			#endregion ProviderModelComboBox

			#region Отладчик

			BeforeSendBeforeFilteringCheckBox.Checked = Debuggers.DebuggerSettings.SoapShowMessageBeforeSendBeforeFiltering;
			BeforeSendAfterFilteringCheckBox.Checked = Debuggers.DebuggerSettings.SoapShowMessageBeforeSendAfterFiltering;
			AfterReceiveCheckBox.Checked = Debuggers.DebuggerSettings.SoapShowMessageAfterReceive;

			#endregion Отладчик

			orgPPAGUIDTextBox.Text = GisGlobals.ConfigFileSettings.orgPPAGUID;

			#region Справочники услуг

			OneItem[] ExportDataProviderNsiItemItems = new OneItem[3]
					{
				new OneItem() {Name="1",Value="1"},
				new OneItem() {Name="51",Value="51"},
				new OneItem() {Name="59",Value="59"},
					};

			ExportDataProviderNsiItemComboBox.DataSource = ExportDataProviderNsiItemItems;

			#endregion Справочники услуг
			}

		#endregion Инициализация

		#region Обработчики нажатий и пр.


		private void OnSelectedValueChanged(object sender, EventArgs e)
			{
			OneItem oi = ProviderModelComboBox.SelectedValue as OneItem;
			if (oi.Value == ProviderModel.CryptoApi.ToString())
				{
				GisGlobals.ProviderModel = ProviderModel.CryptoApi;
				}
			else
			if (oi.Value == ProviderModel.dotNetFramework.ToString())
				{
				GisGlobals.ProviderModel = ProviderModel.dotNetFramework;
				}
			else
				{
				throw new NotImplementedException();
				}
			}


		private void OnSelectCertificates(object sender, EventArgs e)
			{
			GisGlobals.SelectCertificates(null, null);
			}

		private void OnConfigSelectedIndexChanged(object sender, EventArgs e)
			{
			WCF.GisBustedProxyConfigFile CurrentCF = GisGlobals.ProxyConfiguration.ConfigFile;

			WCF.GisBustedProxyConfigFile cf = ConfigComboBox.SelectedValue as WCF.GisBustedProxyConfigFile;

			if (cf == CurrentCF)
				{
				FillEndpoints();
				return;
				}

			GisGlobals.InitProxyConfiguration(cf);

			FillEndpoints();
			}

		private void BeforeSendBeforeFilteringCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			Debuggers.DebuggerSettings.SoapShowMessageBeforeSendBeforeFiltering = BeforeSendBeforeFilteringCheckBox.Checked;
			}

		private void BeforeSendAfterFilteringCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			Debuggers.DebuggerSettings.SoapShowMessageBeforeSendAfterFiltering = BeforeSendAfterFilteringCheckBox.Checked;
			}

		private void AfterReceiveCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			Debuggers.DebuggerSettings.SoapShowMessageAfterReceive = AfterReceiveCheckBox.Checked;
			}

		#endregion Обработчики нажатий

		#region Воркеры

		/// <summary>
		/// Браузер wsdl
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWsdlExplorerButtonClick(object sender, EventArgs e)
			{
			SenderCredentials sc = GetSenderCredentials();
			WCF.WcfEndpointElement we = EndPointsComboBox.SelectedValue as WCF.WcfEndpointElement;
			string Address = we.Address;
			WSDL.WsdlExplorer Explorer = new WSDL.WsdlExplorer(Address, sc);
			Explorer.Run();
			}

		/// <summary>
		/// exportNsiList
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnexportNsiList(object sender, EventArgs e)
			{
			SenderCredentials Credentials = GetSenderCredentials();
			exportNsiListWorker worker = new exportNsiListWorker(Credentials);
			worker.Run();
			}

		/// <summary>
		/// exportDataProviderNsiItem - справочник услуг
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnexportDataProviderNsiItem(object sender, EventArgs e)
			{
			OneItem OI = (OneItem) ExportDataProviderNsiItemComboBox.SelectedItem;
			SenderCredentials sc = GetSenderCredentials();
			exportDataProviderNsiItemWorker worker = new exportDataProviderNsiItemWorker(sc, OI.Value);
			worker.Run();
			}


		#endregion Воркеры


		}
	}
