using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Browser;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Linq;

namespace ImageUploader
{
    public class WindowsAzureBlobUploader 
    {
        private UserFile _file;
        private long _dataLength;
        private long _dataSent;
        private string _initParams;

        private long ChunkSize = 4194304;
        private string UploadUrl;
        private bool UseBlocks;

        private string currentBlockId;
        private List<string> blockIds = new List<string>();

        public WindowsAzureBlobUploader(UserFile file, string uploadContainerUrl)
        {
            _file = file;

            _dataLength = _file.FileStream.Length;
            _dataSent = 0;

            // upload the blob in smaller blocks if it's a "big" file
            if (_dataLength > ChunkSize)
            {
                UseBlocks = true;
            }
            else
            {
                UseBlocks = false;
            }

            // uploadContainerUrl has a Shared Access Signature already
            var uriBuilder = new UriBuilder(uploadContainerUrl);
            uriBuilder.Path += string.Format("/{0}", _file.FileName);
            UploadUrl = uriBuilder.Uri.AbsoluteUri;

            var sasBlobUri = uriBuilder.Uri;
        }

        public void StartUpload(string initParams)
        {
            _initParams = initParams;

            StartUpload();
        }

        public void CancelUpload()
        {
            //Not implemented yet...
        }

        public event EventHandler UploadFinished;

        private void StartUpload()
        {
            long dataToSend = _dataLength - _dataSent;

            var uriBuilder = new UriBuilder(UploadUrl);

            if (UseBlocks)
            {
                // encode the block name and add it to the query string
                currentBlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                uriBuilder.Query = uriBuilder.Query.TrimStart('?') +
                    string.Format("&comp=block&blockid={0}", currentBlockId);
            }

            // with or without using blocks, we'll make a PUT request with the data
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            webRequest.Method = "PUT";
            webRequest.BeginGetRequestStream(new AsyncCallback(WriteToStreamCallback), webRequest);
        }

        // write up to ChunkSize of data to the web request
        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult);
            byte[] buffer = new Byte[4096];
            int bytesRead = 0;
            int tempTotal = 0;

            _file.FileStream.Position = _dataSent;

            //&& !_file.IsDeleted

            while ((bytesRead = _file.FileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < ChunkSize && _file.State != Constants.FileStates.Error)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                _dataSent += bytesRead;
                tempTotal += bytesRead;

                //_file.UIDispatcher.BeginInvoke(delegate()
                //{
                //    OnProgressChanged();
                //});
            }

            requestStream.Close();

            webRequest.BeginGetResponse(new AsyncCallback(ReadHttpResponseCallback), webRequest);
        }

        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            bool error = false;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());

                string responsestring = reader.ReadToEnd();
                reader.Close();
            }
            catch
            {
                error = true;

                //_file.UIDispatcher.BeginInvoke(delegate()
                //{
                //    _file.State = Constants.FileStates.Error;
                //});
            }

            if (!error)
            {
                blockIds.Add(currentBlockId);
            }

            // if there's more data, send another request
            if (_dataSent < _dataLength)
            {
                if (_file.State != Constants.FileStates.Error && !error)
                {
                    StartUpload();
                }
            }
            else // all done
            {
                _file.FileStream.Close();
                _file.FileStream.Dispose();

                if (UseBlocks)
                {
                    PutBlockList(); // commit the blocks into the blob
                }
                else
                {
                    //_file.UIDispatcher.BeginInvoke(delegate()
                    //{
                        if (UploadFinished != null)
                        {
                            UploadFinished(this, null);
                        }
                    //});
                }
            }
        }

        private void PutBlockList()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(
                new Uri(string.Format("{0}&comp=blocklist", UploadUrl)));
            webRequest.Method = "PUT";
            webRequest.Headers["x-ms-version"] = "2009-09-19"; // x-ms-version is required for put block list!
            webRequest.BeginGetRequestStream(new AsyncCallback(BlockListWriteToStreamCallback), webRequest);
        }

        private void BlockListWriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult);

            var document = new XDocument(
                new XElement("BlockList",
                    from blockId in blockIds
                    select new XElement("Uncommitted", blockId)));
            var writer = XmlWriter.Create(requestStream, new XmlWriterSettings() { Encoding = Encoding.UTF8 });
            document.Save(writer);
            writer.Flush();

            requestStream.Close();

            webRequest.BeginGetResponse(new AsyncCallback(BlockListReadHttpResponseCallback), webRequest);
        }

        private void BlockListReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            bool error = false;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());

                string responsestring = reader.ReadToEnd();
                reader.Close();
            }
            catch
            {
                error = true;

                /*_file.UIDispatcher.BeginInvoke(delegate()
                {
                    _file.State = Constants.FileStates.Error;
                });*/
            }

            if (!error)
            {
                /*_file.UIDispatcher.BeginInvoke(delegate()
                {
                    if (UploadFinished != null)
                    {
                        UploadFinished(this, null);
                    }
                });*/
            }
        }

        private void OnProgressChanged()
        {
            _file.BytesUploaded = _dataSent;
        }
    }
}
