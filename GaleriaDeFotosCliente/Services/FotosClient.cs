using GaleriaDeFotosCliente.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace GaleriaDeFotosCliente.Services
{
    public class FotosClient
    {
        TcpClient client = null!;
        public string Equipo { get; set; } = null!;


        public void Conectar(IPAddress ip)
        {
            try
            {
                IPEndPoint ipe = new(ip, 8001);
                client = new();
                client.Connect(ipe);
                Equipo = Dns.GetHostName();

            }
            catch
            {

            }
        }

        public void Desconectar()
        {
            client.Close();
        }

        public void RecibirIMG()
        {
            new Thread(() =>
            {
                try
                {
                    while (client.Connected)
                    {
                        var ns = client.GetStream();
                        byte[] buffer = new byte[client.Available];
                        ns.Read(buffer, 0, buffer.Length);

                        var img = JsonSerializer.Deserialize<ImageDTO>
                        (Encoding.UTF8.GetString(buffer));
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (img != null)
                            {
                                ImagenRecibida?.Invoke(this, img);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {

                }
            })
            { IsBackground = true }.Start();

        }

        public void EnviarIMG(ImageDTO img) 
        {
            if (!string.IsNullOrWhiteSpace(img.Img))
            {
                var json = JsonSerializer.Serialize(img);
                byte[] buffer = Encoding.UTF8.GetBytes(json);

                var ns = client.GetStream();
                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
            }
        }

        public event EventHandler<ImageDTO>? ImagenRecibida;
    }
}
