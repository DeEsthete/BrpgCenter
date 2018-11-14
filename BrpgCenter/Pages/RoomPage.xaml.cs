using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrpgCenter
{
    /// <summary>
    /// Логика взаимодействия для RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        private const int SLEEP_TIME_COMMON = 2000;
        private const int SLEEP_TIME_IMPORTANT = 500;

        private MainPocket pocket;
        private Room room;
        private bool isHost;
        private ChatClient chatClient;
        private StateClient stateClient;
        private CharactersClient charactersClient;
        private StorageClient storageClient;
        private Paint paint;

        public RoomPage(MainPocket pocket, ChatClient chatClient, Room room, bool isHost, Character character)
        {
            InitializeComponent();
            this.pocket = pocket;
            this.room = room;
            this.isHost = isHost;
            this.chatClient = chatClient;
            this.stateClient = new StateClient(room.Ip, room.Port, pocket.Player, character);
            this.charactersClient = new CharactersClient(room.Ip, room.Port, pocket.Player, character);
            this.storageClient = new StorageClient(room.Ip, room.Port, pocket.Player, character);
            paint = new Paint();
            
            AcceptServerFirstMessage();

            chatClient.ReceiveMessage();
            AddMessageToListBox();

            stateClient.StartReceive();
            ApplyPlayersInRoom();

            charactersClient.StartReceive();
            ApplyCharactersInRoom();

            storageClient.StartReceive();
            ApplyFilesInStorage();

            colorPicker.SelectedColor = paint.Color;

            if (room.GameMaster != null)
            {
                masterPlayerName.Text = room.GameMaster.NickName;
            }
        }

        private async void AcceptServerFirstMessage()
        {
            chatClient.AcceptServerFirstMessage();
            if (chatClient.IsConnected)
            {
                await AcceptServerFirstMessageWork();
            }
        }

        private Task AcceptServerFirstMessageWork()
        {
            return Task.Run(() =>
            {
                ServerFirstMessage firstMessage = chatClient.ServerFirstMessage;
                bool serverMessageIsOk = false;
                while (!serverMessageIsOk)
                {
                    Thread.Sleep(500);
                    firstMessage = chatClient.ServerFirstMessage;
                    if (firstMessage != null)
                    {
                        serverMessageIsOk = true;
                        if (firstMessage.Master != null && firstMessage.Master.NickName != "")
                        {
                            Dispatcher.Invoke(() => masterPlayerName.Text = firstMessage.Master.NickName);
                        }
                    }
                }
                foreach (var i in firstMessage.CharacterInRoom)
                {
                    Dispatcher.Invoke(() => playersListBox.Items.Add("Id: " + i.Id + " Имя: " + i.FullName));
                }
            });
        }

        #region PaintMethods
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                paint.CurrentPoint = e.GetPosition(canvas);
        }
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    Stroke = new SolidColorBrush(paint.Color),
                    X1 = paint.CurrentPoint.X,
                    Y1 = paint.CurrentPoint.Y,
                    X2 = e.GetPosition(canvas).X,
                    Y2 = e.GetPosition(canvas).Y
                };

                paint.CurrentPoint = e.GetPosition(canvas);
                canvas.Children.Add(line);
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<Line> lines = new List<Line>();
            foreach (var obj in canvas.Children)
            {
                if (obj is Line line)
                {
                    lines.Add(line);
                }
            }

            paint.Mementos.Add(new Memento(lines));
            paint.RemovedMemento.Clear();
        }

        private void CanvasMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (paint.Mementos.Any())
            {
                if (paint.Mementos.Count == 1)
                {
                    ToMemento(paint.Mementos[0]);
                }
                else
                {
                    paint.RemovedMemento.Add(paint.Mementos.Last());
                    paint.Mementos.Remove(paint.Mementos.Last());
                    if (paint.Mementos.Any())
                    {
                        ToMemento(paint.Mementos.Last());
                    }
                }
            }
        }

        private void ToMemento(Memento memento)
        {
            canvas.Children.Clear();
            foreach (var line in memento.Lines)
            {
                canvas.Children.Add(line);
            }
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (paint.RemovedMemento.Any())
            {
                ToMemento(paint.RemovedMemento.Last());
                paint.Mementos.Add(paint.RemovedMemento.Last());
                paint.RemovedMemento.Remove(paint.RemovedMemento.Last());
            }
        }

        private void ColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                paint.Color = e.NewValue.Value;
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            
            //SaveFileDialog fileDialog = new SaveFileDialog
            //{
            //    FileName = "image",
            //    DefaultExt = ".png",
            //    Filter = "Png images (.png)|*.png"
            //};

            //bool? result = fileDialog.ShowDialog();

            //if (result == true)
            //{
            //    string fileName = fileDialog.FileName;

            //    Rect rect = new Rect(canvas.Margin.Left, canvas.Margin.Top, canvas.ActualWidth, canvas.ActualHeight);
            //    double dpi = 96d;

            //    RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, dpi, dpi, PixelFormats.Default);
            //    rtb.Render(canvas);

            //    BitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(rtb));
            //    try
            //    {
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            encoder.Save(stream);
            //            File.WriteAllBytes(fileName, stream.ToArray());
            //        }
            //        MessageBox.Show("Сохранено!");
            //    }
            //    catch (Exception error)
            //    {
            //        MessageBox.Show(error.Message);
            //    }
            //}
        }
        #endregion

        #region timerMethods

        private async void AddMessageToListBox()
        {
            if (chatClient.IsConnected)
            {
                await AddMessageToListBoxWork(null);
            }
        }

        private Task AddMessageToListBoxWork(object obj)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(SLEEP_TIME_IMPORTANT);
                    foreach (var i in chatClient.Messages)
                    {
                        Dispatcher.Invoke(() => chatListBox.Items.Insert(0,i.SenderName + ": " + i.Content));
                    }
                    chatClient.Messages.Clear();
                }
            });
        }

        private async void ApplyPlayersInRoom()
        {
            await ApplyPlayersInRoomWork();
        }

        private Task ApplyPlayersInRoomWork()
        {
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Dispatcher.Invoke(() => playersListBox.Items.Clear());
                        foreach (var i in stateClient.PlayersInRoom)
                        {
                            Dispatcher.Invoke(() => playersListBox.Items.Insert(0, i.NickName));
                        }
                        //stateClient.PlayersInRoom.Clear();
                        Thread.Sleep(SLEEP_TIME_COMMON);
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        private async void ApplyCharactersInRoom()
        {
            await ApplyCharactersInRoomWork();
        }

        private Task ApplyCharactersInRoomWork()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    object currentSelected = Dispatcher.Invoke(() => charactersListBox.SelectedItem);
                    Dispatcher.Invoke(() => charactersListBox.Items.Clear());
                    foreach (var i in charactersClient.CharactersInRoom)
                    {
                        Dispatcher.Invoke(() => charactersListBox.Items.Insert(0, "Владелец: " + i.Owner.NickName + " Имя: " + i.FullName));
                    }
                    Dispatcher.Invoke(() => charactersListBox.SelectedItem = currentSelected);
                    Thread.Sleep(SLEEP_TIME_COMMON);
                }
            });
        }

        public async void ApplyFilesInStorage()
        {
            await ApplyFilesInStorageWork();
        }

        private Task ApplyFilesInStorageWork()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    object currentSelected = Dispatcher.Invoke(() => storageListBox.SelectedItem);
                    Dispatcher.Invoke(() => storageListBox.Items.Clear());
                    foreach (var i in storageClient.FileList)
                    {
                        Dispatcher.Invoke(() => storageListBox.Items.Insert(0, "Имя: " + i.Name + " Размер: " + i.Length));
                    }
                    Dispatcher.Invoke(() => storageListBox.SelectedItem = currentSelected);
                    Thread.Sleep(SLEEP_TIME_COMMON);
                }
            });
        }
        #endregion

        #region ButtonMethods
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            chatClient.SendMessage(new ChatMessage(pocket.Player.NickName, messageFieldTextBox.Text));
            chatListBox.Items.Add("Вы: " + messageFieldTextBox.Text);
            messageFieldTextBox.Text = "";
        }

        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            pocket.MainWindow.Content = new RoomsPage(pocket);
        }

        private void GoChatButtonClick(object sender, RoutedEventArgs e)
        {
            paintGrid.Visibility = Visibility.Hidden;
            charactersGrid.Visibility = Visibility.Hidden;
            storageGrid.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Visible;
        }

        private void GoCharactersButtonClick(object sender, RoutedEventArgs e)
        {
            paintGrid.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Hidden;
            storageGrid.Visibility = Visibility.Hidden;
            charactersGrid.Visibility = Visibility.Visible;
        }

        private void GoStorageButtonClick(object sender, RoutedEventArgs e)
        {
            paintGrid.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Hidden;
            charactersGrid.Visibility = Visibility.Hidden;
            storageGrid.Visibility = Visibility.Visible;
        }

        public void GoCanvasButtonClick(object sender, RoutedEventArgs e)
        {
            chatGrid.Visibility = Visibility.Hidden;
            charactersGrid.Visibility = Visibility.Hidden;
            storageGrid.Visibility = Visibility.Hidden;
            paintGrid.Visibility = Visibility.Visible;
        }

        private void EditCharacterButtonClick(object sender, RoutedEventArgs e)
        {
            if (charactersListBox.SelectedIndex != -1)
            {
                Character character = charactersClient.CharactersInRoom[charactersListBox.SelectedIndex];
                pocket.CurrentRoom = this;
                pocket.MainWindow.Content = new CharacterPage(pocket, character, charactersClient);
            }
        }

        private void DownloadFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (storageListBox.SelectedIndex != -1)
            {
                storageClient.DownloadFile(storageClient.FileList[storageListBox.SelectedIndex]);
            }
        }

        private void UploadFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = true
            };
            if (myDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(myDialog.FileName);
                storageClient.UploadFile(fileInfo);
            }
            else
            {
                MessageBox.Show("Во время выбора файла возникли некоторые ошибки, попробуйте еще раз!");
            }
        }
        #endregion
    }
}
