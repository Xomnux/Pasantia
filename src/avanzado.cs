using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticaDocFX.Avanzado
{
    public enum SeveridadLog
    {
        Traza = 0,
        Depurar = 1,
        Informacion = 2,
        Advertencia = 3,
        Error = 4,
        Critico = 5
    }

    public sealed class RegistroLog
    {
        public SeveridadLog Severidad { get; }
        public string Mensaje { get; }
        public DateTime FechaUtc { get; }
        public string? Contexto { get; }

        public RegistroLog(SeveridadLog severidad, string mensaje, DateTime fechaUtc, string? contexto = null)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje es obligatorio.", nameof(mensaje));

            Severidad = severidad;
            Mensaje = mensaje.Trim();
            FechaUtc = fechaUtc;
            Contexto = contexto?.Trim();
        }
    }

    public interface IRegistradorLog
    {
        void Escribir(RegistroLog registro);
    }

    public sealed class RegistradorLogEnMemoria : IRegistradorLog
    {
        private readonly List<RegistroLog> _registros = new List<RegistroLog>();
        public IReadOnlyList<RegistroLog> Registros => _registros;

        public void Escribir(RegistroLog registro)
        {
            if (registro is null) throw new ArgumentNullException(nameof(registro));
            _registros.Add(registro);
        }
    }

    public sealed class Identificador
    {
        public string Valor { get; }

        private Identificador(string valor) => Valor = valor;

        public static Identificador Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El identificador es obligatorio.", nameof(valor));

            var v = valor.Trim();
            if (v.Length < 3)
                throw new ArgumentException("El identificador es demasiado corto.", nameof(valor));

            return new Identificador(v);
        }

        public override string ToString() => Valor;
    }

    public readonly struct Dinero : IEquatable<Dinero>
    {
        public string Moneda { get; }
        public decimal Monto { get; }

        public Dinero(string moneda, decimal monto)
        {
            if (string.IsNullOrWhiteSpace(moneda))
                throw new ArgumentException("La moneda es obligatoria.", nameof(moneda));

            moneda = moneda.Trim().ToUpperInvariant();
            Moneda = moneda;
            Monto = monto;
        }

        public Dinero Sumar(Dinero otro)
        {
            VerificarMoneda(otro);
            return new Dinero(Moneda, Monto + otro.Monto);
        }

        public Dinero Multiplicar(decimal factor)
        {
            return new Dinero(Moneda, Monto * factor);
        }

        private void VerificarMoneda(Dinero otro)
        {
            if (!string.Equals(Moneda, otro.Moneda, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Las monedas deben coincidir.");
        }

        public override string ToString() => $"{Moneda} {Monto:0.##}";

        public bool Equals(Dinero other) => string.Equals(Moneda, other.Moneda, StringComparison.OrdinalIgnoreCase) && Monto == other.Monto;
        public override bool Equals(object? obj) => obj is Dinero d && Equals(d);
        public override int GetHashCode() => HashCode.Combine(Moneda.ToUpperInvariant(), Monto);
    }

    public enum EstadoTransaccion
    {
        Pendiente = 0,
        Completada = 1,
        Rechazada = 2
    }

    public sealed class ResultadoOperacion
    {
        public bool Exito { get; }
        public string Codigo { get; }
        public string Mensaje { get; }

        private ResultadoOperacion(bool exito, string codigo, string mensaje)
        {
            Exito = exito;
            Codigo = codigo;
            Mensaje = mensaje;
        }

        public static ResultadoOperacion Ok(string mensaje = "OK")
            => new ResultadoOperacion(true, "OK", mensaje);

        public static ResultadoOperacion Fallo(string codigo, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código requerido.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("Mensaje requerido.", nameof(mensaje));

            return new ResultadoOperacion(false, codigo.Trim(), mensaje.Trim());
        }
    }

    public interface IPuertaPago
    {
        ResultadoOperacion Cobrar(Dinero monto, string tokenMetodoPago, string llaveIdempotencia);
    }

    public sealed class PuertaPagoIdempotenteEnMemoria : IPuertaPago
    {
        private readonly HashSet<string> _llaves = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> _tokensRechazados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void MarcarTokenComoRechazado(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token requerido.", nameof(token));

            _tokensRechazados.Add(token.Trim());
        }

        public ResultadoOperacion Cobrar(Dinero monto, string tokenMetodoPago, string llaveIdempotencia)
        {
            if (string.IsNullOrWhiteSpace(tokenMetodoPago))
                throw new ArgumentException("Token requerido.", nameof(tokenMetodoPago));
            if (string.IsNullOrWhiteSpace(llaveIdempotencia))
                throw new ArgumentException("Idempotencia requerida.", nameof(llaveIdempotencia));
            if (monto.Monto <= 0m)
                return ResultadoOperacion.Fallo("MONTO_INVALIDO", "El monto debe ser mayor que 0.");

            if (_llaves.Contains(llaveIdempotencia))
                return ResultadoOperacion.Ok("Reintento aceptado por idempotencia.");

            _llaves.Add(llaveIdempotencia);

            if (_tokensRechazados.Contains(tokenMetodoPago))
                return ResultadoOperacion.Fallo("PAGO_RECHAZADO", "El método de pago fue rechazado.");

            return ResultadoOperacion.Ok($"Cobro completado: {monto}");
        }
    }

    public sealed class PedidoLinea
    {
        public string Sku { get; }
        public int Cantidad { get; }
        public Dinero PrecioUnitario { get; }

        public PedidoLinea(string sku, int cantidad, Dinero precioUnitario)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU requerido.", nameof(sku));
            if (cantidad < 1)
                throw new ArgumentOutOfRangeException(nameof(cantidad), "Cantidad debe ser >= 1.");

            Sku = sku.Trim().ToUpperInvariant();
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }

        public Dinero TotalLinea() => PrecioUnitario.Multiplicar(Cantidad);
    }

    public sealed class PedidoAgregado
    {
        private readonly List<PedidoLinea> _lineas = new List<PedidoLinea>();

        public Identificador Id { get; }
        public string ClienteId { get; }
        public EstadoTransaccion EstadoPago { get; private set; }
        public DateTime FechaCreacionUtc { get; }
        public DateTime? FechaPagoUtc { get; private set; }

        public IReadOnlyList<PedidoLinea> Lineas => _lineas;

        public PedidoAgregado(Identificador id, string clienteId, DateTime fechaCreacionUtc)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException("ClienteId requerido.", nameof(clienteId));

            ClienteId = clienteId.Trim();
            FechaCreacionUtc = fechaCreacionUtc;
            EstadoPago = EstadoTransaccion.Pendiente;
        }

        public void AgregarLinea(PedidoLinea linea)
        {
            if (linea is null) throw new ArgumentNullException(nameof(linea));
            if (EstadoPago != EstadoTransaccion.Pendiente)
                throw new InvalidOperationException("No se pueden agregar líneas tras intentar pago.");

            _lineas.Add(linea);
        }

        public Dinero Total()
        {
            if (_lineas.Count == 0)
                throw new InvalidOperationException("Pedido sin líneas.");

            var moneda = _lineas[0].PrecioUnitario.Moneda;
            Dinero total = new Dinero(moneda, 0m);

            foreach (var l in _lineas)
            {
                if (!string.Equals(l.PrecioUnitario.Moneda, moneda, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Todas las líneas deben compartir la misma moneda.");

                total = total.Sumar(l.TotalLinea());
            }

            return total;
        }

        public void MarcarPagoCompletado()
        {
            if (EstadoPago == EstadoTransaccion.Completada) return;
            if (EstadoPago == EstadoTransaccion.Rechazada)
                throw new InvalidOperationException("No se puede completar un pago rechazado.");

            EstadoPago = EstadoTransaccion.Completada;
            FechaPagoUtc = DateTime.UtcNow;
        }

        public void MarcarPagoRechazado()
        {
            if (EstadoPago == EstadoTransaccion.Rechazada) return;
            if (EstadoPago == EstadoTransaccion.Completada)
                throw new InvalidOperationException("No se puede rechazar un pago completado.");

            EstadoPago = EstadoTransaccion.Rechazada;
        }
    }

    public interface IRepositorioPedidos
    {
        PedidoAgregado? Obtener(Identificador id);
        void Guardar(PedidoAgregado pedido);
    }

    public sealed class RepositorioPedidosEnMemoria : IRepositorioPedidos
    {
        private readonly Dictionary<string, PedidoAgregado> _datos = new Dictionary<string, PedidoAgregado>(StringComparer.Ordinal);

        public PedidoAgregado? Obtener(Identificador id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            _datos.TryGetValue(id.Valor, out var pedido);
            return pedido;
        }

        public void Guardar(PedidoAgregado pedido)
        {
            if (pedido is null) throw new ArgumentNullException(nameof(pedido));
            _datos[pedido.Id.Valor] = pedido;
        }
    }

    public interface IProveedorHora
    {
        DateTime AhoraUtc { get; }
    }

    public sealed class HoraSistema : IProveedorHora
    {
        public DateTime AhoraUtc => DateTime.UtcNow;
    }

    public sealed class ServicioPago
    {
        private readonly IRepositorioPedidos _repositorio;
        private readonly IPuertaPago _puertaPago;
        private readonly IRegistradorLog _log;
        private readonly IProveedorHora _hora;

        public ServicioPago(IRepositorioPedidos repositorio, IPuertaPago puertaPago, IRegistradorLog log, IProveedorHora hora)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _puertaPago = puertaPago ?? throw new ArgumentNullException(nameof(puertaPago));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _hora = hora ?? throw new ArgumentNullException(nameof(hora));
        }

        public ResultadoOperacion Pagar(Identificador pedidoId, string tokenMetodoPago, string llaveIdempotencia)
        {
            if (pedidoId is null) throw new ArgumentNullException(nameof(pedidoId));
            if (string.IsNullOrWhiteSpace(tokenMetodoPago))
                throw new ArgumentException("Token requerido.", nameof(tokenMetodoPago));
            if (string.IsNullOrWhiteSpace(llaveIdempotencia))
                throw new ArgumentException("Idempotencia requerida.", nameof(llaveIdempotencia));

            var pedido = _repositorio.Obtener(pedidoId);
            if (pedido is null)
                return ResultadoOperacion.Fallo("PEDIDO_NO_ENCONTRADO", "No existe el pedido.");

            Dinero monto;
            try
            {
                monto = pedido.Total();
            }
            catch (Exception ex)
            {
                _log.Escribir(new RegistroLog(SeveridadLog.Error, $"Error calculando total: {ex.Message}", _hora.AhoraUtc, pedidoId.Valor));
                return ResultadoOperacion.Fallo("TOTAL_INVALIDO", "No se pudo calcular el total.");
            }

            _log.Escribir(new RegistroLog(SeveridadLog.Informacion, $"Iniciando cobro {monto}.", _hora.AhoraUtc, pedidoId.Valor));

            var resultadoCobro = _puertaPago.Cobrar(monto, tokenMetodoPago, llaveIdempotencia);
            if (!resultadoCobro.Exito)
            {
                pedido.MarcarPagoRechazado();
                _repositorio.Guardar(pedido);
                _log.Escribir(new RegistroLog(SeveridadLog.Advertencia, $"Cobro falló: {resultadoCobro.Mensaje}", _hora.AhoraUtc, pedidoId.Valor));
                return resultadoCobro;
            }

            pedido.MarcarPagoCompletado();
            _repositorio.Guardar(pedido);
            _log.Escribir(new RegistroLog(SeveridadLog.Informacion, "Pago completado.", _hora.AhoraUtc, pedidoId.Valor));

            return resultadoCobro;
        }
    }

    public sealed class MotorFlujos
    {
        private readonly List<Func<ContextoFlujo, ResultadoOperacion>> _pasos = new List<Func<ContextoFlujo, ResultadoOperacion>>();

        public MotorFlujos Agregar(Func<ContextoFlujo, ResultadoOperacion> paso)
        {
            if (paso is null) throw new ArgumentNullException(nameof(paso));
            _pasos.Add(paso);
            return this;
        }

        public ResultadoOperacion Ejecutar(ContextoFlujo contexto)
        {
            if (contexto is null) throw new ArgumentNullException(nameof(contexto));
            foreach (var paso in _pasos)
            {
                var r = paso(contexto);
                if (!r.Exito) return r;
            }

            return ResultadoOperacion.Ok("Flujo completado.");
        }
    }

    public sealed class ContextoFlujo
    {
        public Identificador PedidoId { get; }
        public string TokenMetodoPago { get; }
        public string LlaveIdempotencia { get; }
        public bool IntentarReintento { get; set; }

        public ContextoFlujo(Identificador pedidoId, string tokenMetodoPago, string llaveIdempotencia)
        {
            PedidoId = pedidoId ?? throw new ArgumentNullException(nameof(pedidoId));

            if (string.IsNullOrWhiteSpace(tokenMetodoPago))
                throw new ArgumentException("Token requerido.", nameof(tokenMetodoPago));
            if (string.IsNullOrWhiteSpace(llaveIdempotencia))
                throw new ArgumentException("Idempotencia requerida.", nameof(llaveIdempotencia));

            TokenMetodoPago = tokenMetodoPago.Trim();
            LlaveIdempotencia = llaveIdempotencia.Trim();
            IntentarReintento = false;
        }
    }

    public static class DemoAvanzado
    {
        public static (ResultadoOperacion resultado, IReadOnlyList<RegistroLog> logs) Ejecutar()
        {
            var repositorio = new RepositorioPedidosEnMemoria();
            var puertaPago = new PuertaPagoIdempotenteEnMemoria();
            var log = new RegistradorLogEnMemoria();
            var hora = new HoraSistema();

            var servicioPago = new ServicioPago(repositorio, puertaPago, log, hora);

            var pedidoId = Identificador.Crear("PED-1001");
            var pedido = new PedidoAgregado(pedidoId, "CLI-9", hora.AhoraUtc);
            pedido.AgregarLinea(new PedidoLinea("SKU-X", 2, new Dinero("USD", 25m)));
            pedido.AgregarLinea(new PedidoLinea("SKU-Y", 1, new Dinero("USD", 10m)));
            repositorio.Guardar(pedido);

            var contexto = new ContextoFlujo(pedidoId, tokenMetodoPago: "token_ok", llaveIdempotencia: "idem-1");

            var motor = new MotorFlujos()
                .Agregar(ctx =>
                {
                    if (ctx.IntentarReintento) return ResultadoOperacion.Ok("Reintento habilitado (demo).");
                    return ResultadoOperacion.Ok("Primera ejecución (demo).");
                })
                .Agregar(ctx => servicioPago.Pagar(ctx.PedidoId, ctx.TokenMetodoPago, ctx.LlaveIdempotencia));

            var resultado = motor.Ejecutar(contexto);
            return (resultado, log.Registros);
        }
    }
}
