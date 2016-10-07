using Common.Structures.Local;
using Common.Structures.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for SeaBoard.xaml
    /// </summary>
    public partial class SeaBoard : Grid
    {
        private SeaBoardCell[,] _cells;
        public SeaBoardCell this[int x, int y]
        {
            get { return _cells[x, y]; }
        }
        public int BoardSize { get; protected set; }

        public SeaBoard()
        {
            InitializeComponent();
            this.BoardSize = 10;
            ReBuildContents();
        }

        public class SeaBoardCell
        {
            private SeaCellState _value;
            private SeaCellTargetMode _borderValue;

            public SeaCellState Value
            {
                get { return _value; }
                set
                {
                    _control.Background = new SolidColorBrush(SeaBoard.CellBackgroundColors[value]);
                    _value = value;
                }
            }
            public SeaCellTargetMode BorderValue
            {
                get { return _borderValue; }
                set
                {
                    _control.BorderBrush = new SolidColorBrush(SeaBoard.CellBorderColors[value]);
                    _borderValue = value;
                }
            }

            public int X { get; private set; }
            public int Y { get; private set; }

            SeaBoard _owner;
            UserControl _control;

            public UserControl Control { get { return _control; } }

            public SeaBoardCell(SeaBoard owner, int x, int y)
            {
                _owner = owner;

                this.X = x;
                this.Y = y;

                _control = new UserControl()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new SolidColorBrush(SeaBoard.CellBackgroundColors[SeaCellState.SEA]),
                    BorderThickness = new Thickness(1.0),
                    BorderBrush = Brushes.Transparent
                };
            }

            public void AnimateState(SeaCellState newState)
            {
                this._value = newState;
                ColorAnimation ca = new ColorAnimation();
                ca.From = (this._control.Background as SolidColorBrush).Color;
                ca.To = SeaBoard.CellBackgroundColors[newState];
                ca.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                this._control.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }

            public void AnimateBorder(SeaCellTargetMode newBorder)
            {
                this._borderValue = newBorder;
                ColorAnimation ca = new ColorAnimation();
                ca.From = (this._control.BorderBrush as SolidColorBrush).Color;
                ca.To = SeaBoard.CellBorderColors[newBorder];
                ca.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                this._control.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
        }
        public class SeaBoardCellEventArgs : EventArgs
        {
            public SeaBoardCell Cell { get; private set; }
            public MouseEventArgs MouseInfo { get; private set; }

            public MouseButton? Button { get; private set; }

            public SeaBoardCellEventArgs(SeaBoardCell cell, MouseEventArgs mouseInfo)
            {
                this.Cell = cell;
                this.MouseInfo = mouseInfo;
                this.Button = (mouseInfo is MouseButtonEventArgs) ? (mouseInfo as MouseButtonEventArgs).ChangedButton : (MouseButton?)null;
            }
        }
        public event EventHandler<SeaBoardCellEventArgs> OnCellMouseUp = delegate { };
        public event EventHandler<SeaBoardCellEventArgs> OnCellMouseEnter = delegate { };
        public event EventHandler<SeaBoardCellEventArgs> OnCellMouseLeave = delegate { };

        private void ReBuildContents()
        {
            contents.Children.Clear();
            contents.RowDefinitions.Clear();
            contents.ColumnDefinitions.Clear();

            int sz = this.BoardSize;

            _cells = new SeaBoardCell[sz, sz];

            contents.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            contents.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            for (int i = 0; i < sz; i++)
            {
                contents.RowDefinitions.Add(new RowDefinition());
                contents.ColumnDefinitions.Add(new ColumnDefinition());

                var hh = new UserControl()
                {
                    Content = ((char)('A' + i)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                contents.Children.Add(hh);
                Grid.SetRow(hh, 0);
                Grid.SetColumn(hh, i + 1);

                var vh = new UserControl()
                {
                    Content = (i + 1).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                contents.Children.Add(vh);
                Grid.SetRow(vh, i + 1);
                Grid.SetColumn(vh, 0);
            }

            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    var cell = new SeaBoardCell(this, x, y);
                    _cells[x, y] = cell;
                    contents.Children.Add(cell.Control);
                    Grid.SetRow(cell.Control, y + 1);
                    Grid.SetColumn(cell.Control, x + 1);
                    cell.Control.PreviewMouseDown += (s, e) => e.Handled = true;
                    cell.Control.MouseUp += (s, e) => this.OnCellMouseUp(this, new SeaBoardCellEventArgs(cell, e));
                    cell.Control.MouseEnter += (s, e) => this.OnCellMouseEnter(this, new SeaBoardCellEventArgs(cell, e));
                    cell.Control.MouseLeave += (s, e) => this.OnCellMouseLeave(this, new SeaBoardCellEventArgs(cell, e));
                }
            }
        }


        public static readonly Dictionary<SeaCellState, Color> CellBackgroundColors = new Dictionary<SeaCellState, Color>()
        {
            { SeaCellState.SEA, Colors.SkyBlue },
            { SeaCellState.FIRE_MISSED, Colors.LightYellow },
            { SeaCellState.SHIP, Colors.Green },
            { SeaCellState.SHIP_HIT, Colors.Red},
            { SeaCellState.SHIP_SUNK, Colors.DarkRed }
        };

        public static readonly Dictionary<SeaCellTargetMode, Color> CellBorderColors = new Dictionary<SeaCellTargetMode, Color>()
        {
            { SeaCellTargetMode.NONE, Colors.Transparent },
            { SeaCellTargetMode.FIRE_TARGET, Colors.Salmon }
        };

    }
}
