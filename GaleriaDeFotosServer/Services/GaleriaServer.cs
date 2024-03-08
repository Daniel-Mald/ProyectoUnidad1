using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GaleriaDeFotosServer.Services
{
    public class GaleriaServer
    {
        TcpListener _server = null!;
        List<TcpClient> _clientes = new List<TcpClient>();
        public event EventHandler FotoRecibida;
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

        }
    }

}
