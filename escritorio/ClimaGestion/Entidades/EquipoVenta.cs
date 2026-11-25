namespace ClimaGestion.Entidades
{
    public class EquipoVenta
    {
        private int id;
        private string marca = "";
        private string modelo = "";
        private string tipo = "split";
        private int frigorias;
        private bool frioCalor = true;
        private bool inverter;
        private decimal precio;
        private int stock;
        private string descripcion = "";
        private bool activo = true;

        public int Id { get => id; set => id = value; }
        public string Marca { get => marca; set => marca = value; }
        public string Modelo { get => modelo; set => modelo = value; }
        public string Tipo { get => tipo; set => tipo = value; }
        public int Frigorias { get => frigorias; set => frigorias = value; }
        public bool FrioCalor { get => frioCalor; set => frioCalor = value; }
        public bool Inverter { get => inverter; set => inverter = value; }
        public decimal Precio { get => precio; set => precio = value; }
        public int Stock { get => stock; set => stock = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public bool Activo { get => activo; set => activo = value; }

        public string Validar()
        {
            if (string.IsNullOrWhiteSpace(Marca)) return "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(Modelo)) return "El modelo es obligatorio.";
            if (Frigorias <= 0) return "Las frigorías tienen que ser mayores a 0.";
            if (Precio <= 0) return "El precio tiene que ser mayor a 0.";
            if (Stock < 0) return "El stock no puede ser negativo.";
            return "";
        }
    }
}
