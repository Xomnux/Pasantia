using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticaDocFX.Intermedio
{
    /// <summary>
    /// Su funcion es mostrar el estado del pedido del cliente
    /// </summary>
    /// <remarks>
    /// usa elementos como Public para que sea visible para todos y Enum para crear la lista de funciones (Creado, pagado y cancelado) 
    /// </remarks>
    public enum EstadoPedido
    {
        Creado = 0,
        Pagado = 1,
        Cancelado = 2
    }

///convierte espacios como el codigo, mensaje y campo en algo visible y obliga a llenar los espacios
 
     /// <summary>
    /// funciona como un sellador de clases
    /// </summary>
    /// <remarks>
    /// la palabra Sealed (Sellado) sella la clase y evita que se pueda volver a usar 
    /// </remarks>
    public sealed class ErrorValidacion
    {
        /// <summary>
        /// Estas 3 lineas registran el codigo, mensaje y campo
        /// </summary>
        /// <remarks>
        /// Al resgistrar las cadenas, la funcion { get; } guarda la informacion para que no sea modificada
        /// </remarks>
        public string Codigo { get; }
        public string Mensaje { get; }
        public string? Campo { get; }

        /// <summary>
        /// Evita que hayan espacios en blanco al recibir los enteros o las cadenas obligatorias
        /// </summary>
        /// <remarks>
        /// Esta funcion hace que encaso de que haya informacion nula o incorrecta, el sistema envie un codigo de error para que sea corregido
        /// Trim elimina espacios en blanco accidentales y string? hace que el parametro pueda estar lleno o nulo
        /// </remarks>
        public ErrorValidacion(string codigo, string mensaje, string? campo = null)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje es obligatorio.", nameof(mensaje));

            Codigo = codigo.Trim();
            Mensaje = mensaje.Trim();
            Campo = campo?.Trim();
        }
    }
        ///<summary>
        /// sella la clase
        /// </summary>
        /// <remarks>
        /// Sealed sella la clase ResultadoValidacion
        /// </remarks>
    public sealed class ResultadoValidacion
    {
        /// <summary>
        /// crea una lista no sobreescribible
        /// </summary>
        /// <remarks>
        /// usa el ReadOnly y get; para crear texto solamente visible pero no editable.
        /// </remarks>
        public IReadOnlyList<ErrorValidacion> Errores { get; }
        /// <summary>
        /// revisa si hay errores
        /// </summary>
        /// <remarks>
        /// con la variable booleana Valido se determina si hay errores por medio del true or false
        /// </remarks>
        public bool Valido => Errores.Count == 0;
        /// <summary>
        /// usa una lista de errores ya hecha
        /// </summary>
        /// <remarks>
        /// envia lo que se considereun error a la lista de errores ya establecida
        /// </remarks>
        private ResultadoValidacion(List<ErrorValidacion> errores)
        {
            Errores = errores;
        }
        /// <summary>
        /// crea una nueva lista de validacion
        /// </summary>
        /// <remarks>
        /// crea una lista  para guardar datos de forma privada
        /// </remarks>
        public static ResultadoValidacion Correcto()
        {
            return new ResultadoValidacion(new List<ErrorValidacion>());
        }
        /// <summary>
        /// crea una lista de errores
        /// </summary>
        /// <remarks>
        /// identifica y enlista los errores cometidos por el usuario
        /// </remarks>
        public static ResultadoValidacion ConErrores(IEnumerable<ErrorValidacion> errores)
        {
            if (errores is null) throw new ArgumentNullException(nameof(errores));
            return new ResultadoValidacion(errores.ToList());
        }
    }
    /// <summary>
    /// permite tener validaciones para productos, usuarios, etc
    /// </summary>
    /// <remarks>
    /// es una interfaz con reglas
    /// </remarks>
    public interface IValidador<in T>
    {
        ResultadoValidacion Validar(T valor);
    }
///Evita que el espacio del campo este vacio

    /// <summary>
    /// evita que no hayan datos con valores nulos
    /// </summary>
    /// <remarks>
    /// se encarga de que los valores de los datos no tengan espacios o sean nulos
    /// </remarks>
    public sealed class ValidadorNoNulo<T> : IValidador<T>
    {
        /// <summary>
        /// Es la variabilidad del nombre del valor durante su validacion
        /// </summary>
        /// <remarks>
        /// privatiza el ReadOnly y valida nombres de valores
        /// </remarks>
        private readonly string _nombreCampo;

        /// <summary>
        /// evita que el campo del nombre del valor este en blanco
        /// </summary>
        /// <remarks>
        /// usa excepciones, nombres en cadena y nameof para evitar errores de compilacion.
        /// </remarks>
        public ValidadorNoNulo(string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(nombreCampo))
                throw new ArgumentException("El nombre del campo es obligatorio.", nameof(nombreCampo));

            _nombreCampo = nombreCampo.Trim();
        }
        /// <summary>
        /// Devuelve un mensaje de correccion si el valor es nulo
        /// </summary>
        /// <remarks>
        /// En caso de que el valor recibido sea nulo, se envia un mensaje para que el nombre sea corregido y llenado
        /// </remarks>
        public ResultadoValidacion Validar(T valor)
        {
            if (valor is null)
            {
                return ResultadoValidacion.ConErrores(new[]
                {
                    new ErrorValidacion("NULO", $"{_nombreCampo} no puede ser nulo.", _nombreCampo)
                });
            }

            return ResultadoValidacion.Correcto();
        }
    }
    /// <summary>
    /// sella una clase
    /// </summary>
    /// <remarks>
    /// sella la clase ValidadorRango
    /// </remarks>
    public sealed class ValidadorRango
    {
        /// <summary>
        /// Permite al valor tener un maximo y minimo incluso en decimal
        /// </summary>
        /// <remarks>
        /// Usa el comando double para hacer uso de decimales en las variables Minimo y Maximo 
        /// </remarks>
        public double Minimo { get; }
        public double Maximo { get; }

        /// <summary>
        /// establece un  limite minimo para el valor de la variable Minimo
        /// </summary>
        /// <remarks>
        /// Evita que el valor Minimo sea mayor al maximo con excepciones y condicionales
        /// </remarks>
        public ValidadorRango(double minimo, double maximo)
        {
            if (minimo > maximo) throw new ArgumentOutOfRangeException(nameof(minimo), "El mínimo debe ser <= máximo.");
            Minimo = minimo;
            Maximo = maximo;
        }
        /// <summary>
        /// Evita que la cadena de campo sea un espacio en blanco
        /// evita que el valor Minimo y Maximo se salgan de su rango
        /// </summary>
        /// <remarks>
        /// Usa una condicional y una excepcion para evitar espacios en blanco
        /// En casode que los valores Minimo y Maximo excendan su limite, devolvera el valor y enviara un mensaje de correccion.
        /// </remarks>
        public ResultadoValidacion Validar(double valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(campo))
                throw new ArgumentException("El campo es obligatorio.", nameof(campo));

            if (valor < Minimo || valor > Maximo)
            {
                return ResultadoValidacion.ConErrores(new[]
                {
                    new ErrorValidacion("FUERA_DE_RANGO", $"{campo} debe estar entre {Minimo} y {Maximo}.", campo)
                });
            }

            return ResultadoValidacion.Correcto();
        }
    }
    /// <summary>
    /// sella la clase CompositeValidador y la hace generica 
    /// </summary>
    /// <remarks>
    /// Hace que la clase no sea heredable y hace que sea generica usando el caracter T
    /// </remarks>
    public sealed class CompositeValidador<T> : IValidador<T>
    {
        /// <summary>
        /// Privatiza la lista de IValidador y la hace generica, tambien crea un nuevo constructor
        /// </summary>
        /// <remarks>
        /// Hace que la lista IValidador sea solamene leible y no editable, tambien creo un constructor para IValidador con "new"
        /// </remarks>
        private readonly List<IValidador<T>> _validadores = new List<IValidador<T>>();

        /// <summary>
        /// envia un mensaje al programador
        /// </summary>
        /// <remarks>
        /// en caso de que el valor sea nulo o en blanco se le envia un mensaje al programador
        /// </remarks>
        public CompositeValidador(IEnumerable<IValidador<T>> validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }
        /// <summary>
        /// es un constructor
        /// </summary>
        /// <remarks>
        /// recibe reglas de validacion y evita que vengan con espacios en blanco 
        /// </remarks>
        public CompositeValidador(params IValidador<T>[] validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public ResultadoValidacion Validar(T valor)
        {
            var errores = new List<ErrorValidacion>();

            foreach (var v in _validadores)
            {
                var resultado = v.Validar(valor);
                if (!resultado.Valido)
                    errores.AddRange(resultado.Errores);
            }

            return errores.Count == 0 ? ResultadoValidacion.Correcto() : ResultadoValidacion.ConErrores(errores);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public sealed class Producto
    {
        public string Codigo { get; }
        public string Nombre { get; }
        public decimal Precio { get; }

        public Producto(string codigo, string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
            if (precio < 0m)
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");

            Codigo = codigo.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Precio = precio;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public sealed class LineaPedido
    {
        public Producto Producto { get; }
        public int Cantidad { get; }

        public LineaPedido(Producto producto, int cantidad)
        {
            Producto = producto ?? throw new ArgumentNullException(nameof(producto));
            if (cantidad < 1) throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser >= 1.");
            Cantidad = cantidad;
        }

        public decimal TotalLinea()
        {
            return Producto.Precio * Cantidad;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public sealed class Pedido
    {
        private readonly List<LineaPedido> _lineas = new List<LineaPedido>();

        public string Id { get; }
        public string ClienteId { get; }
        public EstadoPedido Estado { get; private set; }
        public IReadOnlyList<LineaPedido> Lineas => _lineas;

        public DateTime FechaCreacionUtc { get; }
        public DateTime? FechaPagoUtc { get; private set; }

        public Pedido(string id, string clienteId, DateTime fechaCreacionUtc)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El id es obligatorio.", nameof(id));
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException("El clienteId es obligatorio.", nameof(clienteId));

            Id = id.Trim();
            ClienteId = clienteId.Trim();
            FechaCreacionUtc = fechaCreacionUtc;
            Estado = EstadoPedido.Creado;
        }

        public void AgregarLinea(LineaPedido linea)
        {
            if (linea is null) throw new ArgumentNullException(nameof(linea));
            if (Estado != EstadoPedido.Creado)
                throw new InvalidOperationException("Solo se pueden agregar líneas cuando el pedido está Creado.");

            _lineas.Add(linea);
        }

        public decimal Total()
        {
            if (_lineas.Count == 0)
                throw new InvalidOperationException("El pedido no tiene líneas.");

            return _lineas.Sum(l => l.TotalLinea());
        }

        public void MarcarPagado()
        {
            if (Estado != EstadoPedido.Creado)
                throw new InvalidOperationException("Solo se puede marcar pagado desde estado Creado.");

            Estado = EstadoPedido.Pagado;
            FechaPagoUtc = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            if (Estado == EstadoPedido.Cancelado) return;

            if (Estado == EstadoPedido.Pagado)
                throw new InvalidOperationException("Un pedido pagado no puede cancelarse.");

            Estado = EstadoPedido.Cancelado;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public sealed class RepositorioPedidosEnMemoria
    {
        private readonly Dictionary<string, Pedido> _almacen = new Dictionary<string, Pedido>(StringComparer.Ordinal);

        public Pedido? ObtenerPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El id es obligatorio.", nameof(id));

            _almacen.TryGetValue(id.Trim(), out var pedido);
            return pedido;
        }

        public void Guardar(Pedido pedido)
        {
            if (pedido is null) throw new ArgumentNullException(nameof(pedido));
            _almacen[pedido.Id] = pedido;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class DemoIntermedio
    {
        public static ResultadoValidacion ProbarValidaciones()
        {
            var producto = new Producto("A1", "Auriculares", 50m);

            IValidador<Producto> validador = new CompositeValidador<Producto>(
                new ValidadorNoNulo<Producto>("producto")
            );

            return validador.Validar(producto);
        }
    }
}
