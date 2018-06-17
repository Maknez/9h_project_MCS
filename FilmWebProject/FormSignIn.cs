using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace FilmWebProject
{
    public partial class FormSignIn : Form
    {
        private Form formSignedIn;
        private Form formSignUp;
        public FormSignIn()
        {
            InitializeComponent();
        }

        private string MD5(string Value)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(Value);
            data = x.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }

        private void signInButton_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseUsers.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            bool accountDoesNotExists = true;
            try
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from Users", sqlConnection);

                DataSet dataSetUsers = new DataSet("Users");
                sqlDataAdapter.FillSchema(dataSetUsers, SchemaType.Source, "Users");
                sqlDataAdapter.Fill(dataSetUsers, "Users");
                DataTable tableUsers = dataSetUsers.Tables["Users"];

                string username = usernameTextBox.Text;
                string password = MD5(passwordTextBox.Text);
             
                foreach (DataRow drCurrent in tableUsers.Rows)
                {
                    if (drCurrent["USERNAME"].ToString() == username) 
                    {
                        accountDoesNotExists = false;
                        if(drCurrent["PASSWORD"].ToString() == password)
                        {
                            if (formSignedIn == null)
                            {
                                formSignedIn = new FormSignedIn(Int32.Parse(drCurrent["ACCESS_LEVEL"].ToString()), username);
                            }
                            Hide();
                            formSignedIn.Show();
                        } else
                        {
                            errorMessage.Text = "Username and Password does not match!";
                        }
                    }
                }
            } catch (System.Data.SqlClient.SqlException ex)
            {
                System.Console.WriteLine(ex.Message);
            } finally
            {
                sqlConnection.Close();
            }
            if (accountDoesNotExists)
            {
                errorMessage.Text = "This account does not exists!";
            }
        }

        private void signUpButton_Click(object sender, EventArgs e)
        {
            if (formSignUp == null)
            {
                formSignUp = new FormSignUp();
            }
            Hide();
            formSignUp.Show();
        }
    }
}
