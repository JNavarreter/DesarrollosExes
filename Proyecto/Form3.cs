using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Proyecto.Form2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Proyecto
{
    public partial class Form3 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        public Form3()
        {
            InitializeComponent();

            //// Configurar la conexión a la base de datos
            // string cadenaConexion = "Data Source= localhost;Initial Catalog= DPERSONALIZADO;Integrated Security=True";
            string cadenaConexion = "Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!";
            conexion = new SqlConnection(cadenaConexion);
            
            CargarRol();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombreusu = txtuser.Text;
            string contraseña = ObtenerHash(txtpass.Text);
            string confirmacionContraseña = ObtenerHash(txtconfirmpass.Text);

            // Validación básica
            if (string.IsNullOrEmpty(nombreusu) || string.IsNullOrEmpty(contraseña) || string.IsNullOrEmpty(confirmacionContraseña))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            // Verificar si el usuario ya existe
            if (UsuarioExiste(nombreusu))
            {
                MessageBox.Show("Este nombre de usuario ya está en uso. Elige otro.");
                return;
            }
            if (contraseña != confirmacionContraseña)
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.");
                return;
            }

            int idRolNuevoUsuario = ObtenerIdRolParaNuevoUsuario();
            // Insertar el nuevo usuario en la base de datos
            InsertarUsuario(nombreusu, contraseña,idRolNuevoUsuario);

            MessageBox.Show("Registro exitoso. Ahora puedes iniciar sesión.");
            Hide();
            Form1 Form1 = new Form1();
            Form1.Show();
        }
        private bool UsuarioExiste(string nombreUsuario)
        {
            //using (SqlConnection conexion = new SqlConnection("Data Source= localhost;Initial Catalog= DPERSONALIZADO;Integrated Security=True"))
            using (SqlConnection conexion = new SqlConnection("Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!"))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Usuarios WHERE NombreUsu = @NombreUsu", conexion))
                {
                    cmd.Parameters.AddWithValue("@NombreUsu", nombreUsuario);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        private void InsertarUsuario(string nombreUsuario, string contraseña,int idRol)
        {

            //using (SqlConnection conexion = new SqlConnection("Data Source= localhost;Initial Catalog= DPERSONALIZADO;Integrated Security=True"))
            using (SqlConnection conexion = new SqlConnection("Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!"))
            {
                conexion.Open();
               
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Usuarios] ([NombreUsu],[Password],[IdRol]) VALUES (@NombreUsu, @Password,@IdRol)", conexion))
                {
                    cmd.Parameters.AddWithValue("@NombreUsu", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Password", contraseña);
                    cmd.Parameters.AddWithValue("@IdRol", idRol);
                    cmd.ExecuteNonQuery();
                }
                	
            }
        }
        private string ObtenerHash(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private int ObtenerIdRolParaNuevoUsuario()
        {
            if (comboBox1.SelectedItem != null)
            {
                string nombreRol = comboBox1.SelectedItem.ToString();
                return ObtenerIdRolPorNombre(nombreRol);
            }
            return -1;
        }
        private int ObtenerIdRolPorNombre(string nombreRol)
        {
            //using (SqlConnection conexion = new SqlConnection("Data Source=localhost;Initial Catalog=DPERSONALIZADO;Integrated Security=True"))
            using (SqlConnection conexion = new SqlConnection("Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!"))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT IdRol,Nombre FROM Roles WHERE Nombre = @Nombre", conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreRol);
                    object result = cmd.ExecuteScalar();
                    return result != null ? (int)result : -1; // Devuelve -1 si no se encuentra el rol
                }
            }
        }


        //Botones cerrar/minimizar
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro de Regresar?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                Hide();
            Form4 Form4 = new Form4();
            Form4.Show();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtpass.UseSystemPasswordChar = !checkBox1.Checked;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            txtconfirmpass.UseSystemPasswordChar = !checkBox2.Checked;
        }
        private void CargarRol()
        {
            try
            {
                conexion.Open();

                // Consulta para cargar las Area
                string consulta = "SELECT IdRol,Nombre FROM Roles";
                comando = new SqlCommand(consulta, conexion);
                lector = comando.ExecuteReader();

                // Limpiar comboBox1 antes de cargar nuevas Area
                comboBox1.Items.Clear();

                while (lector.Read())
                {
                    // Asumiendo que tienes una clase Area con propiedades IdArea y AreaNombre
                    Rol Rol = new Rol
                    {
                        IdRol = lector.GetInt32(0),
                        Nombre = lector.GetString(1)
                    };

                    // Agregar la categoría al combobox
                    comboBox1.Items.Add(Rol);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Roles " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }
    }
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public List<string> Roles { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
