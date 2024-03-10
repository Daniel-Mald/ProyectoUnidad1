using GaleriaDeFotosServer.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GaleriaDeFotosServer.Services
{
    public class GaleriaServer
    {
        TcpListener _server = null!;
        List<TcpClient> _clientes = new List<TcpClient>();
        public event EventHandler<ImageDTO>? FotoRecibida;
        //public event EventHandler<TcpClient>? UsuaioConectado;
        
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

        //Este metodo le manda las imagenes que tiene al usuario
        void MandarImagenes(ImageDTO dto, TcpClient cliente)
        {
            string ruta = $"imagenes/{dto.NombreUser}";
            if (Directory.Exists(ruta))
            {
                List<ImageDTO> imagenesDeVuelta = new();
                var fotos =  Directory.GetFiles(ruta);
                foreach (var item in fotos)
                {
                    ImageDTO x = new()
                    {
                        NombreUser = dto.NombreUser,
                        Img = Convert.ToBase64String(File.ReadAllBytes(item))
                    };
                    imagenesDeVuelta.Add(x);
                }

                var json = JsonSerializer.Serialize(imagenesDeVuelta);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                var ns = cliente.GetStream();              
                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
            }
            
        }
        void Recibir(TcpClient c)
        {
            bool RecienConectadoBandera = true;
                     
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
                    if (RecienConectadoBandera)
                    {
                        MandarImagenes(dto, c);
                        RecienConectadoBandera = false;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FotoRecibida?.Invoke(this, dto);
                    });
                }             
            }
        }
    }

}
