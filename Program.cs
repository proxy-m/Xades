using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GisBusted
	{
	internal static class Program
		{
		/// <summary>
		/// Инициализация библиотеки - должна вызываться первой
		/// </summary>
		public static bool Init()
			{
			bool bres = false;

			GisGlobals.ApplicationIcon = GisGlobals.LoadIcon("icon.ico");

			bres = GisGlobals.ConfigFiles.Init();

			if (!bres)
				{
				GisGlobals.ErrorMessageBox("Ошибка поиска файлов инициализации");
				return bres;
				}

			List<WCF.GisBustedProxyConfigFile> ConfigFilesList = GisGlobals.ConfigFiles.ConfigFiles;
			WCF.GisBustedProxyConfigFile ConfigFile = GisGlobals.ConfigFiles.DefaultProxyConfigFile;

			bres = GisGlobals.InitProxyConfiguration(ConfigFile);
			if (!bres)
				{
				GisGlobals.ErrorMessageBox("Ошибка чтения настроек Proxy");
				return bres;
				}

			bres = GisGlobals.ConfigFileSettings.Init();
			if (!bres)
				{
				GisGlobals.ErrorMessageBox("Ошибка ConfigFileSettings.Init();");
				return bres;
				}

			bres = GisGlobals.ConfigureWCF();
			if (!bres)
				{
				GisGlobals.ErrorMessageBox("Ошибка GisGlobals.ConfigureWCF();");
				return bres;
				}

			bres = GisGlobals.SelectCertificates();
			if (!bres)
				{
				GisGlobals.ErrorMessageBox("Ошибка GisGlobals.SelectCertificates();");
				return bres;
				}

			return bres;
			}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
			{
			if (!Init())
				{
				return;
				}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
			}
		} // end class
	}
