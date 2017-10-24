using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

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
		private State player1;
		private Task updateTask;
		private Thickness noMargin = new Thickness(0), leftBumperPressed = new Thickness(1, 1, 0, 0), rightBumperPressed = new Thickness(0, 1, 0, 1);

		public Dictionary<GamepadButtonFlags, Image[]> buttonsToImages;
		public Image dPad, leftBumper, rightBumper;
		public Stick leftStick, rightStick;

		public ControllerDisplayUpdater(Dictionary<GamepadButtonFlags, Image[]> buttonsToImages, Image dPad, Image leftBumper, Image rightBumper, Stick leftStick, Stick rightStick) {
			this.buttonsToImages = buttonsToImages;
			this.dPad = dPad;
			this.leftBumper = leftBumper;
			this.rightBumper = rightBumper;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			player1 = controller1.GetState();
		}

		Thickness StickValueToMargin(short x, short y) {
			return new Thickness((x < 0 ? x / 32768.0 : x / 32767.0) * 2.0, (y < 0 ? y / 32768.0 : y / 32767.0) * 2.0, 0, 0);
		}

		void Update() {
			player1 = controller1.GetState();
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

		}
	}
}
