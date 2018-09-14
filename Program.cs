using System;
using CoreEscuela.Entidades;

namespace Etapa1
{
    class Program
    {
        static void Main(string[] args)
        {
            var escuela = new Escuela("Platzi Academy", 2012, TiposEscuela.Primaria,
                        ciudad:"Bogotá" , pais:"Colombia"
                        );
            Console.WriteLine(escuela);
        }
    }
}
