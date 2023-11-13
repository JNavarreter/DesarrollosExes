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
    public partial class Form2 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        public Form2()
        {
            InitializeComponent();

            //// Configurar la conexión a la base de datos
            string cadenaConexion = "Data Source= localhost;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            ////string cadenaConexion = "Data Source= 192.168.1.202 ;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            conexion = new SqlConnection(cadenaConexion);

            // Cargar las Area al iniciar el formulario
            CargarArea();

            // Asociar el evento de cambio de selección en comboBox1
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

        }

        private void CargarArea()
        {
            try
            {
                conexion.Open();

                // Consulta para cargar las Area
                string consulta = "SELECT IdArea, AreaNombre FROM Area";
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
                MessageBox.Show("Error al cargar las Areas: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener la categoría seleccionada
            Desarrollo Ruta = (Desarrollo)comboBox2.SelectedItem;

            if (Ruta != null)
            {
                try
                {
                    conexion.Open();

                    // Consulta para cargar los Desarrollo relacionados con la categoría seleccionada
                    string consulta = "SELECT IdDesarrollo, NombreDesarrollo , RutaDesarrollo FROM Desarrollo WHERE IdDesarrollo = @IdDesarrollo";
                    comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@IdDesarrollo", Ruta.IdDesarrollo);
                    lector = comando.ExecuteReader();

                    // Limpiar comboBox2 antes de cargar nuevos Desarrollo
                    comboBox3.Items.Clear();

                    while (lector.Read())
                    {
                        // Asumiendo que tienes una clase Desarrollo con propiedades IdDesarrollo y NombreDesarrollo
                        Ruta Desarrollo = new Ruta
                        {
                            IdDesarrollo = lector.GetInt32(0),
                            NombreDesarrollo = lector.GetString(1),
                            RutaDesarrollo = lector.GetString(2)
                        };

                        // Agregar el Desarrollo al combobox
                        comboBox3.Items.Add(Desarrollo);
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
                    string consulta = "SELECT IdDesarrollo, NombreDesarrollo FROM Desarrollo WHERE IdAreas = @IdAreas";
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

        //Botones cerrar/minimizar y ejecutar
        private void btnCerrar_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            string rutaPrograma = comboBox3.Text;
            //// Obtiene la ruta del TextBox
            //string rutaPrograma = txtRutaPrograma.Text;

            if (!string.IsNullOrEmpty(rutaPrograma))
            {
                // Crear un proceso para ejecutar el programa
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = rutaPrograma,
                    UseShellExecute = true
                };

                try
                {
                    // Iniciar el proceso
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
                lbltexto.Text = "Por favor, ingresa la ruta del programa";
            }
        }

        //Propiedades
        public class Area
        {
            public int IdArea { get; set; }
            public string AreaNombre { get; set; }

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

            public override string ToString()
            {
                return NombreDesarrollo;
            }
        }
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
    }
}
