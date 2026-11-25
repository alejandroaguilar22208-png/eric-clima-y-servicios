using System.Drawing;
using System.Windows.Forms;

namespace ClimaGestion.Vista
{
    public static class Estilos
    {
        public static readonly Color Azul = Color.FromArgb(0, 92, 175);
        public static readonly Color AzulOscuro = Color.FromArgb(11, 35, 64);
        public static readonly Color AzulClaro = Color.FromArgb(207, 224, 243);
        public static readonly Color Verde = Color.FromArgb(226, 245, 230);
        public static readonly Color Rojo = Color.FromArgb(253, 226, 226);
        public static readonly Color Gris = Color.FromArgb(235, 235, 235);
        public static readonly Color TextoGris = Color.FromArgb(100, 116, 139);

        public static void Grilla(DataGridView grilla)
        {
            grilla.EnableHeadersVisualStyles = false;
            grilla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grilla.GridColor = Color.FromArgb(226, 232, 240);
            grilla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grilla.ColumnHeadersHeight = 36;
            grilla.ColumnHeadersDefaultCellStyle.BackColor = Azul;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = Azul;
            grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grilla.DefaultCellStyle.SelectionBackColor = AzulClaro;
            grilla.DefaultCellStyle.SelectionForeColor = AzulOscuro;
            grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 254);
            grilla.RowTemplate.Height = 30;
        }

        public static void OcultarColumnas(DataGridView grilla, params string[] columnas)
        {
            foreach (string columna in columnas)
            {
                if (grilla.Columns.Contains(columna)) grilla.Columns[columna].Visible = false;
            }
        }

        public static void FormatoPlata(DataGridView grilla, params string[] columnas)
        {
            foreach (string columna in columnas)
            {
                if (grilla.Columns.Contains(columna))
                {
                    grilla.Columns[columna].DefaultCellStyle.Format = "$ #,##0";
                    grilla.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        public static int IdSeleccionado(DataGridView grilla, string columna)
        {
            if (grilla.CurrentRow == null) return 0;
            return System.Convert.ToInt32(grilla.CurrentRow.Cells[columna].Value);
        }

        public static string TextoSeleccionado(DataGridView grilla, string columna)
        {
            if (grilla.CurrentRow == null) return "";
            return grilla.CurrentRow.Cells[columna].Value.ToString();
        }
    }
}
