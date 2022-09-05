using System;
using System.Runtime.InteropServices;

namespace Crypto.CryptoProviders
	{
	/// <summary>
	/// Вспомогательный класс для привязки объекта в пуле памяти
	/// </summary>
	public sealed class AutoPinner : IDisposable
		{
		/// <summary>
		/// Привязанный объект
		/// </summary>
		private GCHandle _pinnedArray;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="obj">Привязываемый объект</param>
		public AutoPinner(object obj)
			{
			_pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
			}

		/// <summary>
		/// Оператор приведения к IntPtr
		/// </summary>
		/// <param name="ap"></param>
		public static implicit operator IntPtr(AutoPinner ap)
			{
			return ap._pinnedArray.AddrOfPinnedObject();
			}

		#region Реализация интерфейса IDisposable

		/// <summary>
		/// Флаг для отслеживания того, что Dispose уже вызывался
		/// </summary>
		private bool m_Disposed;

		/// <summary>
		/// Свойство для флага m_Disposed
		/// </summary>
		public bool Disposed
			{
			get
				{
				return m_Disposed;
				}

			private set
				{
				m_Disposed = value;
				}
			}

		/// <summary>
		/// Реализация Dispose()
		/// </summary>
		public void Dispose()
			{
			Dispose(true);
			// Объект будет освобожден при помощи метода Dispose, следовательно необходимо вызвать
			// GC.SupressFinalize чтобы убрать этот объект из очереди финализатора и предотвратить
			// вызов финализатора во второй раз
			GC.SuppressFinalize(this);
			}

		/// <summary>
		/// Метод Dispose(bool disposing) выполняется в двух различных вариантах: Если
		/// disposing=true, то этот метод вызывается (прямо или косвенно) из пользовательского кода.
		/// Управляемые и неуправляемые ресурсы могут быть освобождены. Если disposing=false, то этот
		/// метод вызывается рантаймом из финализатора и могут освобождаться только неуправляемые ресурсы.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
			{
			// Проверка что Dispose(bool) уже вызывался
			if (this.Disposed)
				{
				return;
				}

			// Если disposing=true, то освобождаем все (управляемые и неуправляемые) ресурсы
			if (disposing)
				{
				// Освобождаем управляемые ресурсы
				// DisposeManagedResources();
				}

			// Вызываем соответствующие методы для освобождения неуправляемых ресурсов
			_pinnedArray.Free();

			// Ставим флаг завершения
			Disposed = true;
			}

		#endregion Реализация интерфейса IDisposable

		/// <summary>
		/// Привязанный объект
		/// </summary>
		public object Target
			{
			get
				{
				return _pinnedArray.Target;
				}
			}
		}
	}
