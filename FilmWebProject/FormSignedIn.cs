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
    public partial class FormSignedIn : Form
    {
        private Form formSignIn;
        private Form formAddNewMovie;
        private int access_level;
        private string username;
        public FormSignedIn(int user_access_level, string user_username)
        {
            InitializeComponent();
            access_level = user_access_level;
            if (access_level == 1)
            {
                addButton.Visible = false;
                removeButton.Visible = false;
            }
            username = user_username;
            userNameLabel.Text = username;
            ShowData();
        }

        private void movieBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.movieBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSetMovies);
        }

        private void FormSignedInAdmin_Load(object sender, EventArgs e)
        {
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'dataSetMovies.Category' . Możesz go przenieść lub usunąć.
            this.categoryTableAdapter.Fill(this.dataSetMovies.Category);
            this.movieTableAdapter.Fill(this.dataSetMovies.Movie);

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (formAddNewMovie == null)
            {
                formAddNewMovie = new FormAddNewMovie(access_level, username);
            }
            Hide();
            formAddNewMovie.Show();
        }

        private void logOutButton_Click(object sender, EventArgs e)
        {
            if (formSignIn == null)
            {
                formSignIn = new FormSignIn();
            }
            Hide();
            formSignIn.Show();

        }

        private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int categoryID;
            try
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(String.Format("Select * from Category where CATEGORY_NAME = '{0}'", categoryComboBox.Text), sqlConnection);
                DataSet dataSetCategory = new DataSet("Category");
                sqlDataAdapter.FillSchema(dataSetCategory, SchemaType.Source, "Category");
                sqlDataAdapter.Fill(dataSetCategory, "Category");
                DataTable tableCategory = dataSetCategory.Tables["Category"];

                DataRow drAnswer = tableCategory.Rows[0];
                categoryID = Int32.Parse(drAnswer["Id"].ToString());
//                movieBindingSource.Filter = String.Format("CATEGORY_ID = {0}", categoryID);

                SqlDataAdapter sqlDataAdapterMovie = new SqlDataAdapter(String.Format("Select * from Movie where CATEGORY_ID = {0}", categoryID), sqlConnection);

                DataSet dataSetMovie = new DataSet("MOVIE");
                sqlDataAdapterMovie.Fill(dataSetMovie, "MOVIE");
                movieDataGridView.DataSource = dataSetMovie.Tables["MOVIE"];
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
            }
            catch (System.IndexOutOfRangeException)
            {
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void movieDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = e.RowIndex;
                DataGridViewRow dataGridViewRow = movieDataGridView.Rows[index];
                titleTextBox.Text = dataGridViewRow.Cells[1].Value.ToString();
                descriptionTextBox.Text = dataGridViewRow.Cells[2].Value.ToString();

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connectionString);

                sqlConnection.Open();

                SqlDataAdapter[] sqlDataAdapter = new SqlDataAdapter[4];

                sqlDataAdapter[0] = new SqlDataAdapter(String.Format("Select * from Category where ID = {0}", Int32.Parse(dataGridViewRow.Cells[3].Value.ToString())), sqlConnection);
                sqlDataAdapter[1] = new SqlDataAdapter(String.Format("Select * from Director where ID = {0}", Int32.Parse(dataGridViewRow.Cells[4].Value.ToString())), sqlConnection);
                sqlDataAdapter[2] = new SqlDataAdapter(String.Format("Select * from Year where ID = {0}", Int32.Parse(dataGridViewRow.Cells[5].Value.ToString())), sqlConnection);
                sqlDataAdapter[3] = new SqlDataAdapter(String.Format("Select * from Country where ID = {0}", Int32.Parse(dataGridViewRow.Cells[6].Value.ToString())), sqlConnection);

                for (int i = 0; i < 4; i++)
                {
                    DataSet dataSet = new DataSet("DataSet");
                    sqlDataAdapter[i].Fill(dataSet, "DataSet");
                    DataTable dataTable = dataSet.Tables["DataSet"];
                    DataRow dataRow = dataTable.Rows[0];
                    if (i == 0)
                    {
                        categoryTextBox.Text = dataRow["CATEGORY_NAME"].ToString();
                    }
                    else if (i == 1)
                    {
                        directorTextBox.Text = dataRow["DIRECTOR_NAME"].ToString();
                    }
                    else if (i == 2)
                    {
                        yearTextBox.Text = dataRow["YEAR_NAME"].ToString();
                    }
                    else if (i == 3)
                    {
                        countryTextBox.Text = dataRow["COUNTRY_NAME"].ToString();
                    }
                }
            }
            catch (System.Exception)
            {
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";
                directorTextBox.Text = "";
                categoryTextBox.Text = "";
                yearTextBox.Text = "";
                countryTextBox.Text = "";
            }

        }
        private void removeButton_Click(object sender, EventArgs e)
        {
            try
            {
                int index = movieDataGridView.CurrentCell.RowIndex;
                DataGridViewRow dataGridViewRow = movieDataGridView.Rows[index];
                int indexOfObjectToDelete = Int32.Parse(dataGridViewRow.Cells[0].Value.ToString());
                System.Console.WriteLine(index + "   TEST    " + indexOfObjectToDelete);
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                sqlConnection.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = String.Format("DELETE FROM Movie WHERE ID = {0}", indexOfObjectToDelete);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
                errorMessage.Text = "Successfully removed";
                ShowData();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private void ShowData()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from Movie", sqlConnection);

                DataSet dataSetMovie = new DataSet("MOVIE");
                sqlDataAdapter.Fill(dataSetMovie, "MOVIE");
                movieDataGridView.DataSource = dataSetMovie.Tables["MOVIE"];

                SqlDataAdapter sqlDataAdapterCategory = new SqlDataAdapter("Select * from Category", sqlConnection);

                DataSet dataSetCategory = new DataSet("MOVIE");
                sqlDataAdapterCategory.Fill(dataSetCategory, "MOVIE");
                categoryBindingSource1.DataSource = dataSetCategory.Tables["MOVIE"];

            }
            catch (System.Data.SqlClient.SqlException)
            {

            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void showAllButton_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int categoryID;
            try
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapterMovie = new SqlDataAdapter("Select * from Movie", sqlConnection);

                DataSet dataSetMovie = new DataSet("MOVIE");
                sqlDataAdapterMovie.Fill(dataSetMovie, "MOVIE");
                movieDataGridView.DataSource = dataSetMovie.Tables["MOVIE"];
            }
            catch (System.Data.SqlClient.SqlException)
            {
            }
            catch (System.IndexOutOfRangeException)
            {
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
