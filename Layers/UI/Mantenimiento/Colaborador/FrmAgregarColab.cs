﻿using Octopus.Enum;
using Octopus.Interfaces;
using Octopus.Layers.BLL;
using Octopus.Layers.DAL;
using Octopus.Layers.Entities;
using Octopus.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus.Layers.UI.Mantenimiento.Colaborador
{
    public partial class FrmAgregarColab : Form
    {
        byte[] imagen = null;
        private static readonly log4net.ILog _MyLogControlEventos =
                                log4net.LogManager.GetLogger("MyControlEventos");
        public FrmAgregarColab()
        {
            InitializeComponent();
            timer1.Start();
            if (this.cmbPuesto.Items.Count < 0)
            {
                MessageBox.Show("Debe crear puestos antes", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.10;
            }
            else
            {
                this.Opacity = 0;
                timer2.Stop();
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1)
            {
                this.Opacity += 0.10;
            }
            else
            {
                this.Opacity = 1;
                timer1.Stop();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.timer2.Start();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (imagen == null)
            {
                MessageBox.Show("Por favor seleccione la foto de perfil.", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnFoto.Focus();
                return;
            }

            if (txtNombres.Text == "")
            {
                MessageBox.Show("Por favor digite los nombres.", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombres.Focus();
                return;
            }

            if (txtApellidos.Text == "")
            {
                MessageBox.Show("Por favor digite los apellidos.", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellidos.Focus();
                return;
            }

            if (txtCorreo.Text == "")
            {
                MessageBox.Show("Por favor digite el correo.", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return;
            }

            if (mtbID.Text == "")
            {
                MessageBox.Show("Por favor digite la cédula", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtbID.Focus();
                return;
            }
            if (mtbID.Text.Length != 9)
            {
                MessageBox.Show("Por favor digite la cédula completa", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtbID.Focus();
                return;
            }

            if (txtIban.Text == "")
            {
                MessageBox.Show("Por favor digite la cuenta IBAN", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIban.Focus();
                return;
            }

            if (rtbDireccion.Text == "")
            {
                MessageBox.Show("Por favor digite la dirección", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDireccion.Focus();
                return;
            }

            double monto;
            try
            {
                monto = Convert.ToDouble(txtSalarioXHora.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Por favor digite el salario por hora correctamente", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalarioXHora.Focus();
                return;
            }

            if (txtUsuario.Text == "")
            {
                MessageBox.Show("Por favor digite el usuario", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (txtContrasena.Text == "")
            {
                MessageBox.Show("Por favor digite la contraseña", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
                return;
            }

            if (txtContrasena.Text.Length != 7)
            {
                MessageBox.Show("La contraseña debe ser de 7 dígitos", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
                return;
            }

            if (cmbDepartamento.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione el departamento", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepartamento.Focus();
                return;
            }

            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione el rol", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return;
            }

            if (cmbPuesto.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione el puesto", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPuesto.Focus();
                return;
            }

            if (cmbDepartamento.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione el departamento", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepartamento.Focus();
                return;
            }

            int id = Convert.ToInt32(mtbID.Text.Trim());
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            string correo = txtCorreo.Text.Trim();

            if (!IsEmailValido(correo))
            {
                MessageBox.Show("Dirección de correo electrónico no válida", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return;
            }

            string cuentaIban = txtIban.Text.Trim();
            string direccion =rtbDireccion.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string contra = txtContrasena.Text.Trim();
            Puesto puesto = cmbPuesto.SelectedItem as Puesto;
            int puestoID = puesto.ID;
            Departamento departamento = cmbDepartamento.SelectedItem as Departamento;
            int departamentoID = departamento.ID;
            int supervisorID;
            if (cmbSupervisor.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor seleccione el supervisor", "Cuidado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSupervisor.Focus();
                return;
            }
            string supervisorCmb = this.cmbSupervisor.SelectedItem.ToString();
            if (supervisorCmb == "Ninguno")
            {
                supervisorID = -1;
            }
            else
            {
                Supervisor supervisor = (Supervisor)cmbSupervisor.SelectedItem;
                supervisorID = supervisor.ID;
            }
            
            Rol rol = (Rol)cmbRol.SelectedItem;

            ColaboradorFactory colaboradorFactory = new ColaboradorFactory();
            IColaborador colaborador = colaboradorFactory.CrearColaborador(id, puestoID, nombres, apellidos, dtpFechaNacim.Value,
                direccion, departamentoID, monto, this.imagen, correo, supervisorID, cuentaIban,
                usuario, contra, rol, DateTime.Now);
            
            
                ColaboradorBLL colaboradorBLL = new ColaboradorBLL();
                colaboradorBLL.Insert(colaborador);
                _MyLogControlEventos.Info("Se insertó un colaborador");
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("edad"))
                {
                    MessageBox.Show("La edad debe ser mayor o igual a 18", "Opsss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _MyLogControlEventos.Error("Error: La edad debe ser mayor o igual a 18");
                }
                else if(ex.Message.Contains("IBAN"))
                {
                    MessageBox.Show("La cuenta IBAN ya está registrada", "Opsss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _MyLogControlEventos.Error("Error: La cuenta IBAN ya está registrada");
                }
                else if(ex.Message.Contains("PRIMARY KEY"))
                {
                    MessageBox.Show("La ID ya existe", "Opsss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _MyLogControlEventos.Error("Error: La ID ya existe");
                }
                else if (ex.Message.Contains("fecha"))
                {
                    MessageBox.Show("La fecha debe indicar que es mayor de edad", "Opsss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _MyLogControlEventos.Error("Error: La fecha debe indicar que es mayor de edad");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    _MyLogControlEventos.Error("Error: "+ex.Message);
                }
                return;
            }

            MessageBox.Show("Se creó el colaborador exitosamente", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            

        }

        private bool IsEmailValido(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        private void FrmAgregarColab_Load(object sender, EventArgs e)
        {
            LlenarCombos();
        }

        private void LlenarCombos()
        {
            List<Departamento> departamentos = DepartamentoBLL.ListaCompleta();
            if (departamentos.LongCount() > 0)
            {
                cmbDepartamento.DataSource = departamentos;
                cmbDepartamento.DisplayMember = "Nombre";
                cmbDepartamento.SelectedIndex = 0;
            }

            cmbRol.Items.Add(Rol.Administrador);
            cmbRol.Items.Add(Rol.Colaborador);
            cmbRol.Items.Add(Rol.Supervisor);
            cmbRol.SelectedIndex = 0;

            List<Puesto> puestos = PuestoBLL.ListaCompleta();

            if (puestos.LongCount() > 0)
            {
                cmbPuesto.DataSource = puestos;
                cmbPuesto.DisplayMember = "Nombre";
                cmbPuesto.SelectedIndex = 0;
            }

            List<Supervisor> supervisor = ColaboradorBLL.ListaSupervisores();
            
            if (supervisor.LongCount() > 0)
            {
                foreach (Supervisor item in supervisor)
                {
                    cmbSupervisor.Items.Add(item);
                }
                cmbSupervisor.SelectedIndex = 0;
            }

        }

        private void btnFoto_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Archivos de imagen|*.jpg";
                openFileDialog.InitialDirectory = Application.StartupPath;
                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.imagen = ImageToByteArray(openFileDialog.FileName);
                    MemoryStream ms = new MemoryStream(this.imagen);
                    Image imagen = Image.FromStream(ms);
                    pbFotografia.Image = imagen;

                }
            }
            catch (Exception)
            {

                throw;
            }
        
        
        
        }
        public byte[] ImageToByteArray(string imagePath)
        {
            byte[] imageBytes;
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    imageBytes = br.ReadBytes((int)fs.Length);
                }
            }
            return imageBytes;
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.txtNombres.Text = "";
            this.txtApellidos.Text = "";
            this.mtbID.Text = "";
            this.txtCorreo.Text = "";
            this.txtIban.Text = "";
            this.rtbDireccion.Text = "";
            this.txtUsuario.Text = "";
            this.txtContrasena.Text = "";
            this.txtSalarioXHora.Text = "";
            this.dtpFechaNacim.ResetText();

            this.cmbDepartamento.SelectedIndex = 0;
            this.cmbPuesto.SelectedIndex = 0;
            this.cmbRol.SelectedIndex = 0;

            if (cmbSupervisor.Items.Count < 0)
            {
                this.cmbSupervisor.SelectedIndex = 0;
            }
            
            this.imagen =null;
            this.pbFotografia.Image = new Bitmap(Application.StartupPath + @"\Imagenes\MenuAdmin\Iconos\Mantenimiento\Colaborador\usuario.png");

        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            string result = cmbRol.SelectedItem.ToString();
            if (result == "Supervisor")
            {
                cmbSupervisor.Items.Add("Ninguno");
                cmbSupervisor.SelectedIndex = 0;
            }
            else if(result != "Ninguno")
            {
                cmbSupervisor.Items.Remove("Ninguno");
            }
        }
    }
}
