using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient.ViewModel;

namespace WPFClient.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow(User selectedUser)
        {
            InitializeComponent();
            var inst = (GameViewModel)this.DataContext;
            inst.SelectedUser = selectedUser;
            inst.PlayRollDice += (sender, e) =>
            {
                RollDice.Position = TimeSpan.Zero;
                RollDice.Play();
            };
            inst.PlayMoveChip += (sender, e) =>
            {
                MoveChip.Position = TimeSpan.Zero;
                MoveChip.Play();
            };
        }

    }
}
