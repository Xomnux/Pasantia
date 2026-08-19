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
    /// establece que en la etiqueta del texto no puede ir espacio en blanco
    ///</summary>
    ///<remarks>
    ///Cada texto no puede ser de valor nulo
    ///</remarks>
    public sealed class EtiquetaTexto
    {
    ///<summary>
    /// Obtiene el texto y el prefijo
    ///</summary>
    ///<remarks>
    /// toma la disposicion del texto y el prefijo
    ///</remarks>
        public string Texto { get; }
        public string Prefijo { get; }
    ///<summary>
    /// Pide el texto y el prefijo
    ///</summary>
    ///<remarks>
    /// pide y define el lugar del texto y prefijo
    ///</remarks>
        public EtiquetaTexto(string texto, string prefijo = "")
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("El texto no puede estar vacío.", nameof(texto));

            Prefijo = prefijo ?? "";
            Texto = texto.Trim();
        }
    ///<summary>
    /// obtiene las etiquetas
    ///</summary>
    ///<remarks>
    /// Obtiene las etiquetas devolviendo prefijo y texto
    ///</remarks>
        public string ObtenerEtiqueta()
        {
            return $"{Prefijo}{Texto}";
        }
    /// <summary>
    /// sobreescribe
    /// </summary>
    /// <remarks>
    /// sobreescribe el texto de cadena
    /// </remarks>
        public override string ToString()
        {
            return ObtenerEtiqueta();
        }
    }
    ///<summary>
    /// aumenta o disminuye el valor inicial de la variable
    ///</summary>
    ///<remarks>
    /// puede tomar y aumentar o disminuir un valor 
    ///</remarks>
    public sealed class Contador
    {
        /// <summary>
        /// Usa atributos como Public y Private
        /// </summary>
        /// <remark>
        /// cambia los atributos de las variables
        /// </remark>
        public int Valor { get; private set; }

        /// <summary>
        /// es para inicializar el constructor
        /// </summary>
        ///<remarks>
        /// hace que el constructor arrance desde 0
        /// </remarks>
        public Contador(int valorInicial = 0)
        {
            Valor = valorInicial;
        }

        /// <summary>
        /// establece el atributo de la variables Incrementar
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
        public void Reiniciar(int valorInicial = 0)
        {
            Valor = valorInicial;
        }
    }
    ///<summary>
    ///Devuelve valores no deseados
    ///</summary>
    ///<remarks>
    ///devuelve el valor en caso de que no sea par y Si el valor no es suficiente tira una variable nueva y envia una advertencia de cambio
    ///</remarks>
    public static class UtilidadesBasicas
    {
        /// <summary>
        /// devuelve el numero a ser par
        /// </summary>
        /// <remarks>
        /// bota el residuo de la division
        /// </remarks>
        public static bool EsPar(int numero)
        {
            return numero % 2 == 0;
        }
        /// <summary>
        /// se asegura de que el valor maximo sea mayor al valor minimo
        /// </summary>
        /// <remarks>
        /// usa condicionales y en caso de que no se cumplan enviara un mensaje de advertencia
        /// </remarks>
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
    ///enfila y desenfila los valores
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
        /// usa comando de configuracion de colas
        /// </remarks>
        private readonly Queue<T> _cola = new Queue<T>();

        /// <summary>
        /// envia la cantidad del numero a la cola
        /// </summary>
        /// <remarks>
        /// usa la variable de cantidad para enviarla a la cola de valores
        /// </remarks>
        public int Cantidad => _cola.Count;

        /// <summary>
        /// encola las variables de numero entero
        /// </summary>
        /// <remarks>
        /// usa el comando de encolar para manipular los numeros enteros
        /// </remarks>
        public void Encolar(T elemento)
        {
            _cola.Enqueue(elemento);
        }
        ///<summary>
        /// desencola los numeros enteros
        /// </summary>
        /// <remarks>
        /// desencola con desencolar 
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
        /// usa comandos de excepcion y retorno
        /// </remarks>
        public T VerPrimero()
        {
            if (_cola.Count == 0)
                throw new InvalidOperationException("La cola está vacía.");

            return _cola.Peek();
        }
    }
}
