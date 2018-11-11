using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.XInput;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Tiny_Controller_Display {
	struct Stick {
		public TranslateTransform unpressedSticktop, pressedSticktop;
		public double Dx {//x-offset to render at
			get {
				return unpressedSticktop.X;
			}
			set {
				unpressedSticktop.X = pressedSticktop.X = value;
			}
		}
		public double Dy {//y-offset to render at
			get {
				return unpressedSticktop.Y;
			}
			set {
				unpressedSticktop.Y = pressedSticktop.Y = value;
			}
		}
		public Stick(TranslateTransform unpressedSticktopTranslation, TranslateTransform pressedSticktopTranslation) {
			unpressedSticktop = unpressedSticktopTranslation;
			pressedSticktop = pressedSticktopTranslation;
		}
	}

	class ControllerDisplayUpdater {
		public Controller Controller { get; private set; }
		private State player = new State();
		private readonly Task updateTask;

		private readonly Dictionary<GamepadButtonFlags, Image[]> buttonsToImages;
		private TranslateTransform dPad, leftBumper, rightBumper;
		private Stick leftStick, rightStick;
		private RectangleGeometry leftArcClip, rightArcClip;

		public ControllerDisplayUpdater(
			UserIndex userIndex,
			Dictionary<GamepadButtonFlags, Image[]> buttonsToImages,
			TranslateTransform dPadTranslation, TranslateTransform leftBumperTranslation, TranslateTransform rightBumperTranslation,
			Stick leftStick, Stick rightStick,
			RectangleGeometry leftArcClip, RectangleGeometry rightArcClip) {
			Controller = new Controller(userIndex);
			this.buttonsToImages = buttonsToImages;
			dPad = dPadTranslation;
			leftBumper = leftBumperTranslation;
			rightBumper = rightBumperTranslation;
			this.leftStick = leftStick;
			this.rightStick = rightStick;
			this.leftArcClip = leftArcClip;
			this.rightArcClip = rightArcClip;
			updateTask = BackgroundUpdate();
		}

		(double, double) StickInputToDisplacement(short x, short y) {
			return ((x < 0 ? x / 32768.0 : x / 32767.0) * 2.0, (y < 0 ? y / 32768.0 : y / 32767.0) * -2.0);
		}

		double TriggerToArcClipY(byte t) {
			return t / 255.0 * 15.0;
		}

		private void Update() {
			Controller.GetState(out player);
			foreach(var buttonToImages in buttonsToImages) {
				foreach(Image i in buttonToImages.Value) {
					if((player.Gamepad.Buttons & buttonToImages.Key) != 0) {
						i.Visibility = Visibility.Visible;
					} else {
						i.Visibility = Visibility.Hidden;
					}
				}
			}
			(leftStick.Dx, leftStick.Dy) = StickInputToDisplacement(player.Gamepad.LeftThumbX, player.Gamepad.LeftThumbY);
			(rightStick.Dx, rightStick.Dy) = StickInputToDisplacement(player.Gamepad.RightThumbX, player.Gamepad.RightThumbY);
			(leftBumper.X, leftBumper.Y) = ((player.Gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0) ? (1, 1) : (0, 0);
			(rightBumper.X, rightBumper.Y) = ((player.Gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0) ? (-1, 1) : (0, 0);
			dPad.X = Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0) - Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0);
			dPad.Y = Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0) - Convert.ToDouble((player.Gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0);
			leftArcClip.Rect = new Rect(0, TriggerToArcClipY(player.Gamepad.LeftTrigger), 57, 37);
			rightArcClip.Rect = new Rect(0, TriggerToArcClipY(player.Gamepad.RightTrigger), 57, 37);
		}

		private async Task BackgroundUpdate() {
			while(true) {
				Update();
				await Task.Delay(1);
			}
		}
	}
}
