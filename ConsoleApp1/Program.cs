using BookingOfGoods;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Channels;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            
            string[] peoples = 
            {
                "Анна Петрова",
                "Максим Сидоров",
                "Екатерина Ивановская",
                "Дмитрий Кузнецов",
                "Софья Романова",
                
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, 5, (i) =>
            {
                using (ShopStorage shopStorage = new ShopStorage())
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (shopStorage.Reserve("IPhone", peoples[i], rnd.Next(1, 4)))
                        {
                            Console.WriteLine($"Успешно забронировался товар от потока: {i} | {peoples[i]})");
                        }
                        else
                        {
                            Console.WriteLine($"Товар нет для потока: {i} | {peoples[i]})");
                        }

                        Thread.Sleep(50);
                    }
                }
            });
            
            sw.Stop();
            Console.WriteLine($"Успешно всё, время: {sw.Elapsed}");
            Console.ReadKey();
        }
    }
    
}
