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
using System.Collections;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public Form1()
        {
            //string cadenaConexion = "Data Source= localhost;Initial Catalog= DPERSONALIZADO;Integrated Security=True";
            string cadenaConexion = "Server= 192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!";
            conexion = new SqlConnection(cadenaConexion);

            InitializeComponent();
            CargarRol();
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
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string nombreusu = txtuser.Text;
            string contraseña = EncryptPassword(txtpass.Text);
            Rol rolSeleccionado = (Rol)comboBox1.SelectedItem;

            if (comboBox1.SelectedIndex != -1)
            {
                int idRol = rolSeleccionado.IdRol;

                if (AutenticarUsuario(nombreusu, contraseña, out Rol usuarioRol))
                {
                    List<Area> areas = ObtenerAreasPorRol(usuarioRol.IdRol);

                    if (idRol == 8)
                    {
                        MessageBox.Show("Inicio de sesión exitoso");
                        Form4 form4 = new Form4();
                        Hide();
                        form4.Show();
                    }
                    //else if (comboBox1.SelectedItem is Rol selectedRol)
                    //{
                    //    MessageBox.Show("Inicio de sesión exitoso");
                    //    Form2 form2 = new Form2(selectedRol);
                    //    Hide();
                    //    form2.Show();
                    //}
                    else if (usuarioRol.IdRol == rolSeleccionado.IdRol)
                    {
                        MessageBox.Show("Inicio de sesión exitoso");
                        Form2 form2 = new Form2(rolSeleccionado);
                        Hide();
                        form2.Show();
                    }
                    else
                    {
                        MessageBox.Show("No tienes permisos para este rol.");
                    }
                }
                else
                {
                    MessageBox.Show("Tu Contraseña o Usuario no coinciden, Vuelve a Intentarlo.");
                }
            }
        }
        private List<Area> ObtenerAreasPorRol(int idRol)
        {
            List<Area> areas = new List<Area>();

            // Query to retrieve areas based on the selected role
            string query = "SELECT IdArea, AreaNombre FROM Areas WHERE IdRol = @IdRol";

            //using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=DPERSONALIZADO;Integrated Security=True"))
            using (SqlConnection connection = new SqlConnection("Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!"))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Area area = new Area
                        {
                            IdArea = reader.GetInt32(0),
                            AreaNombre = reader.GetString(1)
                        };
                        areas.Add(area);
                    }
                }
            }

            return areas;
        }
        private bool AutenticarUsuario(string nombreusu, string contraseña, out Rol usuarioRol)
        {
            usuarioRol = null;

            //using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=DPERSONALIZADO;Integrated Security=True"))
            using (SqlConnection connection = new SqlConnection("Server=192.168.1.202;Database=DPERSONALIZADO ;User Id= appuser;Password= MP4ppU$3r!"))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsu = @NombreUsu AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreUsu", nombreusu);
                cmd.Parameters.AddWithValue("@Password", contraseña);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Authentication successful, now retrieve the role details
                    string roleQuery = "SELECT IdRol, Nombre FROM Roles WHERE IdRol = (SELECT IdRol FROM Usuarios WHERE NombreUsu = @NombreUsu)";
                    SqlCommand roleCmd = new SqlCommand(roleQuery, connection);
                    roleCmd.Parameters.AddWithValue("@NombreUsu", nombreusu);

                    SqlDataReader reader = roleCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Rol rol = new Rol
                        {
                            IdRol = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        };
                        usuarioRol = rol; 

                        return true;
                    }
                }
            }
            return false;
        }
        private string EncryptPassword(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
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
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtpass.UseSystemPasswordChar = !checkBox1.Checked;
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
        private void btnRegistro_Click(object sender, EventArgs e)
        {
            Hide();
            Form3 Form3 = new Form3();
            Form3.Show();
        }
    }
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsu { get; set; }
        public string Password { get; set; }
        public Rol IdRol { get; set; }

        public override string ToString()
        {
            return NombreUsu;
        }
    }
}
