using System;
using System.Collections.Generic;

namespace PracticaDocFX.Facil
{
    /// <summary>
    /// Enumera el nivel de accion
    /// </summary>
    /// <remark>
    /// Cada nivel describe un grado de accion
    /// </remark>
    public enum NivelAccion
    {
        Suave = 0,
        Normal = 1,
        Fuerte = 2
    }
    ///<summary>
    /// Establece que en la etiqueta del texto no puede ir espacio en blanco
    ///</summary>
    ///<remarks>
    ///Cada texto no puede ser de valor nulo
    ///</remarks>
    public sealed class EtiquetaTexto
    {
        ///<summary>
        /// Obtiene el texto
        ///</summary>
        ///<remarks>
        /// Toma la disposicion del texto
        ///</remarks>
        public string Texto { get; }
        ///<summary>
        /// Obtiene el prefijo
        ///</summary>
        ///<remarks>
        /// Toma la disposicion del prefijo
        ///</remarks>
        public string Prefijo { get; }
        ///<summary>
        /// Pide el texto y el prefijo
        ///</summary>
        ///<remarks>
        /// Pide y define el lugar del texto y prefijo
        ///</remarks>
        /// <param name="prefijo">
        /// Corresponde al prefijo de la etiqueta
        /// </param>
        /// <param name="texto">
        /// Corresponde al texto de la etiqueta
        /// </param>
        /// <value>
        /// Representa la etiqueta del texto
        /// </value>
        public EtiquetaTexto(string texto, string prefijo = "")
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("El texto no puede estar vacío.", nameof(texto));

            Prefijo = prefijo ?? "";
            Texto = texto.Trim();
        }
        ///<summary>
        /// Obtiene las etiquetas
        ///</summary>
        ///<remarks>
        /// Obtiene las etiquetas devolviendo prefijo y texto
        ///</remarks>
        public string ObtenerEtiqueta()
        {
            return $"{Prefijo}{Texto}";
        }
        /// <summary>
        /// Sobreescribe
        /// </summary>
        /// <remarks>
        /// Sobreescribe el texto de cadena
        /// </remarks>
        public override string ToString()
        {
            return ObtenerEtiqueta();
        }
    }
    ///<summary>
    /// Aumenta o disminuye el valor inicial de la variable
    ///</summary>
    ///<remarks>
    /// Puede tomar y aumentar o disminuir un valor 
    ///</remarks>
    public sealed class Contador
    {
        /// <summary>
        /// Usa atributos como Public y Private
        /// </summary>
        /// <remark>
        /// Cambia los atributos de las variables
        /// </remark>
        public int Valor { get; private set; }

        /// <summary>
        /// Es para inicializar el constructor
        /// </summary>
        ///<remarks>
        /// Hace que el constructor arrance desde 0
        /// </remarks>
        /// <param name="valorInicial">
        /// Corresponde al valor inicial en el contador (0)
        /// </param>
        public Contador(int valorInicial = 0)
        {
            Valor = valorInicial;
        }

        /// <summary>
        /// Establece el atributo de la variables Incrementar
        /// </summary>
        /// <remarks>
        /// Hace que la funcion de la variable sea la de sumar al valor
        /// </remarks>
        public int Incrementar()
        {
            Valor++;
            return Valor;
        }

        /// <summary>
        /// Establece el atributo de la variable Decrementar
        /// </summary>
        /// <remarks>
        /// Hace que la funcion de la variable sea la de restar al valor
        /// </remarks>
        public int Decrementar()
        {
            Valor--;
            return Valor;
        }

        /// <summary>
        ///Hace que el valor se torne a 0 
        /// </summary>
        /// <remarks>
        /// Usa una variable de valor 0 para asi convertir a la variable de Valor en un 0
        /// </remarks>
        /// <param name="valorInicial">
        /// Corresponde al valor inicial (0) en caso de que se reinicie el contador
        /// </param>
        public void Reiniciar(int valorInicial = 0)
        {
            Valor = valorInicial;
        }
    }
    ///<summary>
    ///Devuelve valores no deseados
    ///</summary>
    ///<remarks>
    ///Devuelve el valor en caso de que no sea par y Si el valor no es suficiente tira una variable nueva y envia una advertencia de cambio
    ///</remarks>
    public static class UtilidadesBasicas
    {
        /// <summary>
        /// Devuelve el numero a ser par
        /// </summary>
        /// <remarks>
        /// Bota el residuo de la division
        /// </remarks>
        /// <param name="numero">
        /// Equivale al numero residuo de la division
        /// </param>
        public static bool EsPar(int numero)
        {
            return numero % 2 == 0;
        }
        /// <summary>
        /// Se asegura de que el valor maximo sea mayor al valor minimo
        /// </summary>
        /// <remarks>
        /// Usa condicionales y en caso de que no se cumplan enviara un mensaje de advertencia
        /// </remarks>
        /// <param name="maximo">
        /// Equivale al rango maximo
        /// </param>
        /// <param name="minimo">
        /// Equivale al rango minimo
        /// </param>
        /// <param name="valor">
        /// Equivale al valor del numero 
        /// </param>
        public static int Limitar(int valor, int minimo, int maximo)
        {
            if (minimo > maximo)
                throw new ArgumentException("El mínimo no puede ser mayor que el máximo.");

            if (valor < minimo) return minimo;
            if (valor > maximo) return maximo;
            return valor;
        }
        /// <summary>
        /// Realiza una suma de un arreglo de valores
        /// </summary>
        /// <remarks>
        /// Usa la funcion de calculo de suma para sumar enteros
        /// </remarks>
        /// <param name="valores">
        /// Equivale a un conjunto de enteros
        /// </param>
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
    ///<summary>
    ///Enfila y desenfila los valores
    ///</summary>
    ///<remarks>
    ///Pone en cola (fila) las variables y tambien las desencola, en caso de que no haya un valor, se envia un mensaje
    ///</remarks>
    public sealed class ColaSimple<T>
    {
        /// <summary>
        /// Crea una cola de numeros no sobreescribibles
        /// </summary>
        /// <remarks>
        /// Usa comando de configuracion de colas
        /// </remarks>
        /// <value>
        /// Representa la fila de cantidades de numeros
        /// </value>
        private readonly Queue<T> _cola = new Queue<T>();

        /// <summary>
        /// Envia la cantidad de elementos a la cola
        /// </summary>
        /// <remarks>
        /// Usa la variable de cantidad para enviarla a la cola de elementos
        /// </remarks>
        public int Cantidad => _cola.Count;

        /// <summary>
        /// Encola las variables de numero entero
        /// </summary>
        /// <remarks>
        /// Usa el comando de encolar para manipular los numeros enteros
        /// </remarks>
        /// <param name="elemento">
        /// Corresponde al elemento que va en la cola
        /// </param>
        public void Encolar(T elemento)
        {
            _cola.Enqueue(elemento);
        }
        ///<summary>
        /// Desencola los tipos de dato
        /// </summary>
        /// <remarks>
        /// Desencola con desencolar
        /// </remarks>
        public T Desencolar()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola está vacía.");

            return _cola.Dequeue();
        }
        /// <summary>
        /// Envia un mensaje si la cola esta vacia y si tiene regresa el primero
        /// </summary>
        /// <remarks>
        /// Usa comandos de excepcion y retorno
        /// </remarks>
        public T VerPrimero()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola está vacía.");

            return _cola.Peek();
        }
    }
}
