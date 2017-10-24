using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SharpDX.XInput;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tiny_Controller_Display {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private ControllerDisplayUpdater displayUpdater;

		public MainWindow() {
			InitializeComponent();
			displayUpdater = new ControllerDisplayUpdater(
			new Dictionary<GamepadButtonFlags, Image[]>(){
				{GamepadButtonFlags.A, new Image[1]{bottomButton}},
				{GamepadButtonFlags.B, new Image[1]{rightButton}},
				{GamepadButtonFlags.X, new Image[1]{leftButton}},
				{GamepadButtonFlags.Y, new Image[1]{topButton}},
				{GamepadButtonFlags.LeftThumb, new Image[2]{leftSticktopPressed, leftStickwellPressed}},
				{GamepadButtonFlags.RightThumb, new Image[2]{rightSticktopPressed, rightStickwellPressed}},
				{GamepadButtonFlags.Start, new Image[1]{startButton}},
				{GamepadButtonFlags.Back, new Image[1]{selectButton}}
				},
			dPad, leftBumper, rightBumper, new Stick(leftSticktop,leftSticktopPressed), new Stick(rightSticktop,rightSticktopPressed)
			);
		}
		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left)
				DragMove();
		}

	}
}
