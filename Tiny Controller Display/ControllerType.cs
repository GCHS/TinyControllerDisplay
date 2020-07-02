using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_Controller_Display {
	class ControllerType : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public enum Types {
			XB1Elite, DS4Rev2, Dualsense
		};

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private Types _type = Types.XB1Elite;
		public Types Type { get => _type;
			set {
				_type = value;
				NotifyPropertyChanged();
			}
		}
		public string FolderName => Type switch	{
			Types.XB1Elite => "xb1_elite",
			Types.DS4Rev2 => "ds4_rev2",
			Types.Dualsense => "dualsense",
			_ => throw new NotImplementedException(),
		};
		public ControllerType() { }
		public ControllerType(string type) {
			Type = (Types)Enum.Parse(typeof(Types), type);
		}
	}
}
