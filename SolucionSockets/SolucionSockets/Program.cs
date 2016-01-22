using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SolucionSockets
{
    class Program
    {
        static void Main(string[] args)
        {


            int puerto;
            Console.WriteLine("Hola.Indica el puerto en el que te quieres conectar");
            puerto = VerificarPuerto(Console.ReadLine());
            while (puerto < 0 || puerto > 65535)
            {
                Console.WriteLine("Puerto no valido.Torna a probar.");
                puerto = VerificarPuerto(Console.ReadLine());
            }
            Task.Factory.StartNew(() => { Servidor server = new Servidor(5555); });
            Thread.Sleep(500);
            Cliente cl = new Cliente("localhost", 5555);
            Console.ReadKey();
        }
        public static int VerificarPuerto(String text)
        {
            int port = -1;
            try
            {
                port = Convert.ToInt32(text);
                return port;

            }
            catch
            {
                return port;
            }

        }
    }
    class Cliente
    {
        public Cliente(string servidor, int puerto)
        {
            try
            {
                TcpClient cliente = new TcpClient(servidor, puerto);
                NetworkStream n = cliente.GetStream();
                BinaryReader r = new BinaryReader(n);
                BinaryWriter w = new BinaryWriter(n);
                string texto = null;
                int jugada;
                Thread.Sleep(500);
                do
                {
                    Console.WriteLine("1.Piedra");
                    Console.WriteLine("2.Papel");
                    Console.WriteLine("3.Tijeras");
                    jugada = ConvertirFormato(Console.ReadLine());
                    while (jugada < 1 || jugada > 3)
                    {
                        Console.WriteLine("Opción no valida, intentalo de nuevo.");
                        jugada = ConvertirFormato(Console.ReadLine());
                    }

                    if (jugada == 1)
                    {
                        Console.WriteLine("Jugador saca: Piedra");
                    }
                    if (jugada == 2)
                    {
                        Console.WriteLine("Jugador saca: Papel");
                    }
                    if (jugada == 3)
                    {
                        Console.WriteLine("Jugador saca: Tijeras");
                    }
                    texto = Convert.ToString(jugada);
                    w.Write(texto);
                    w.Flush();
                    Console.WriteLine(r.ReadString());
                } while (texto != ":End");
                Console.WriteLine(r.ReadString());
                Console.WriteLine("Desconectado.");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error de connexión");
            }

        }
        public static int ConvertirFormato(String texto)
        {
            //Convertir el formato de string a integer
            int jugada = -1;
            try
            {
                jugada = Convert.ToInt32(texto);
                return jugada;

            }
            catch
            {
                return jugada;
            }

        }
        
    }
    }
    class Servidor
    {
public Servidor(int puerto)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, puerto);
                listener.Start();
                Console.WriteLine("Servidor iniciado");
                TcpClient connexioClient = listener.AcceptTcpClient();
                Console.WriteLine("Cliente conectado");
                NetworkStream netStream = connexioClient.GetStream();
                BinaryReader reader = new BinaryReader(netStream);
                BinaryWriter writer = new BinaryWriter(netStream);
                string mensaje = null;
                int jugada1, jugada2, ganador;
                do
                {
                    mensaje = reader.ReadString();
                    jugada1 = Convert.ToInt32(mensaje);
                    jugada2 = TiradaServidor();

                    if (jugada2 == 1)
                    {
                        mensaje = "Piedra";
                    }
                    if (jugada2 == 2)
                    {
                        mensaje = "Papel";
                    }
                    if (jugada2 == 3)
                    {
                        mensaje = "Tijeras";
                    }
         
                    Console.WriteLine("La maquina saca: " + mensaje);
                    ganador = ComprobarGanador(jugada1, jugada2);

                    if (ganador == 0)
                    {
                        mensaje = "Empate!";
                    }
                    if (ganador == 1)
                    {
                        mensaje = "Gana el jugador!";
                    }
                    if (ganador == 2)
                    {
                        mensaje = "Gana la maquina!";
                    }
                   
                    writer.Write("El resultado és: " + mensaje);
                    writer.Flush();
                } while (true);
               writer.Write("Cliente desconectado");
                connexioClient.Close();
                listener.Stop();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error de connexión " + e);
            }
        }
        public int TiradaServidor()
        {
            Random rdm = new Random();
            int opcio = rdm.Next(1, 4);
            return opcio;
        }
        public int ComprobarGanador(int jugador, int servidor)
        {
            int ganador=0;
            switch (jugador)
            {
                case 1:
                    switch (servidor)
                    {
                        case 1:
                            ganador = 0;
                            break;
                        case 2:
                            ganador = 2;
                            break;
                        case 3:
                            ganador = 1;
                            break;
                    }
                    break;
                case 2:
                    switch (servidor)
                    {
                        case 1:
                            ganador = 1;
                            break;
                        case 2:
                            ganador = 0;
                            break;
                        case 3:
                            ganador = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (servidor)
                    {
                        case 1:
                            ganador = 2;
                            break;
                        case 2:
                            ganador = 1;
                            break;
                        case 3:
                            ganador = 0;
                            break;
                    }
                    break;
            }
            return ganador;
        }
    }
   