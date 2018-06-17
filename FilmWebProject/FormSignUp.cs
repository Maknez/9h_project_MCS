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
    public partial class FormSignUp : Form
    {
        private Form formSignIn;

        public FormSignUp()
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

        private void createAccountButton_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseUsers.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            errorMessage.ForeColor = Color.Red;
            try
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from Users", sqlConnection);

                DataSet dataSetUsers = new DataSet("Users");
                sqlDataAdapter.FillSchema(dataSetUsers, SchemaType.Source, "Users");
                sqlDataAdapter.Fill(dataSetUsers, "Users");
                DataTable tableUsers = dataSetUsers.Tables["Users"];

                string firstname = firstNameTextBox.Text;
                string surname = surnameTextBox.Text;
                string username = usernameTextBox.Text;
                string password = MD5(passwordTextBox.Text);
                string confirmPassword = MD5(confirmPasswordTextBox.Text);

                int nextAccountID = tableUsers.Rows.Count + 1;
                bool accountAlreadyExists = false;

                foreach (DataRow drCurrent in tableUsers.Rows)
                {
                    if (drCurrent["USERNAME"].ToString() == username)
                    {
                        accountAlreadyExists = true;
                    }
                }
                if (!accountAlreadyExists)
                {
                    if (!firstname.Equals("") && !surname.Equals("") && !username.Equals("") && !password.Equals("") && !confirmPassword.Equals(""))
                    {
                        if (password.Equals(confirmPassword))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = System.Data.CommandType.Text;
                            if (adminCheckBox.Checked)
                            {
                                cmd.CommandText = String.Format("INSERT Users (ID, FIRST_NAME, SURNAME, USERNAME, PASSWORD, ACCESS_LEVEL) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', 2)", nextAccountID, firstname, surname, username, password);
                            }
                            else
                            {
                                cmd.CommandText = String.Format("INSERT Users (ID, FIRST_NAME, SURNAME, USERNAME, PASSWORD, ACCESS_LEVEL) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', 1)", nextAccountID, firstname, surname, username, password);
                            }

                            cmd.Connection = sqlConnection;
                            cmd.ExecuteNonQuery();
                            errorMessage.ForeColor = Color.Green;
                            errorMessage.Text = "Account created successfully!";
                            firstNameTextBox.Text = "";
                            surnameTextBox.Text = "";
                            usernameTextBox.Text = "";
                            passwordTextBox.Text = "";
                            confirmPasswordTextBox.Text = "";
                            adminCheckBox.Checked = false;
                        }
                        else
                        {
                            errorMessage.Text = "Confirm password does not match!";
                        }
                    }
                    else
                    {
                        errorMessage.Text = "You cannot save empty values!";
                    }
                }
                else
                {
                    errorMessage.Text = "This username is already in use!";
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (formSignIn == null)
            {
                formSignIn = new FormSignIn();
            }
            Hide();
            formSignIn.Show();
        }
    }
}
