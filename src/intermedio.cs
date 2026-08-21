using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticaDocFX.Intermedio
{
    /// <summary>
    /// Su funcion es mostrar el estado del pedido del cliente
    /// </summary>
    /// <remarks>
    /// Usa elementos como Public para que sea visible para todos y Enum para crear la lista de funciones (Creado, pagado y cancelado) 
    /// </remarks>
    public enum EstadoPedido
    {
        Creado = 0,
        Pagado = 1,
        Cancelado = 2
    }
 
     /// <summary>
    /// Obtiene y muestra valores, tambien rechaza espacios en blanco y envia mensajes de correccion en caso de no cumplis con las excepciones 
    /// </summary>
    /// <remarks>
    /// La palabra Sealed (Sellado) sella la clase y evita que se pueda volver a usar 
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
        /// <param name="campo">
        /// Cadena que representa el campo
        /// </param>
        /// <param name="codigo">
        /// Cadena que representa el codigo
        /// </param>
        /// <param name="mensaje">
        /// Cadena que representa al mensaje
        /// </param>
        /// <value>
        /// Representa la correccion de errores de validacion
        /// </value>
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
        /// Brinda el resultado de la validacion
        /// </summary>
        /// <remarks>
        /// No es heredable
        /// </remarks>
    public sealed class ResultadoValidacion
    {
        /// <summary>
        /// Crea una lista no sobreescribible
        /// </summary>
        /// <remarks>
        /// Usa el ReadOnly y get; para crear texto solamente visible pero no editable.
        /// </remarks>
        public IReadOnlyList<ErrorValidacion> Errores { get; }
        /// <summary>
        /// Revisa si hay errores de Validacion
        /// </summary>
        /// <remarks>
        /// Con la variable booleana Valido se determina si hay errores por medio del true or false
        /// </remarks>
        public bool Valido => Errores.Count == 0;
        /// <summary>
        /// Usa una lista de errores ya hecha
        /// </summary>
        /// <remarks>
        /// Envia lo que se considere un error a la lista de errores ya establecida
        /// </remarks>
        /// <param name="errores">
        /// Guarda errores en la propiedad 
        /// </param>
        /// <value>
        /// Representa lo que es ser un identificador de errores
        /// </value>
        private ResultadoValidacion(List<ErrorValidacion> errores)
        {
            Errores = errores;
        }
        /// <summary>
        /// Crea una nueva lista de validacion
        /// </summary>
        /// <remarks>
        /// Crea una lista  para guardar datos de forma privada
        /// </remarks>
        public static ResultadoValidacion Correcto()
        {
            return new ResultadoValidacion(new List<ErrorValidacion>());
        }
        /// <summary>
        /// Crea una lista de errores
        /// </summary>
        /// <remarks>
        /// Identifica y enlista los errores cometidos por el usuario
        /// </remarks>
        public static ResultadoValidacion ConErrores(IEnumerable<ErrorValidacion> errores)
        {
            if (errores is null) throw new ArgumentNullException(nameof(errores));
            return new ResultadoValidacion(errores.ToList());
        }

    }
    /// <summary>
    /// Permite tener validaciones para productos, usuarios, etc
    /// </summary>
    /// <remarks>
    /// Es una interfaz con reglas
    /// </remarks>
    public interface IValidador<in T>
    {
        /// <summary>
        ///El constructor hace que la propiedad tenga un valor generico
        /// </summary>
        /// <param name="valor">
        /// Le da un valor generico a la propiedad, asi que le permite contener enteros y cadenas
        /// </param>
        ResultadoValidacion Validar(T valor);
    }

    /// <summary>
    /// Evita que no hayan datos con valores nulos
    /// </summary>
    /// <remarks>
    /// Se encarga de que los valores de los datos no tengan espacios o sean nulos
    /// </remarks>
    public sealed class ValidadorNoNulo<T> : IValidador<T>
    {
        /// <summary>
        /// Es la variabilidad del nombre del valor durante su validacion
        /// </summary>
        /// <remarks>
        /// Privatiza el ReadOnly y valida nombres de valores
        /// </remarks>
        private readonly string _nombreCampo;

        /// <summary>
        /// Evita que el campo del nombre del valor este en blanco
        /// </summary>
        /// <remarks>
        /// Usa excepciones, nombres en cadena y nameof para evitar errores de compilacion.
        /// </remarks>
        /// <param name="nombreCampo">
        /// Corresponde al nombramiento del espacio
        /// </param>
        /// <value>
        /// Representa el constructor de la clase
        /// </value>
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
        /// <param name="valor">
        /// Es el valor que se le da al nombre del campo, no puede ser nulo
        /// </param>
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
    /// Establece un  rango minimo y maximo que no puede ser excedido, ademas establece que su espacio debe ser sin espacios en blanco
    /// </summary>
    /// <remarks>
    /// Establece valores como enteros dobles y exige que no se dejen en blanco
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
        /// Establece un  limite minimo para el valor de la variable Minimo
        /// </summary>
        /// <remarks>
        /// Evita que el valor Minimo sea mayor al maximo con excepciones y condicionales
        /// </remarks>
        /// <param name="maximo">
        /// Es el rango maximo posible que puede tener el valor, debe ser mayor al minimo.
        /// </param>
        /// <param name="minimo">
        /// Es el rango minimo posible que puede tener el valor, debe ser menor al maximo.
        /// </param>
        public ValidadorRango(double minimo, double maximo)
        {
            if (minimo > maximo) throw new ArgumentOutOfRangeException(nameof(minimo), "El mínimo debe ser <= máximo.");
            Minimo = minimo;
            Maximo = maximo;
        }
        /// <summary>
        /// Evita que la cadena de campo sea un espacio en blanco
        /// Evita que el valor Minimo y Maximo se salgan de su rango
        /// </summary>
        /// <remarks>
        /// Usa una condicional y una excepcion para evitar espacios en blanco
        /// En casode que los valores Minimo y Maximo excendan su limite, devolvera el valor y enviara un mensaje de correccion.
        /// </remarks>
        /// <param name="campo">
        /// Cadena que corresponde al campo
        /// </param>
        /// <param name="valor">
        /// Doble que corresponde al valor
        /// </param>
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
    /// Enlista y valida los productos por medio de enumeracion logica
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
        /// Envia un mensaje al programador
        /// </summary>
        /// <remarks>
        /// En caso de que el valor sea nulo o en blanco se le envia un mensaje al programador
        /// </remarks>
        /// <param name="validadores">
        /// Dan valor a los datos recibidos antes de enviarlos a la lista
        /// </param>
        public CompositeValidador(IEnumerable<IValidador<T>> validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }
        /// <summary>
        /// Es un constructor
        /// </summary>
        /// <remarks>
        /// Recibe reglas de validacion y evita que vengan con espacios en blanco 
        /// </remarks>
        /// <param name="validadores">
        /// Lista de tamaño definido de interfaces de validador 
        /// </param>
        /// <value>
        /// 
        /// </value>
        public CompositeValidador(params IValidador<T>[] validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }
        /// <summary>
        /// Reporta los errores
        /// </summary>
        /// <remarks>
        /// Agarra los datos y entrega un reporte de errores al programador
        /// </remarks>
        /// <param name="valor">
        /// El entero numerico del resultado de la validación
        /// </param>
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
    /// Sella la clase Producto
    /// </summary>
    /// <remarks>
    /// Usa sealed para sellar una clase y hacerla no heredable
    /// </remarks>
    public sealed class Producto
    {   
        /// <summary>
        /// Atributa las propiedades Codigo, Nombre y Precio
        /// </summary>
        /// <remarks>
        /// Permite leer el valor de las propiedades con { get; }
        /// </remarks>
        public string Codigo { get; }
        public string Nombre { get; }
        public decimal Precio { get; }

        /// <summary>
        /// Evita que los espacios en los que deban ir las propiedades esten vacios o de otro modo, tengan espacios blancos
        /// </summary>
        /// <remarks>
        /// Usa herramientas como Trim y usa excepciones, tambien evita que el valor de precio sea negativo 
        /// y le permite tener decimales.
        /// </remarks>
        /// <param name="codigo">
        /// Cadena que representa el codigo del producto
        /// </param>
        /// <param name="nombre">
        /// Cadena que representa el nombre del producto
        /// </param>
        /// <param name="precio">
        /// Numero que representa el precio del producto
        /// </param>
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
    /// Sella una clase con sealed
    /// </summary>
    /// <remarks>
    /// Sella la clase LineaPedido y permite que sea leida por cualquier persona haciendola publica
    /// </remarks>
    public sealed class LineaPedido
    {
        /// <summary>
        /// Hace de la clase producto algo leible y se pueda tener su valor
        /// </summary>
        /// <remarks>
        /// Usa public y get; para leer y obtener valores
        /// </remarks>
        public Producto Producto { get; }

        /// <summary>
        /// Hace que la propiedad de Cantidad solo admita numeros enteros
        /// </summary>
        /// <remarks>
        /// Usa int para nombrar la propiedad como numerica
        /// </remarks>
        public int Cantidad { get; }

        /// <summary>
        /// Evita que el producto sea nulo o menor a 1
        /// </summary>
        /// <remarks>
        /// Como condicional en caso de que el producto sea menor a 1 no sera permitido, y se enviara un mensaje para que sea mayor a 1.
        /// </remarks>
        /// <param name="producto">
        /// Corresponde al tipo de producto
        /// </param>
        /// <param name="cantidad">
        /// Es la cantidad del mismo producto, debe ser mayor o igual que 1
        /// </param>
        public LineaPedido(Producto producto, int cantidad)
        {
            Producto = producto ?? throw new ArgumentNullException(nameof(producto));
            if (cantidad < 1) throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser >= 1.");
            Cantidad = cantidad;
        }
        /// <summary>
        /// Hace visible el total
        /// </summary>
        /// <remarks>
        /// Hace que el total pueda tener decimales  
        /// </remarks>
        public decimal TotalLinea()
        {
            return Producto.Precio * Cantidad;
        }
    }
    /// <summary>
    /// Corresponde al Pedido del cliente en lista, estado, informacion y fecha de pago y creacion
    /// </summary>
    /// <remarks>
    /// Usa public y sealed para sellar la clase
    /// </remarks>
    public sealed class Pedido
    {
        /// <summary>
        /// Hace que la LineaPedido sea unicamente de lectura
        /// </summary>
        /// <remarks>
        /// Usa ReadOnly y privatiza la propiedad
        /// </remarks>
        private readonly List<LineaPedido> _lineas = new List<LineaPedido>();

        /// <summary>
        /// Hace que las propiedades Id y ClienteId tengan un valor obtenible
        /// </summary>
        /// <remarks>
        /// Usa propiedades de cadena y get; para la obtencion y visualizacion de valores
        /// </remarks>
        public string Id { get; }
        public string ClienteId { get; }

        /// <summary>
        /// Publica la propiedad EstadoPedido y hace que solo pueda ser definida dentro de la clase
        /// </summary>
        /// <remarks>
        /// Publica la propiedad y la hace definible bajo una regla
        /// </remarks>
        public EstadoPedido Estado { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Hace que la propiedad no sea editable
        /// </remarks>
        public IReadOnlyList<LineaPedido> Lineas => _lineas;

        /// <summary>
        /// Registra la Fecha de Creacion
        /// </summary>
        /// <remarks>
        /// Hace que una fecha de creacion sea obtenible y guardada
        /// </remarks>
        public DateTime FechaCreacionUtc { get; }
        /// <summary>
        /// Registra la fecha de pago
        /// </summary>
        /// <remarks>
        /// Hace que la fecha de pago se pueda obtener y sea definida solo dentro de su clase
        /// </remarks>
        public DateTime? FechaPagoUtc { get; private set; }

        /// <summary>
        /// Hace que las variables sean visibles y evita que esten en blanco
        /// </summary>
        /// <remarks>
        /// Obliga a las variables estar llenas y correctas y elimina espacios en blanco 
        /// </remarks>
        /// <param name="clienteId">
        /// Es la identificacion del cliente en cadena
        /// </param>
        /// <param name="fechaCreacionUtc">
        /// Corresponde a la fecha exacta de la creacion del pedido
        /// </param>
        /// <param name="id">
        /// Es la identificacion del pedido en cadena
        /// </param>
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
        /// <summary>
        /// Significa que solo se creara la factura una vez el pedido haya sido creado
        /// </summary>
        /// <remarks>
        /// Usa condicionales y decide que en caso de que el pedido no este creado no se enviara ninguna linea en las lista de facturacion
        /// </remarks>
        /// <param name="linea">
        /// Corresponde a la linea individual por producto en la factura
        /// </param>
        public void AgregarLinea(LineaPedido linea)
        {
            if (linea is null) throw new ArgumentNullException(nameof(linea));
            if (Estado != EstadoPedido.Creado)
                throw new InvalidOperationException("Solo se pueden agregar líneas cuando el pedido está Creado.");

            _lineas.Add(linea);
        }

        /// <summary> 
        /// Pasa el valor Total a numero entero con decimal y devuelve mensaje en caso de que no haya un pedido
        /// </summary>
        /// <remarks>
        /// Convierte el total en un numero con decimales y devuelve un mensaje en caso de que el pedido no tenga objetos aun
        /// </remarks>
        public decimal Total()
        {
            if (_lineas.Count == 0)
                throw new InvalidOperationException("El pedido no tiene líneas.");

            return _lineas.Sum(l => l.TotalLinea());
        }
        /// <summary>
        /// Define el estado de cobro del Pedido
        /// </summary>
        /// <remarks>
        /// Solo se puede cobrar el pedido una vez este creado, se guarda el estado de cobro del pedido y su fecha de creacion.
        /// </remarks>
        public void MarcarPagado()
        {
            if (Estado != EstadoPedido.Creado)
                throw new InvalidOperationException("Solo se puede marcar pagado desde estado Creado.");

            Estado = EstadoPedido.Pagado;
            FechaPagoUtc = DateTime.UtcNow;
        }
        /// <summary>
        /// Evita que un pedido pagado sea cancelado.
        /// </summary>
        /// <remarks>
        /// En caso de que un pedido actualmente pagado sea cancelado, no se podra cancelar
        /// </remarks>
        public void Cancelar()
        {
            if (Estado == EstadoPedido.Cancelado) return;

            if (Estado == EstadoPedido.Pagado)
                throw new InvalidOperationException("Un pedido pagado no puede cancelarse.");

            Estado = EstadoPedido.Cancelado;
        }
    }
    /// <summary>
    /// Base de datos de los Pedidos
    /// </summary>
    /// <remarks>
    /// Guarda y recuerda los pedidos
    /// </remarks>
    public sealed class RepositorioPedidosEnMemoria
    {
        /// <summary>
        /// Guarda los pedidos por nombre en cadena
        /// </summary>
        /// <remarks>
        /// Diccionario que guarda los nombres de los pedidos en orden respecto a su cadena de nombre
        /// </remarks>
        /// <value>
        /// Diccionario de cadenas y pedidos
        /// </value>
        private readonly Dictionary<string, Pedido> _almacen = new Dictionary<string, Pedido>(StringComparer.Ordinal);

        /// <summary>
        /// Pide el id del pedido
        /// </summary>
        /// <remarks>
        /// Hace que la identificacion de los pedidos sea por medio del id unico de cada pedido
        /// </remarks>
        /// <param name="id">
        /// Corresponde a la cadena de identificacion del pedido 
        /// </param>
        public Pedido? ObtenerPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El id es obligatorio.", nameof(id));

            _almacen.TryGetValue(id.Trim(), out var pedido);
            return pedido;
        }
        /// <summary>
        /// Guarda los pedidos
        /// </summary>
        /// <remarks>
        /// Guarda los pedidos y los envia al almacen (diccionario)
        /// </remarks>
        /// <param name="pedido">
        /// Corresponde al pedido guardado
        /// </param>
        public void Guardar(Pedido pedido)
        {
            if (pedido is null) throw new ArgumentNullException(nameof(pedido));
            _almacen[pedido.Id] = pedido;
        }
    }
    /// <summary>
    /// Clase que prueba el archivo intermedio
    /// </summary>
    /// <remarks>
    /// Prueba de validacion en el archivo intermedio
    /// </remarks>
    public static class DemoIntermedio
    {
        /// <summary>
        /// Prueba las validaciones
        /// </summary>
        /// <remarks>
        /// Obtiene los productos ylos valida antes de enviarlos al diccionario
        /// </remarks>
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
