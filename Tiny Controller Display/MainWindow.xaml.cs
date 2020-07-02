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

		private Dictionary<ControllerDisplay, Point> displayToPositionDelta = null;

		private static bool doMoveDisplaysTogether = true;

		public ControllerDisplay() : this(UserIndex.One) { }

		public ControllerDisplay(UserIndex userIndex) {
			InitializeComponent();
			userIndexToDisplay[userIndex] = this;
			Title = $"P{((int) userIndex)+1}";

			SyncToggles();
			
			Resources["artFolder"] = new ControllerType();

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
				dPadTranslation, leftBumperTranslation, rightBumperTranslation,
				new Stick(leftSticktop.RenderTransform as TranslateTransform, leftSticktopPressed.RenderTransform as TranslateTransform),
				new Stick(rightSticktop.RenderTransform as TranslateTransform, rightSticktopPressed.RenderTransform as TranslateTransform),
				leftTrigger.Clip as RectangleGeometry, rightTrigger.Clip as RectangleGeometry
			);

		}

		private void SyncToggles() {
			Controller1Toggle.IsChecked = userIndexToDisplay.ContainsKey(UserIndex.One);
			Controller2Toggle.IsChecked = userIndexToDisplay.ContainsKey(UserIndex.Two);
			Controller3Toggle.IsChecked = userIndexToDisplay.ContainsKey(UserIndex.Three);
			Controller4Toggle.IsChecked = userIndexToDisplay.ContainsKey(UserIndex.Four);

			MoveDisplaysTogetherToggle.IsChecked = doMoveDisplaysTogether;
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				if(doMoveDisplaysTogether) {
					displayToPositionDelta = new Dictionary<ControllerDisplay, Point>();
					foreach(var display in userIndexToDisplay.Values) {
							displayToPositionDelta[display] = new Point(display.Left - Left, display.Top - Top);
					}
					LocationChanged += Window_LocationChanged;
				}
				DragMove();
				LocationChanged -= Window_LocationChanged;
			}
		}
		private void Window_LocationChanged(object sender, EventArgs e) {
			if(doMoveDisplaysTogether) {
				foreach(var displayAndDelta in displayToPositionDelta) {
					displayAndDelta.Key.Left = displayAndDelta.Value.X + Left;
					displayAndDelta.Key.Top = displayAndDelta.Value.Y + Top;
				}
			}
		}

		private void ToggleDisplay(UserIndex userIndex) {
			lock(userIndexToDisplay) {
				if(!userIndexToDisplay.ContainsKey(userIndex)) {//toggling on
					(userIndexToDisplay[userIndex] = new ControllerDisplay(userIndex) {
						Top = Top,
						Left = NewDisplayLeftFromUserIndex(userIndex)
					}).Show();
					SyncTogglesOnAllDisplays();
				} else {//toggling off
					userIndexToDisplay[userIndex].Close();//display removes itself on closure and updates all toggles
				}
			}
		}

		private static void SyncTogglesOnAllDisplays() {
			foreach(ControllerDisplay display in userIndexToDisplay.Values) {
				display.SyncToggles();
			}
		}

		private double NewDisplayLeftFromUserIndex(UserIndex userIndex) {
			return Left + Width * ((int)userIndex - (int)displayUpdater.Controller.UserIndex) * 1.25;
		}

		private void Controller1Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.One);
		private void Controller2Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Two);
		private void Controller3Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Three);
		private void Controller4Toggle_Click(object sender, RoutedEventArgs e) => ToggleDisplay(UserIndex.Four);

		private void UnifiedMovementToggle_Click(object sender, RoutedEventArgs e) {
			lock(userIndexToDisplay) {
				doMoveDisplaysTogether = !doMoveDisplaysTogether;
				SyncTogglesOnAllDisplays();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			userIndexToDisplay.Remove(displayUpdater.Controller.UserIndex);
			SyncTogglesOnAllDisplays();
		}

		private void controllerTypeChanger_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			Resources["artFolder"] = ((sender as ListBox).SelectedItem as ListBoxItem).Tag;
		}
	}
}
