using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Browser;

namespace ImageUploader
{
    [ScriptableType]
    public partial class MainPage : UserControl
    {
        private UserFile _file;
        private WriteableBitmap wb;
        string _uploadContainerUrl;

        void _file_Finished(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                VisualStateManager.GoToState(this, "Finished", true);
                //_uploadContainerUrl + _file.FileName

                string path = _uploadContainerUrl.Substring(0, _uploadContainerUrl.IndexOf("?"));
                string fileUrl = path + "/" + _file.FileName;

                HtmlPage.Window.Invoke("UploadFinished", fileUrl);

                // test it by showing the image
                //myImage.Source = wb;
            });
        }
        
        public MainPage(IDictionary<string, string> initParams)
        {           
            InitializeComponent();

            if (initParams.ContainsKey("UploadContainerUrl") && !string.IsNullOrEmpty(initParams["UploadContainerUrl"]))
            {
                _uploadContainerUrl = initParams["UploadContainerUrl"];
            }

            _file = new UserFile();
            _file.FileFinished += _file_Finished;

            HtmlPage.RegisterScriptableObject("File", _file);
            HtmlPage.RegisterScriptableObject("MainPage", this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "JPEG / PNG (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (openDialog.ShowDialog().GetValueOrDefault(false))
            {
                using (FileStream stream = openDialog.File.OpenRead())
                {
                    wb = ImageHelper.GetImageSource(stream, 700, 700);
                }

                //byte[] buffer;
                //using (
                    Stream Source = JpgEncoder.Encode(wb, 85); //)
                {
                    //int bufferSize = Convert.ToInt32(Source.Length);
                    //buffer = new byte[bufferSize];
                    //Source.Read(buffer, 0, bufferSize);

                    //Create a new UserFile object
                    
                    _file.FileName = openDialog.File.Name;
                    _file.FileStream = Source;
                    //userFile.UIDispatcher = this.Dispatcher;
                    _file.UploadContainerUrl = _uploadContainerUrl;

                    _file.Upload(null);

                    //Source.Close();
                }              
            }
        }
    }
}
