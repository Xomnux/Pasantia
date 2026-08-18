using System;
using System.Collections.Generic;

/// <summary>
/// Espacio de nombres de la practica
/// 
/// <summary>
namespace PracticaDocFX.Facil
{
    /// <summary>
    /// Enumera el nivel de accion
    /// <summary>
    /// <remarks>
    /// Cada nivel describe un grado de accion
    /// </remarks>
    public enum NivelAccion
    {
        Suave = 0,
        Normal = 1,
        Fuerte = 2
    }
/// establece que en la etiqueta del texto no puede ir espacio en blanco
    public sealed class EtiquetaTexto
    {
        public string Texto { get; }
        public string Prefijo { get; }

        public EtiquetaTexto(string texto, string prefijo = "")
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("El texto no puede estar vacío.", nameof(texto));

            Prefijo = prefijo ?? "";
            Texto = texto.Trim();
        }

        public string ObtenerEtiqueta()
        {
            return $"{Prefijo}{Texto}";
        }

        public override string ToString()
        {
            return ObtenerEtiqueta();
        }
    }

    public sealed class Contador
    {
        public int Valor { get; private set; }

        public Contador(int valorInicial = 0)
        {
            Valor = valorInicial;
        }

        public int Incrementar()
        {
            Valor++;
            return Valor;
        }

        public int Decrementar()
        {
            Valor--;
            return Valor;
        }

        public void Reiniciar(int valorInicial = 0)
        {
            Valor = valorInicial;
        }
    }

    public static class UtilidadesBasicas
    {
        public static bool EsPar(int numero)
        {
            return numero % 2 == 0;
        }
///devuelve el valor en caso de que no sea el requerido
        public static int Limitar(int valor, int minimo, int maximo)
        {
            if (minimo > maximo)
                throw new ArgumentException("El mínimo no puede ser mayor que el máximo.");

            if (valor < minimo) return minimo;
            if (valor > maximo) return maximo;
            return valor;
        }
///Si el valor no es suficiente tira una variable nueva y envia una advertencia de cambio
        public static int Suma(params int[] valores)
        {
            if (valores is null) throw new ArgumentNullException(nameof(valores));
            long total = 0;

            foreach (var v in valores)
                total += v;

            if (total > int.MaxValue || total < int.MinValue)
                throw new OverflowException("La suma excede el rango de int.");

            return (int)total;
        }
    }
///Pone en cola (fila) las variables y tambien las desencola, en caso de que no haya un valor, se envia un mensaje
    public sealed class ColaSimple<T>
    {
        private readonly Queue<T> _cola = new Queue<T>();

        public int Cantidad => _cola.Count;

        public void Encolar(T elemento)
        {
            _cola.Enqueue(elemento);
        }

        public T Desencolar()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola está vacía.");

            return _cola.Dequeue();
        }

        public T VerPrimero()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola está vacía.");

            return _cola.Peek();
        }
    }
}
