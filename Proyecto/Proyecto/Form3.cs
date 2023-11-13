using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
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
            string cadenaConexion = "Data Source= localhost;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            ////string cadenaConexion = "Data Source= 192.168.1.202 ;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            conexion = new SqlConnection(cadenaConexion);
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombreusu = txtuser.Text;
            string contraseña = ObtenerHash(txtpass.Text);

            // Validación básica
            if (string.IsNullOrEmpty(nombreusu) || string.IsNullOrEmpty(contraseña))
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

            // Insertar el nuevo usuario en la base de datos
            InsertarUsuario(nombreusu, contraseña);

            MessageBox.Show("Registro exitoso. Ahora puedes iniciar sesión.");
            Hide();
            Form1 Form1 = new Form1();
            Form1.Show();
        }
        private bool UsuarioExiste(string nombreUsuario)
        {
            using (SqlConnection conexion = new SqlConnection("Data Source= localhost;Initial Catalog= PERSONALIZADO;Integrated Security=True"))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Usuario WHERE NombreUsu = @NombreUsu", conexion))
                {
                    cmd.Parameters.AddWithValue("@NombreUsu", nombreUsuario);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        private void InsertarUsuario(string nombreUsuario, string contraseña)
        {

            using (SqlConnection conexion = new SqlConnection("Data Source= localhost;Initial Catalog= PERSONALIZADO;Integrated Security=True"))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Usuario] ([NombreUsu],[Password]) VALUES (@NombreUsu, @Password)", conexion))
                {
                    cmd.Parameters.AddWithValue("@NombreUsu", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Password", contraseña);
                    cmd.ExecuteNonQuery();
                }
                //}	
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
        //Botones cerrar/minimizar
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtpass.UseSystemPasswordChar = !checkBox1.Checked;
        }

    }
}
