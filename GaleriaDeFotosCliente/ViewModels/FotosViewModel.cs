using CommunityToolkit.Mvvm.Input;
using GaleriaDeFotosCliente.Models.DTOs;
using GaleriaDeFotosCliente.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GaleriaDeFotosCliente.ViewModels
{
    public class FotosViewModel : INotifyPropertyChanged
    {
        FotosClient cliente = new();
        public string Imagen {  get; set; } //Ruta de la imagen de la pc, ejemplo: C:\Users\Alka\Pictures\1.jpg
        public string IP { get; set; } = ""; 
        public bool Conectado { get; set; }

        public ICommand ConectarCommand { get; set; }
        public ICommand DesconectarCommand { get; set; }

        public ICommand EnviarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }

        
        public ObservableCollection<ImageDTO> ListaImgs { get; set; } = new();

        public FotosViewModel()
        {
            //cliente.ImagenRecibida += Cliente_ImagenRecibida;
            EnviarCommand = new RelayCommand(Enviar);
            ConectarCommand = new RelayCommand(Conectar);
            DesconectarCommand = new RelayCommand(Desconectar);
            EliminarCommand = new RelayCommand<string>(Eliminar); //Enviar como parametro el string "img" de ImageDTO
            cliente.ImagenRecibida += Cliente_ImagenRecibida1;
        }

        private void Cliente_ImagenRecibida1(object? sender, ImageDTO e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ListaImgs.Add(e);
            });
        }

        private void Desconectar()
        {
            cliente.Desconectar();
        }

        private void Eliminar(string? obj)
        {
            
        }

        private void Conectar()
        {
            IPAddress.TryParse(IP, out IPAddress? ipAddress);
            if(ipAddress != null)
            {
                cliente.Conectar(ipAddress);
                Conectado = true;
                PropertyChanged?.Invoke(this, new(nameof(Conectado)));
            }
        }

        private void Enviar()
        {
            if (!string.IsNullOrWhiteSpace(Imagen))
            {
                byte[] bytesImg = File.ReadAllBytes(Imagen);

                ImageDTO img = new()
                {
                    Estado = true,
                    NombreUser = cliente.Equipo,
                    Img = Convert.ToBase64String(bytesImg)
                };
                //ListaImgs.Add(img);
                cliente.EnviarIMG(img);
            }
        }


        //Lo hice pensando que tmb tendria un carrusel el cliente

        


        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
