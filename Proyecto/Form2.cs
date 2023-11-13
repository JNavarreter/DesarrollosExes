using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.MonthCalendar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Proyecto
{
    public partial class Form2 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        private Rol selectedRole;
        public Form2(Rol role)
        {
            InitializeComponent();
            selectedRole = role;
            //string connectionString = "Data Source= localhost;Initial Catalog=DPERSONALIZADO;Integrated Security=True";
            string connectionString = "Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!";
            conexion = new SqlConnection(connectionString);

            CargarAreasPorRol(selectedRole.IdRol);

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is Area selectedArea)
            {
                int idArea = selectedArea.IdArea;
                CargarDesarrollosPorArea(idArea);
            }
        }
        private void CargarAreasPorRol(int roleId)
        {
            try
            {
                conexion.Open();

                // Consulta para cargar las áreas relacionadas con el rol seleccionado
                string consulta = "SELECT IdArea, AreaNombre FROM Areas WHERE IdRol = @RoleId";
                comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@RoleId", roleId);
                lector = comando.ExecuteReader();

                // Limpiar comboBox1 antes de cargar nuevas áreas
                comboBox1.Items.Clear();

                while (lector.Read())
                {
                    // Asumiendo que tienes una clase Area con propiedades IdArea y AreaNombre
                    Area area = new Area
                    {
                        IdArea = lector.GetInt32(0),
                        AreaNombre = lector.GetString(1)
                    };

                    // Agregar el área al combobox
                    comboBox1.Items.Add(area);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las áreas: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }
        private void CargarDesarrollosPorArea(int areaId)
        {
            try
            {
                conexion.Open();

                // Consulta para cargar los Desarrollos relacionados con el área seleccionada
                string consulta = "SELECT IdDesarrollo, NombreDesarrollo FROM Desarrollos WHERE IdArea = @AreaId";
                comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@AreaId", areaId);
                lector = comando.ExecuteReader();

                // Limpiar comboBox2 antes de cargar nuevos Desarrollos
                comboBox2.Items.Clear();

                while (lector.Read())
                {
                    // Asumiendo que tienes una clase Desarrollo con propiedades IdDesarrollo y NombreDesarrollo
                    Desarrollos desarrollo = new Desarrollos
                    {
                        IdDesarrollo = lector.GetInt32(0),
                        NombreDesarrollo = lector.GetString(1)
                    };

                    // Agregar el Desarrollo al ComboBox2
                    comboBox2.Items.Add(desarrollo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los Desarrollos: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Desarrollos desarrolloSeleccionado = (Desarrollos)comboBox2.SelectedItem;

            if (desarrolloSeleccionado != null)
            {
                try
                {
                    conexion.Open();

                    string consulta = "SELECT IdDesarrollo, NombreDesarrollo, RutaDesarrollo FROM Desarrollos WHERE IdDesarrollo = @IdDesarrollo";
                    comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@IdDesarrollo", desarrolloSeleccionado.IdDesarrollo);
                    lector = comando.ExecuteReader();

                    if (lector.Read())
                    {
                        string rutaPrograma = lector.GetString(2);

                        if (!string.IsNullOrEmpty(rutaPrograma))
                        {
                            ProcessStartInfo psi = new ProcessStartInfo
                            {
                                FileName = rutaPrograma,
                                UseShellExecute = true
                            };

                            try
                            {
                                Process.Start(psi);
                                lbltexto.Text = "Programa abierto con éxito";
                            }
                            catch (Exception ex)
                            {
                                lbltexto.Text = $"Error al abrir el programa: {ex.Message}";
                            }
                        }
                        else
                        {
                            lbltexto.Text = "La ruta del programa es nula o vacía";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los Desarrollo: " + ex.Message);
                }
                finally
                {
                    conexion.Close();
                }
            }
        }
        //Botones cerrar/minimizar y ejecutar
        private void btnCerrar_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro de cerrar el programa?", "Warning",MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                Application.Exit();
        }
    }
    //Propiedades
    public class Ruta
    {
        public int IdDesarrollo { get; set; }
        public string NombreDesarrollo { get; set; }
        public string RutaDesarrollo { get; set; }
        public override string ToString()
        {
            return RutaDesarrollo;
        }
    }
    public class Area
    {
        public int IdArea { get; set; }
        public string AreaNombre { get; set; }
        public Rol IdRol { get; set; }
        public override string ToString()
        {
            return AreaNombre;
        }
    }
    public class Desarrollos
    {
        public int IdDesarrollo { get; set; }
        public string NombreDesarrollo { get; set; }
        public string RutaDesarrollo { get; set; }
        public Area Area { get; set; }

        public override string ToString()
        {
            return NombreDesarrollo;
        }
    }
}
