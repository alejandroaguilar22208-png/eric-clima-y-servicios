namespace ClimaGestion.Entidades
{
    public class ItemPresupuesto
    {
        private string descripcion = "";
        private decimal cantidad = 1;
        private decimal precioUnitario;

        public string Descripcion { get => descripcion; set => descripcion = value; }
        public decimal Cantidad { get => cantidad; set => cantidad = value; }
        public decimal PrecioUnitario { get => precioUnitario; set => precioUnitario = value; }

        public decimal Subtotal()
        {
            return Cantidad * PrecioUnitario;
        }
    }
}
