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
using System.Windows.Interop;

namespace Tiny_Controller_Display {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private ControllerDisplayUpdater displayUpdater;
		private double dpiX, dpiY;
		public MainWindow() {
			InitializeComponent();
			{
				var source = PresentationSource.FromVisual(controllerDisplay);
				Matrix scaleMatrix;
				if(source != null) {
					scaleMatrix = source.CompositionTarget.TransformToDevice;
				} else {
					using(var src = new HwndSource(new HwndSourceParameters())) {
						scaleMatrix = src.CompositionTarget.TransformToDevice;
					}
				}
				dpiX = scaleMatrix.M11;
				dpiY = scaleMatrix.M22;
			}
			
			//foreach(Image i in new Image[18] {
			//		leftBumper, rightBumper, faceplate, bottomButton,
			//		dPad, leftButton, leftStickwellPressed, leftSticktop,
			//		leftSticktopPressed, rightButton, rightStickwellPressed,
			//		rightSticktop, rightSticktopPressed, selectButton,
			//		startButton, topButton, leftTrigger, rightTrigger }) {
			//	i.Width /= dpiX;
			//	i.Height /= dpiY;
			//}

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
				dPad, leftBumper, rightBumper,
				new Stick(leftSticktop, leftSticktopPressed), new Stick(rightSticktop, rightSticktopPressed),
				(leftTrigger.Clip as RectangleGeometry), (rightTrigger.Clip as RectangleGeometry)
			);
		}
		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left)
				DragMove();
		}

	}
}
