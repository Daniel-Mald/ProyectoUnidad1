using GaleriaDeFotosServer.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GaleriaDeFotosServer.Services
{
    public class GaleriaServer
    {
        TcpListener _server = null!;
        List<TcpClient> _clientes = new List<TcpClient>();
        public event EventHandler<ImageDTO> FotoRecibida;
        public void IniciarServer()
        {
            _server = new(new IPEndPoint(IPAddress.Any, 30000));
            _server.Start();
            new Thread(Escuchar)
            {
                IsBackground = true
            }.Start();
        }
        public void CerrarServer()
        {
            if(_server != null && _server.Server.IsBound)
            {
                _server.Stop();
                foreach (var item in _clientes)
                {
                    item.Close();
                }
            }
        }
        void Escuchar()
        {
            while (_server.Server.IsBound)
            {
                var tcpClient = _server.AcceptTcpClient();
                _clientes.Add(tcpClient);

                Thread t = new(() =>
                {
                    Recibir(tcpClient);
                });
                t.IsBackground = true;
                t.Start();


                
            }
        }
        void Recibir(TcpClient c)
        {
            while (c.Connected)
            {
                var networkStream = c.GetStream();
                while(c.Available == 0)
                {
                    Thread.Sleep(1000);
                }
                byte[] buffer = new byte[c.Available];
                networkStream.Read(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer);


                var dto = JsonSerializer.Deserialize<ImageDTO>(json);
                if(dto != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FotoRecibida?.Invoke(this, dto);
                    });
                }

                //string base64text = System.Text.Encoding.ASCII.GetString(buffer,0,buffer.Length);
                 
                //var base64Image = System.Convert.FromBase64String(base64text);
                //IPEndPoint clientEndPoint = (IPEndPoint)
                //string imageName = $"Imagenes/{c.}";
                //System.IO.File.WriteAllBytes()
                //evento
            }
        }
    }

}
