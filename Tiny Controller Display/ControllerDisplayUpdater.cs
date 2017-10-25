using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Tiny_Controller_Display {
	struct Stick {
		public Image unpressedSticktop, pressedSticktop;
		public Stick(Image unpressedSticktop, Image pressedSticktop) {
			this.unpressedSticktop = unpressedSticktop;
			this.pressedSticktop = pressedSticktop;
		}
	}

	class ControllerDisplayUpdater {
		private Controller controller1=new Controller(UserIndex.One);
		private State player1 = new State();
		private Task updateTask;
		private Thickness noMargin = new Thickness(0), leftBumperPressed = new Thickness(1, 1, 0, 0), rightBumperPressed = new Thickness(-2, 1, 0, 0);//for some reason, negative margins need to be doubled to display properly. 

		private Dictionary<GamepadButtonFlags, Image[]> buttonsToImages;
		private Image dPad, leftBumper, rightBumper;
		private Stick leftStick, rightStick;
		private RectangleGeometry leftArcClip, rightArcClip;

		public ControllerDisplayUpdater(
			Dictionary<GamepadButtonFlags, Image[]> buttonsToImages,
			Image dPad, Image leftBumper, Image rightBumper,
			Stick leftStick, Stick rightStick,
			RectangleGeometry leftArcClip, RectangleGeometry rightArcClip) {
			this.buttonsToImages = buttonsToImages;
			this.dPad = dPad;
			this.leftBumper = leftBumper;
			this.rightBumper = rightBumper;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			this.leftArcClip = leftArcClip;
			this.rightArcClip = rightArcClip;
		}

		Thickness StickValueToMargin(short x, short y) {
			return new Thickness((x < 0 ? x * 2.0 / 32768.0 : x / 32767.0) * 2.0, (y < 0 ? y / 32768.0 : y * 2.0 / 32767.0) * -2.0, 0, 0);//for some reason, negative margins need to be doubled to display properly. 
		}

		Thickness GetDPadMargin() {
			double topMargin = Convert.ToDouble((player1.Gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0) - Convert.ToDouble((player1.Gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0) * 2.0;
			double leftMargin = Convert.ToDouble((player1.Gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0) - Convert.ToDouble((player1.Gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0) * 2.0;
			return new Thickness(leftMargin, topMargin, 0, 0);
		}

		double TriggerToArcClipY(byte t) {
			return t / 255.0 * 15.0;
		}

		public void Update() {
			controller1.GetState(out player1);
			foreach(KeyValuePair<GamepadButtonFlags,Image[]> buttonToImages in buttonsToImages) {
				foreach(Image i in buttonToImages.Value) {
					if((player1.Gamepad.Buttons & buttonToImages.Key) != 0) {
						i.Visibility = Visibility.Visible;
					} else {
						i.Visibility = Visibility.Hidden;
					}
				}
			}
			leftStick.unpressedSticktop.Margin = leftStick.pressedSticktop.Margin = StickValueToMargin(player1.Gamepad.LeftThumbX, player1.Gamepad.LeftThumbY);
			rightStick.unpressedSticktop.Margin = rightStick.pressedSticktop.Margin = StickValueToMargin(player1.Gamepad.RightThumbX, player1.Gamepad.RightThumbY);
			leftBumper.Margin = ((player1.Gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0) ? leftBumperPressed : noMargin;
			rightBumper.Margin = ((player1.Gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0) ? rightBumperPressed : noMargin;
			leftArcClip.Rect = new Rect(0, TriggerToArcClipY(player1.Gamepad.LeftTrigger), 57, 37);
			rightArcClip.Rect = new Rect(0, TriggerToArcClipY(player1.Gamepad.RightTrigger), 57, 37);
			dPad.Margin = GetDPadMargin();
		}
	}
}
