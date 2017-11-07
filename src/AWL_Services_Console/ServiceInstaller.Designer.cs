using System;
using System.Configuration;
using System.Reflection;

namespace AWL.Services.Console
{
	partial class ServiceInstaller
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.mainServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			// 
			// mainServiceInstaller
			// 
			this.mainServiceInstaller.Description = "AWL Services";
			this.mainServiceInstaller.DisplayName = DisplayName;
			this.mainServiceInstaller.ServiceName = ServiceName;
			// 
			// mainServiceProcessInstaller
			// 
			this.mainServiceProcessInstaller.Password = null;
			this.mainServiceProcessInstaller.Username = null;
			// 
			// ServiceInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.mainServiceInstaller,
            this.mainServiceProcessInstaller});

		}

		#endregion

		public System.ServiceProcess.ServiceInstaller mainServiceInstaller;
		public System.ServiceProcess.ServiceProcessInstaller mainServiceProcessInstaller;


		private string ServiceName
		{
			get
			{
				

				return GetConfigValue("serviceName"); //(System.Configuration.ConfigurationManager.AppSettings["ServiceName"] == null ? "AWL Services" : ConfigurationManager.AppSettings["ServiceName"].ToString());
			}
		}

		private string DisplayName
		{
			get
			{
				return GetConfigValue("displayName"); //(System.Configuration.ConfigurationManager.AppSettings["DisplayName"] == null ? "AWL Services" : ConfigurationManager.AppSettings["DisplayName"].ToString());
			}
		}

		private string GetConfigValue(string key)
		{
			Assembly service = Assembly.GetAssembly(typeof(ServiceInstaller));
			Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
			if (config.AppSettings.Settings[key] != null)
			{
				return config.AppSettings.Settings[key].Value;
			}
			else
			{
				throw new IndexOutOfRangeException
					("Settings collection does not contain the requested key: " + key);
			}
		}
	}
}