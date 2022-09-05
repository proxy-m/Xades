namespace GisBusted
	{
	partial class MainForm
		{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
			{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ConfigLabel = new System.Windows.Forms.Label();
			this.ConfigComboBox = new System.Windows.Forms.ComboBox();
			this.orgPPAGUIDLabel = new System.Windows.Forms.Label();
			this.orgPPAGUIDTextBox = new System.Windows.Forms.TextBox();
			this.SoapGroupBox = new System.Windows.Forms.GroupBox();
			this.AfterReceiveCheckBox = new System.Windows.Forms.CheckBox();
			this.BeforeSendAfterFilteringCheckBox = new System.Windows.Forms.CheckBox();
			this.BeforeSendBeforeFilteringCheckBox = new System.Windows.Forms.CheckBox();
			this.Linelabel1 = new System.Windows.Forms.Label();
			this.EndPointsLabel = new System.Windows.Forms.Label();
			this.EndPointsComboBox = new System.Windows.Forms.ComboBox();
			this.WsdlExplorerButton = new System.Windows.Forms.Button();
			this.exportNsiListButton = new System.Windows.Forms.Button();
			this.ExportDataProviderNsiItemComboBox = new System.Windows.Forms.ComboBox();
			this.ServiceLabel = new System.Windows.Forms.Label();
			this.exportDataProviderNsiItemButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SelectCertificatesButton = new System.Windows.Forms.Button();
			this.CPtypeLabel = new System.Windows.Forms.Label();
			this.ProviderModelComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SoapGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// ConfigLabel
			// 
			this.ConfigLabel.AutoSize = true;
			this.ConfigLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConfigLabel.Location = new System.Drawing.Point(7, 16);
			this.ConfigLabel.Name = "ConfigLabel";
			this.ConfigLabel.Size = new System.Drawing.Size(47, 13);
			this.ConfigLabel.TabIndex = 0;
			this.ConfigLabel.Text = "Сервер:";
			// 
			// ConfigComboBox
			// 
			this.ConfigComboBox.FormattingEnabled = true;
			this.ConfigComboBox.Location = new System.Drawing.Point(110, 13);
			this.ConfigComboBox.Name = "ConfigComboBox";
			this.ConfigComboBox.Size = new System.Drawing.Size(644, 21);
			this.ConfigComboBox.TabIndex = 1;
			this.ConfigComboBox.SelectedIndexChanged += new System.EventHandler(this.OnConfigSelectedIndexChanged);
			// 
			// orgPPAGUIDLabel
			// 
			this.orgPPAGUIDLabel.AutoSize = true;
			this.orgPPAGUIDLabel.BackColor = System.Drawing.Color.Transparent;
			this.orgPPAGUIDLabel.Location = new System.Drawing.Point(7, 44);
			this.orgPPAGUIDLabel.Name = "orgPPAGUIDLabel";
			this.orgPPAGUIDLabel.Size = new System.Drawing.Size(73, 13);
			this.orgPPAGUIDLabel.TabIndex = 2;
			this.orgPPAGUIDLabel.Text = "orgPPAGUID:";
			// 
			// orgPPAGUIDTextBox
			// 
			this.orgPPAGUIDTextBox.Location = new System.Drawing.Point(109, 41);
			this.orgPPAGUIDTextBox.Name = "orgPPAGUIDTextBox";
			this.orgPPAGUIDTextBox.Size = new System.Drawing.Size(389, 20);
			this.orgPPAGUIDTextBox.TabIndex = 3;
			// 
			// SoapGroupBox
			// 
			this.SoapGroupBox.BackColor = System.Drawing.Color.Transparent;
			this.SoapGroupBox.Controls.Add(this.AfterReceiveCheckBox);
			this.SoapGroupBox.Controls.Add(this.BeforeSendAfterFilteringCheckBox);
			this.SoapGroupBox.Controls.Add(this.BeforeSendBeforeFilteringCheckBox);
			this.SoapGroupBox.Location = new System.Drawing.Point(7, 67);
			this.SoapGroupBox.Name = "SoapGroupBox";
			this.SoapGroupBox.Size = new System.Drawing.Size(316, 88);
			this.SoapGroupBox.TabIndex = 22;
			this.SoapGroupBox.TabStop = false;
			this.SoapGroupBox.Text = "Сообщения SOAP:";
			// 
			// AfterReceiveCheckBox
			// 
			this.AfterReceiveCheckBox.AutoSize = true;
			this.AfterReceiveCheckBox.Location = new System.Drawing.Point(6, 64);
			this.AfterReceiveCheckBox.Name = "AfterReceiveCheckBox";
			this.AfterReceiveCheckBox.Size = new System.Drawing.Size(146, 17);
			this.AfterReceiveCheckBox.TabIndex = 22;
			this.AfterReceiveCheckBox.Text = "Полученное сообщение";
			this.AfterReceiveCheckBox.UseVisualStyleBackColor = true;
			this.AfterReceiveCheckBox.CheckedChanged += new System.EventHandler(this.AfterReceiveCheckBox_CheckedChanged);
			// 
			// BeforeSendAfterFilteringCheckBox
			// 
			this.BeforeSendAfterFilteringCheckBox.AutoSize = true;
			this.BeforeSendAfterFilteringCheckBox.Location = new System.Drawing.Point(6, 41);
			this.BeforeSendAfterFilteringCheckBox.Name = "BeforeSendAfterFilteringCheckBox";
			this.BeforeSendAfterFilteringCheckBox.Size = new System.Drawing.Size(281, 17);
			this.BeforeSendAfterFilteringCheckBox.TabIndex = 21;
			this.BeforeSendAfterFilteringCheckBox.Text = "Отправляемое сообщение после преобразования";
			this.BeforeSendAfterFilteringCheckBox.UseVisualStyleBackColor = true;
			this.BeforeSendAfterFilteringCheckBox.CheckedChanged += new System.EventHandler(this.BeforeSendAfterFilteringCheckBox_CheckedChanged);
			// 
			// BeforeSendBeforeFilteringCheckBox
			// 
			this.BeforeSendBeforeFilteringCheckBox.AutoSize = true;
			this.BeforeSendBeforeFilteringCheckBox.Location = new System.Drawing.Point(6, 19);
			this.BeforeSendBeforeFilteringCheckBox.Name = "BeforeSendBeforeFilteringCheckBox";
			this.BeforeSendBeforeFilteringCheckBox.Size = new System.Drawing.Size(263, 17);
			this.BeforeSendBeforeFilteringCheckBox.TabIndex = 20;
			this.BeforeSendBeforeFilteringCheckBox.Text = "Отправляемое сообщение до преобразования";
			this.BeforeSendBeforeFilteringCheckBox.UseVisualStyleBackColor = true;
			this.BeforeSendBeforeFilteringCheckBox.CheckedChanged += new System.EventHandler(this.BeforeSendBeforeFilteringCheckBox_CheckedChanged);
			// 
			// Linelabel1
			// 
			this.Linelabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Linelabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Linelabel1.Location = new System.Drawing.Point(7, 292);
			this.Linelabel1.Name = "Linelabel1";
			this.Linelabel1.Size = new System.Drawing.Size(751, 2);
			this.Linelabel1.TabIndex = 2;
			// 
			// EndPointsLabel
			// 
			this.EndPointsLabel.AutoSize = true;
			this.EndPointsLabel.BackColor = System.Drawing.Color.Transparent;
			this.EndPointsLabel.Location = new System.Drawing.Point(7, 220);
			this.EndPointsLabel.Name = "EndPointsLabel";
			this.EndPointsLabel.Size = new System.Drawing.Size(91, 13);
			this.EndPointsLabel.TabIndex = 23;
			this.EndPointsLabel.Text = "Конечные точки:";
			// 
			// EndPointsComboBox
			// 
			this.EndPointsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.EndPointsComboBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.EndPointsComboBox.FormattingEnabled = true;
			this.EndPointsComboBox.Location = new System.Drawing.Point(7, 236);
			this.EndPointsComboBox.MaxDropDownItems = 100;
			this.EndPointsComboBox.Name = "EndPointsComboBox";
			this.EndPointsComboBox.Size = new System.Drawing.Size(751, 22);
			this.EndPointsComboBox.TabIndex = 24;
			// 
			// WsdlExplorerButton
			// 
			this.WsdlExplorerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.WsdlExplorerButton.Location = new System.Drawing.Point(642, 264);
			this.WsdlExplorerButton.Name = "WsdlExplorerButton";
			this.WsdlExplorerButton.Size = new System.Drawing.Size(116, 23);
			this.WsdlExplorerButton.TabIndex = 25;
			this.WsdlExplorerButton.Text = "Wsdl explorer";
			this.WsdlExplorerButton.UseVisualStyleBackColor = true;
			this.WsdlExplorerButton.Click += new System.EventHandler(this.OnWsdlExplorerButtonClick);
			// 
			// exportNsiListButton
			// 
			this.exportNsiListButton.Location = new System.Drawing.Point(7, 300);
			this.exportNsiListButton.Name = "exportNsiListButton";
			this.exportNsiListButton.Size = new System.Drawing.Size(107, 23);
			this.exportNsiListButton.TabIndex = 26;
			this.exportNsiListButton.Text = "exportNsiList";
			this.exportNsiListButton.UseVisualStyleBackColor = true;
			this.exportNsiListButton.Click += new System.EventHandler(this.OnexportNsiList);
			// 
			// ExportDataProviderNsiItemComboBox
			// 
			this.ExportDataProviderNsiItemComboBox.FormattingEnabled = true;
			this.ExportDataProviderNsiItemComboBox.Location = new System.Drawing.Point(136, 338);
			this.ExportDataProviderNsiItemComboBox.Name = "ExportDataProviderNsiItemComboBox";
			this.ExportDataProviderNsiItemComboBox.Size = new System.Drawing.Size(81, 21);
			this.ExportDataProviderNsiItemComboBox.TabIndex = 27;
			// 
			// ServiceLabel
			// 
			this.ServiceLabel.AutoSize = true;
			this.ServiceLabel.BackColor = System.Drawing.Color.Transparent;
			this.ServiceLabel.Location = new System.Drawing.Point(7, 342);
			this.ServiceLabel.Name = "ServiceLabel";
			this.ServiceLabel.Size = new System.Drawing.Size(112, 13);
			this.ServiceLabel.TabIndex = 28;
			this.ServiceLabel.Text = "Номер справочника:";
			// 
			// exportDataProviderNsiItemButton
			// 
			this.exportDataProviderNsiItemButton.Location = new System.Drawing.Point(232, 337);
			this.exportDataProviderNsiItemButton.Name = "exportDataProviderNsiItemButton";
			this.exportDataProviderNsiItemButton.Size = new System.Drawing.Size(162, 23);
			this.exportDataProviderNsiItemButton.TabIndex = 29;
			this.exportDataProviderNsiItemButton.Text = "exportDataProviderNsiItem";
			this.exportDataProviderNsiItemButton.UseVisualStyleBackColor = true;
			this.exportDataProviderNsiItemButton.Click += new System.EventHandler(this.OnexportDataProviderNsiItem);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(7, 328);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(751, 2);
			this.label1.TabIndex = 30;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(7, 364);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(751, 2);
			this.label2.TabIndex = 31;
			// 
			// SelectCertificatesButton
			// 
			this.SelectCertificatesButton.Location = new System.Drawing.Point(343, 132);
			this.SelectCertificatesButton.Name = "SelectCertificatesButton";
			this.SelectCertificatesButton.Size = new System.Drawing.Size(155, 23);
			this.SelectCertificatesButton.TabIndex = 32;
			this.SelectCertificatesButton.Text = "Выбрать сертификаты...";
			this.SelectCertificatesButton.UseVisualStyleBackColor = true;
			this.SelectCertificatesButton.Click += new System.EventHandler(this.OnSelectCertificates);
			// 
			// CPtypeLabel
			// 
			this.CPtypeLabel.AutoSize = true;
			this.CPtypeLabel.BackColor = System.Drawing.Color.Transparent;
			this.CPtypeLabel.Location = new System.Drawing.Point(7, 176);
			this.CPtypeLabel.Name = "CPtypeLabel";
			this.CPtypeLabel.Size = new System.Drawing.Size(147, 13);
			this.CPtypeLabel.TabIndex = 33;
			this.CPtypeLabel.Text = "Модель криптопровайдера:";
			// 
			// ProviderModelComboBox
			// 
			this.ProviderModelComboBox.FormattingEnabled = true;
			this.ProviderModelComboBox.Location = new System.Drawing.Point(7, 195);
			this.ProviderModelComboBox.Name = "ProviderModelComboBox";
			this.ProviderModelComboBox.Size = new System.Drawing.Size(751, 21);
			this.ProviderModelComboBox.TabIndex = 34;
			this.ProviderModelComboBox.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(8, 170);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(751, 2);
			this.label3.TabIndex = 35;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::GisBusted.Properties.Resources.bk;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(766, 492);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ProviderModelComboBox);
			this.Controls.Add(this.CPtypeLabel);
			this.Controls.Add(this.SelectCertificatesButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.exportDataProviderNsiItemButton);
			this.Controls.Add(this.ServiceLabel);
			this.Controls.Add(this.ExportDataProviderNsiItemComboBox);
			this.Controls.Add(this.exportNsiListButton);
			this.Controls.Add(this.WsdlExplorerButton);
			this.Controls.Add(this.EndPointsComboBox);
			this.Controls.Add(this.EndPointsLabel);
			this.Controls.Add(this.Linelabel1);
			this.Controls.Add(this.SoapGroupBox);
			this.Controls.Add(this.orgPPAGUIDTextBox);
			this.Controls.Add(this.orgPPAGUIDLabel);
			this.Controls.Add(this.ConfigComboBox);
			this.Controls.Add(this.ConfigLabel);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(774, 519);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Кукла Ланита";
			this.Load += new System.EventHandler(this.OnLoad);
			this.SoapGroupBox.ResumeLayout(false);
			this.SoapGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.Label ConfigLabel;
		private System.Windows.Forms.ComboBox ConfigComboBox;
		private System.Windows.Forms.Label orgPPAGUIDLabel;
		private System.Windows.Forms.TextBox orgPPAGUIDTextBox;
		private System.Windows.Forms.GroupBox SoapGroupBox;
		private System.Windows.Forms.CheckBox AfterReceiveCheckBox;
		private System.Windows.Forms.CheckBox BeforeSendAfterFilteringCheckBox;
		private System.Windows.Forms.CheckBox BeforeSendBeforeFilteringCheckBox;
		private System.Windows.Forms.Label Linelabel1;
		private System.Windows.Forms.Label EndPointsLabel;
		private System.Windows.Forms.ComboBox EndPointsComboBox;
		private System.Windows.Forms.Button WsdlExplorerButton;
		private System.Windows.Forms.Button exportNsiListButton;
		private System.Windows.Forms.ComboBox ExportDataProviderNsiItemComboBox;
		private System.Windows.Forms.Label ServiceLabel;
		private System.Windows.Forms.Button exportDataProviderNsiItemButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button SelectCertificatesButton;
		private System.Windows.Forms.Label CPtypeLabel;
		private System.Windows.Forms.ComboBox ProviderModelComboBox;
		private System.Windows.Forms.Label label3;
		}
	}

