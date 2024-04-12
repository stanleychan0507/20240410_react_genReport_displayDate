// See https://aka.ms/new-console-template for more information
using ConsoleApp1;
using System.Drawing;

Console.WriteLine("Hello, World!");


Class1 class1 = new Class1 { A = 1, B = 1 };
Class1 class2 = new Class1 { A = 1, B = 1 };
Class1 class3 = new Class1 { A = 1, B = 1 };

Class1 class4 = class1 + class2 + class3;

Console.WriteLine($"{ class4.A}, { class4.B}");
Console.ReadLine();

