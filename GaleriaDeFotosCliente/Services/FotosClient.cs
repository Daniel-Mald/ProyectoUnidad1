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
                RecibirIMG();
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
                        while (client.Available == 0)
                        {
                            Thread.Sleep(500);
                        }
                        
                           
                            
                        var ns = client.GetStream();
                        string json = "";


                                while (client.Available > 0)
                                {
                                    byte[] buffer = new byte[client.Available];
                                    ns.Read(buffer, 0, buffer.Length);
                                    json+= Encoding.UTF8.GetString(buffer);

                                }
                            
                            string[] n = json.Split('}');
                            foreach (string item in n)
                            {
                               string newJson = item + "}";
                                var img = JsonSerializer.Deserialize<ImageDTO>(newJson);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    if (img != null)
                                    {
                                        ImagenRecibida?.Invoke(this, img);
                                    }
                                });
                            }

                            
                            
                        
                       

                        
                        

                        //while (client.Available > 0)
                        //{
                        //    byte[] buffer = new byte[client.Available];
                        //    int bytesRead = ns.Read(buffer, 0, buffer.Length);
                        //    json.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        //}



                        //var img = JsonSerializer.Deserialize<ImageDTO>(json);

                       








                    }
                }
                catch (Exception ex)
                {

                }
                //finally
                //{
                //    // Cerrar la conexión y liberar recursos
                //    client.Close();
                //}
            })
            { IsBackground = true }.Start();

        }

        public void EnviarIMG(ImageDTO img)
        {
            if (!string.IsNullOrWhiteSpace(img.Img) || img.Estado == false)
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
