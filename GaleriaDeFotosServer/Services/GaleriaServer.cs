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
            _server = new(new IPEndPoint(IPAddress.Any, 8001));
            _server.Start();
            new Thread(Escuchar)
            {
                IsBackground = true
            }.Start();
        }
        public void CerrarServer()
        {
            try
            {
                if (_server != null && _server.Server.IsBound)
                {
                    _server.Stop();
                    foreach (var item in _clientes)
                    {
                        item.Close();
                    }
                }
            }
            catch (Exception)
            {

                throw;
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
        async Task MandarImagenes(string nombre, TcpClient cliente)
        {
            try
            {
                string ruta = $"../imagenes/{nombre}";
                if (Directory.Exists(ruta))
                {
                    
                        var fotos = Directory.GetFiles(ruta);
                        foreach (var item in fotos) //este es el foreach que le regresa las imagenes al cliente, pero no entiendo porque aveces no da todas las iteraciones
                        {
                        ImageDTO x = new()
                        {
                            NombreUser = nombre,
                            Img = Convert.ToBase64String(File.ReadAllBytes(item)),
                            ImagenId = int.Parse(Path.GetFileNameWithoutExtension(item)),
                            };
                            
                            //imagenesDeVuelta.Add(x);
                            var json = JsonSerializer.Serialize(x);
                            byte[] buffer = Encoding.UTF8.GetBytes(json);
                            var ns = cliente.GetStream();
                           await ns.WriteAsync(buffer, 0, buffer.Length);
                            ns.Flush();
                            

                        }
                   


                }
            }
            catch (Exception)
            {

                throw;
            }
            
            
        }
        
        void Recibir(TcpClient c)
        {
            
                     
            while (c.Connected)
            {

                var ns = c.GetStream();
                string json = "";
                while(c.Available == 0)//este pedazo lo hice porque si una imagen es demasiado grande me daba error
                {
                    Thread.Sleep(500);
                }
                while (c.Available > 0)
                {
                    byte[] buffer = new byte[c.Available];
                    ns.Read(buffer, 0, buffer.Length );
                    json +=  Encoding.UTF8.GetString(buffer);               
                }
                var dto = JsonSerializer.Deserialize<ImageDTO>(json);


                if(dto != null && dto.Estado == false)
                {
                    File.Delete($"../Imagenes/{dto.NombreUser}/{dto.ImagenId}.jpg");
                }
               

                    
                
                if (dto != null && c.Available == 0)
                {
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FotoRecibida?.Invoke(this, dto);
                    });
                }

                //aqui manda las imagenes de regreso al cliente
                MandarImagenes(dto.NombreUser, c);

            }
        }
    }

}
