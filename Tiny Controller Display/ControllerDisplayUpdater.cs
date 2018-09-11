using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.XInput;
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
		private Controller controller;
		private State player = new State();
		private readonly Task updateTask;
		private Thickness noMargin = new Thickness(0), leftBumperPressed = new Thickness(1, 1, 0, 0), rightBumperPressed = new Thickness(-2, 1, 0, 0);//negative margins need to be doubled to display properly. 

		private readonly Dictionary<GamepadButtonFlags, Image[]> buttonsToImages;
		private Image dPad, leftBumper, rightBumper;
		private Stick leftStick, rightStick;
		private RectangleGeometry leftArcClip, rightArcClip;

		public ControllerDisplayUpdater(
			UserIndex userIndex,
			Dictionary<GamepadButtonFlags, Image[]> buttonsToImages,
			Image dPad, Image leftBumper, Image rightBumper,
			Stick leftStick, Stick rightStick,
			RectangleGeometry leftArcClip, RectangleGeometry rightArcClip) {
			controller = new Controller(userIndex);
			this.buttonsToImages = buttonsToImages;
			this.dPad = dPad;
			this.leftBumper = leftBumper;
			this.rightBumper = rightBumper;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			this.leftArcClip = leftArcClip;
			this.rightArcClip = rightArcClip;
			updateTask = BackgroundUpdate();
		}

		Thickness StickValueToMargin(short x, short y) {
			return new Thickness((x < 0 ? x * 2.0 / 32768.0 : x / 32767.0) * 2.0, (y < 0 ? y / 32768.0 : y * 2.0 / 32767.0) * -2.0, 0, 0);//negative margins need to be doubled to display properly. 
		}

		Thickness GetDPadMargin() {
			double topMargin = Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0) - Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0) * 2.0;
			double leftMargin = Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0) - Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0) * 2.0;
			return new Thickness(leftMargin, topMargin, 0, 0);
		}

		double TriggerToArcClipY(byte t) {
			return t / 255.0 * 15.0;
		}

		private void Update() {
			controller.GetState(out player);
			foreach(KeyValuePair<GamepadButtonFlags,Image[]> buttonToImages in buttonsToImages) {
				foreach(Image i in buttonToImages.Value) {
					if((player.Gamepad.Buttons & buttonToImages.Key) != 0) {
						i.Visibility = Visibility.Visible;
					} else {
						i.Visibility = Visibility.Hidden;
					}
				}
			}
			leftStick.unpressedSticktop.Margin = leftStick.pressedSticktop.Margin = StickValueToMargin(player.Gamepad.LeftThumbX, player.Gamepad.LeftThumbY);
			rightStick.unpressedSticktop.Margin = rightStick.pressedSticktop.Margin = StickValueToMargin(player.Gamepad.RightThumbX, player.Gamepad.RightThumbY);
			leftBumper.Margin = ((player.Gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0) ? leftBumperPressed : noMargin;
			rightBumper.Margin = ((player.Gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0) ? rightBumperPressed : noMargin;
			leftArcClip.Rect = new Rect(0, TriggerToArcClipY(player.Gamepad.LeftTrigger), 57, 37);
			rightArcClip.Rect = new Rect(0, TriggerToArcClipY(player.Gamepad.RightTrigger), 57, 37);
			dPad.Margin = GetDPadMargin();
		}

		private async Task BackgroundUpdate() {
			while(true) {
				Update();
				await Task.Delay(1);
			}
		}
	}
}
