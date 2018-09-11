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
	public partial class ControllerDisplay : Window {
		private ControllerDisplayUpdater displayUpdater;

		private static Dictionary<UserIndex, ControllerDisplay> userIndexToDisplay = new Dictionary<UserIndex, ControllerDisplay>();

		public ControllerDisplay() : this(UserIndex.One) { }

		public ControllerDisplay(UserIndex userIndex) {
			InitializeComponent();
			userIndexToDisplay[userIndex] = this;

			if(userIndexToDisplay.ContainsKey(UserIndex.One))   { Controller1Toggle.IsChecked   = true; }
			if(userIndexToDisplay.ContainsKey(UserIndex.Two))   { Controller2Toggle.IsChecked   = true; }
			if(userIndexToDisplay.ContainsKey(UserIndex.Three)) { Controller3Toggle.IsChecked   = true; }
			if(userIndexToDisplay.ContainsKey(UserIndex.Four))  { Controller4Toggle.IsChecked   = true; }

			displayUpdater = new ControllerDisplayUpdater(
				userIndex,
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
				dPad, leftBumper, rightBumper,
				new Stick(leftSticktop, leftSticktopPressed), new Stick(rightSticktop, rightSticktopPressed),
				leftTrigger.Clip as RectangleGeometry, rightTrigger.Clip as RectangleGeometry
			);
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private void ToggleDisplay(UserIndex userIndex) {
			lock(userIndexToDisplay) {
				if(!userIndexToDisplay.ContainsKey(userIndex)) {
					(userIndexToDisplay[userIndex] = new ControllerDisplay(userIndex) {
						Top = Top,
						Left = Left + Math.Floor(Width * 1.25)
					}).Show();
				}else{//toggling off
					userIndexToDisplay[userIndex].Close();
					userIndexToDisplay.Remove(userIndex);
				}
			}
		}


		private void Controller1Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.One);
		private void Controller2Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Two);
		private void Controller3Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Three);
		private void Controller4Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Four);
	}
}
