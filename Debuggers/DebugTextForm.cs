using System;
using System.Windows.Forms;

namespace GisBusted.Debuggers
	{
	/// <summary>
	/// Показать отладочное окно с моноширинным текстом
	/// </summary>
	public partial class DebugTextForm : Form
		{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="Caption">Заголовок окна</param>
		/// <param name="Context">Содержимое окна</param>
		public DebugTextForm(string Caption, string Context)
			{
			InitializeComponent();
			if (GisGlobals.ApplicationIcon != null)
				{
				this.Icon = GisGlobals.ApplicationIcon;
				}

			if (Caption != null)
				{
				this.Text = Caption;
				}

			if (Context != null)
				{
				this.DataTextBox.Text = Context;
				this.DataTextBox.Select(0, 0);
				}

			WordWrapCheckBox.Checked = this.DataTextBox.WordWrap;
			Focus();
			}

		private void OnWordWrapCheckedChanged(object sender, EventArgs e)
			{
			this.DataTextBox.WordWrap = WordWrapCheckBox.Checked;
			}

		private void OnLoad(object sender, EventArgs e)
			{
			this.Icon = GisGlobals.ApplicationIcon;
			this.ShowIcon = true;
			}
		}
	}
