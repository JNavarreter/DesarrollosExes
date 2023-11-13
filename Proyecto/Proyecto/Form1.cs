using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;
using static Proyecto.Form2;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public Form1()
        {
            //// Configurar la conexión a la base de datos
            string cadenaConexion = "Data Source= localhost;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            ////string cadenaConexion = "Data Source= 192.168.1.202 ;Initial Catalog= PERSONALIZADO;Integrated Security=True";
            conexion = new SqlConnection(cadenaConexion);

            InitializeComponent();
        }

        //Permite realizar movimiento de pantalla asi como los DLL nativos captura señales del mouse
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")] private extern static void ReleaseCapture();

        //Permite realizar movimiento de los mensajes en pantalla
        [DllImport("user32.DLL", EntryPoint = "SendMessage")] private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        //Cuadros de texto usuario y password
        private void txtuser_Enter(object sender, EventArgs e)
        {
            if (txtuser.Text == "Usuario")
            {
                txtuser.Text = "";
                txtuser.ForeColor = Color.LightGray;//COLOR DE LA LETRA
            }
        }

        private void txtuser_Leave(object sender, EventArgs e)
        {
            if (txtuser.Text == "")
            {
                txtuser.Text = "Usuario";
                txtuser.ForeColor = Color.White;//COLOR DE LA LETRA
            }
        }

        private void txtpass_Enter(object sender, EventArgs e)
        {
            if (txtpass.Text == "Contraseña")
            {
                txtpass.Text = "";
                txtpass.ForeColor = Color.LightGray;//COLOR DE LA LETRA
                txtpass.UseSystemPasswordChar = true;
            }
        }

        private void txtpass_Leave(object sender, EventArgs e)
        {
            if (txtpass.Text == "")
            {
                txtpass.Text = "Contraseña";
                txtpass.ForeColor = Color.White;//COLOR DE LA LETRA
                txtpass.UseSystemPasswordChar = false;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }


        // BOTONES CERRAR/MINIMIZAR Y INICIO DE SESION
        private void btncerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string nombreusu = txtuser.Text;
            string contraseña= EncryptPassword(txtpass.Text);

            if (VerificarLogin(nombreusu, contraseña))
            {
                MessageBox.Show("Inicio de sesión exitoso");
                // Aquí puedes redirigir a la siguiente pantalla o realizar otras acciones necesarias
                Hide();
                Form2 Form2 = new Form2();
                Form2.Show();
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos");
            }
        }
        private string EncryptPassword(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
        // Método para verificar el login
        private bool VerificarLogin(string nombreusu, string contraseña)
        {
            conexion.Open();
            string query = "SELECT COUNT(1) FROM Usuario WHERE NombreUsu=@NombreUsu AND Password=@Password";
            SqlCommand cmd = new SqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@NombreUsu", nombreusu);
            cmd.Parameters.AddWithValue("@Password", contraseña);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            conexion.Close();

            return count == 1;
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            Hide();
            Form3 Form3 = new Form3();
            Form3.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        { 
            txtpass.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsu { get; set; }

        public override string ToString()
        {
            return NombreUsu;
        }
    }
}
