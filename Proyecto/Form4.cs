using System;
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

namespace Proyecto
{
    public partial class Form4 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        public Form4()
        {
            InitializeComponent();
            string cadenaConexion = "Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!";
            conexion = new SqlConnection(cadenaConexion);

            // Cargar las Area al iniciar el formulario
            CargarArea();

            // Asociar el evento de cambio de selección en comboBox1
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void CargarArea()
        {
            try
            {
                conexion.Open();

                // Consulta para cargar las Area
                string consulta = "SELECT IdArea, AreaNombre FROM Areas";
                comando = new SqlCommand(consulta, conexion);
                lector = comando.ExecuteReader();

                // Limpiar comboBox1 antes de cargar nuevas Area
                comboBox1.Items.Clear();

                while (lector.Read())
                {
                    // Asumiendo que tienes una clase Area con propiedades IdArea y AreaNombre
                    Area Area = new Area
                    {
                        IdArea = lector.GetInt32(0),
                        AreaNombre = lector.GetString(1)
                    };

                    // Agregar la categoría al combobox
                    comboBox1.Items.Add(Area);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las Area: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         // Obtener la categoría seleccionada
            Area AreaSeleccionada = (Area)comboBox1.SelectedItem;

            if (AreaSeleccionada != null)
            {
                try
                {
                    conexion.Open();

                    // Consulta para cargar los Desarrollo relacionados con la categoría seleccionada
                    string consulta = "SELECT IdDesarrollo, NombreDesarrollo FROM Desarrollos WHERE IdArea = @IdArea";
                    comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@IdArea", AreaSeleccionada.IdArea);
                    lector = comando.ExecuteReader();

                    // Limpiar comboBox2 antes de cargar nuevos Desarrollo
                    comboBox2.Items.Clear();

                    while (lector.Read())
                    {
                        // Asumiendo que tienes una clase Desarrollo con propiedades IdDesarrollo y NombreDesarrollo
                        Desarrollo Desarrollo = new Desarrollo
                        {
                            IdDesarrollo = lector.GetInt32(0),
                            NombreDesarrollo = lector.GetString(1)
                        };

                        // Agregar el Desarrollo al combobox
                        comboBox2.Items.Add(Desarrollo);
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

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Desarrollo desarrolloSeleccionado = (Desarrollo)comboBox2.SelectedItem;

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

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3= new Form3();
            Hide();
            form3.Show();
        }
    }
    //Propiedades
    public class Rutas
    {
        public int IdDesarrollo { get; set; }
        public string NombreDesarrollo { get; set; }
        public string RutaDesarrollo { get; set; }
        public string DisplayText => NombreDesarrollo;

        public override string ToString()
        {
            return DisplayText;
        }
    }
    public class Areas
    {
        public int IdArea { get; set; }
        public string AreaNombre { get; set; }
        public Rol IdRol { get; set; }
        public override string ToString()
        {
            return AreaNombre;
        }
    }
    public class Desarrollo
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
