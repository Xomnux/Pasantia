using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticaDocFX.Intermedio
{
    public enum EstadoPedido
    {
        Creado = 0,
        Pagado = 1,
        Cancelado = 2
    }

    public sealed class ErrorValidacion
    {
        public string Codigo { get; }
        public string Mensaje { get; }
        public string? Campo { get; }

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

    public sealed class ResultadoValidacion
    {
        public IReadOnlyList<ErrorValidacion> Errores { get; }

        public bool Valido => Errores.Count == 0;

        private ResultadoValidacion(List<ErrorValidacion> errores)
        {
            Errores = errores;
        }

        public static ResultadoValidacion Correcto()
        {
            return new ResultadoValidacion(new List<ErrorValidacion>());
        }

        public static ResultadoValidacion ConErrores(IEnumerable<ErrorValidacion> errores)
        {
            if (errores is null) throw new ArgumentNullException(nameof(errores));
            return new ResultadoValidacion(errores.ToList());
        }
    }

    public interface IValidador<in T>
    {
        ResultadoValidacion Validar(T valor);
    }

    public sealed class ValidadorNoNulo<T> : IValidador<T>
    {
        private readonly string _nombreCampo;

        public ValidadorNoNulo(string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(nombreCampo))
                throw new ArgumentException("El nombre del campo es obligatorio.", nameof(nombreCampo));

            _nombreCampo = nombreCampo.Trim();
        }

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

    public sealed class ValidadorRango
    {
        public double Minimo { get; }
        public double Maximo { get; }

        public ValidadorRango(double minimo, double maximo)
        {
            if (minimo > maximo) throw new ArgumentOutOfRangeException(nameof(minimo), "El mínimo debe ser <= máximo.");
            Minimo = minimo;
            Maximo = maximo;
        }

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

    public sealed class CompositeValidador<T> : IValidador<T>
    {
        private readonly List<IValidador<T>> _validadores = new List<IValidador<T>>();

        public CompositeValidador(IEnumerable<IValidador<T>> validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }

        public CompositeValidador(params IValidador<T>[] validadores)
        {
            if (validadores is null) throw new ArgumentNullException(nameof(validadores));
            _validadores.AddRange(validadores);
        }

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
