namespace ClimaGestion.Entidades
{
    public class Material
    {
        private int id;
        private string nombre = "";
        private string unidad = "unidad";
        private decimal stockActual;
        private decimal stockMinimo;
        private decimal precio;

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Unidad { get => unidad; set => unidad = value; }
        public decimal StockActual { get => stockActual; set => stockActual = value; }
        public decimal StockMinimo { get => stockMinimo; set => stockMinimo = value; }
        public decimal Precio { get => precio; set => precio = value; }

        public bool StockBajo()
        {
            return StockActual <= StockMinimo;
        }

        public string Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre)) return "El nombre es obligatorio.";
            if (StockActual < 0 || StockMinimo < 0) return "El stock no puede ser negativo.";
            if (Precio < 0) return "El precio no puede ser negativo.";
            return "";
        }
    }
}
