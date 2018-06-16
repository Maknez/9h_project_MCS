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
    public partial class FormAddNewMovie : Form
    {
        private Form formSignedIn;
        private int access_level;
        private string username;
        public FormAddNewMovie(int user_access_level, string user_username)
        {
            InitializeComponent();
            access_level = user_access_level;
            username = user_username;
            this.movieTableAdapter.Fill(this.dataSetMovies.Movie);
        }

        private void movieBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.movieBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSetMovies);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (formSignedIn == null)
            {
                formSignedIn = new FormSignedIn(access_level, username);
            }
            Hide();
            formSignedIn.Show();
        }

        private void addNewMovieButton_Click(object sender, EventArgs e)
        {
            string title = titleTextBox.Text;
            string category = categoryTextBox.Text;
            string director = directorTextBox.Text;
            string year = yearDateTimePicker.Text;
            string country = countryTextBox.Text;
            string description = descriptionTextBox.Text;

            int movieID = 0;
            int categoryID = 0;
            int countryID = 0;
            int directorID = 0;
            int yearID = 0;


            if (!title.Equals("") && !category.Equals("") && !director.Equals("") && !year.Equals("") && !country.Equals("") && !description.Equals(""))
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                bool movieAlreadyInDatabase = false;
                errorMessage.ForeColor = Color.Red;
                try
                {
                    sqlConnection.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select MOVIE_NAME from Movie", sqlConnection);

                    DataSet dataSetMovie = new DataSet("MOVIE");
                    sqlDataAdapter.Fill(dataSetMovie, "MOVIE");
                    DataTable tableMovie = dataSetMovie.Tables["MOVIE"];

                    foreach (DataRow drCurrent in tableMovie.Rows)
                    {
                        if (drCurrent["MOVIE_NAME"].ToString() == title)
                        {
                            movieAlreadyInDatabase = true;
                        }
                    }

                    movieID = tableMovie.Rows.Count + 1;
                    if (!movieAlreadyInDatabase)
                    {
                        SqlDataAdapter[] addSqlDataAdapter = new SqlDataAdapter[4];
                        addSqlDataAdapter[0] = new SqlDataAdapter("Select * from Category", sqlConnection);
                        addSqlDataAdapter[1] = new SqlDataAdapter("Select * from Director", sqlConnection);
                        addSqlDataAdapter[2] = new SqlDataAdapter("Select * from Year", sqlConnection);
                        addSqlDataAdapter[3] = new SqlDataAdapter("Select * from Country", sqlConnection);

                        bool itemExists;

                        for (int i = 0; i < 4; i++)
                        {
                            itemExists = false;
                            DataSet dataSet = new DataSet("ITEM");
                            addSqlDataAdapter[i].Fill(dataSet, "ITEM");
                            DataTable table = dataSet.Tables["ITEM"];

                            foreach (DataRow drCurrent in table.Rows)
                            {
                                if (i == 0)
                                {
                                    if (drCurrent["CATEGORY_NAME"].ToString() == category)
                                    {
                                        categoryID = Int32.Parse(drCurrent["ID"].ToString());
                                        itemExists = true;
                                    }
                                }
                                if (i == 1)
                                {
                                    if (drCurrent["DIRECTOR_NAME"].ToString() == director)
                                    {
                                        directorID = Int32.Parse(drCurrent["ID"].ToString());
                                        itemExists = true;
                                    }
                                }
                                if (i == 2)
                                {
                                    if (drCurrent["YEAR_NAME"].ToString() == yearDateTimePicker.Text)
                                    {
                                        yearID = Int32.Parse(drCurrent["ID"].ToString());
                                        itemExists = true;
                                    }
                                }
                                if (i == 3)
                                {
                                    if (drCurrent["COUNTRY_NAME"].ToString() == country)
                                    {
                                        countryID = Int32.Parse(drCurrent["ID"].ToString());
                                        itemExists = true;
                                    }
                                }
                            }
                            if (!itemExists)
                            {
                                if (i == 0)
                                {
                                    categoryID = table.Rows.Count + 1;
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = String.Format("INSERT Category (ID, CATEGORY_NAME) VALUES ({0}, '{1}')", categoryID, category);
                                    cmd.Connection = sqlConnection;
                                    cmd.ExecuteNonQuery();
                                }
                                if (i == 1)
                                {
                                    directorID = table.Rows.Count + 1;
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = String.Format("INSERT Director (ID, DIRECTOR_NAME) VALUES ({0}, '{1}')", directorID, director);
                                    cmd.Connection = sqlConnection;
                                    cmd.ExecuteNonQuery();
                                }
                                if (i == 2)
                                {
                                    yearID = table.Rows.Count + 1;
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = String.Format("INSERT Year (ID, YEAR_NAME) VALUES ({0}, '{1}')", yearID, year);
                                    cmd.Connection = sqlConnection;
                                    cmd.ExecuteNonQuery();
                                }
                                if (i == 3)
                                {
                                    countryID = table.Rows.Count + 1;
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = String.Format("INSERT Country (ID, COUNTRY_NAME) VALUES ({0}, '{1}')", countryID, country);
                                    cmd.Connection = sqlConnection;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        SqlCommand cmdMovie = new SqlCommand();
                        cmdMovie.CommandType = System.Data.CommandType.Text;
                        cmdMovie.CommandText = String.Format("INSERT Movie (ID, MOVIE_NAME, DESCRIPTION, CATEGORY_ID, DIRECTOR_ID, YEAR_ID, COUNTRY_ID) VALUES ({0}, '{1}', '{2}', {3}, {4}, {5}, {6})", movieID, title, description, categoryID, directorID, yearID, countryID);
                        cmdMovie.Connection = sqlConnection;
                        cmdMovie.ExecuteNonQuery();
                        errorMessage.ForeColor = Color.Green;
                        errorMessage.Text = "Movie added successfully!";
                        ShowData();
                    }
                    else
                    {
                        errorMessage.Text = "Movie already in database!";
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
            else
            {
                errorMessage.Text = "You cannot save empty values!";
            }







        }

        private void FormAddNewMovie_Load(object sender, EventArgs e)
        {
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'dataSetMovies.Country' . Możesz go przenieść lub usunąć.
            this.countryTableAdapter.Fill(this.dataSetMovies.Country);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'dataSetMovies.Year' . Możesz go przenieść lub usunąć.
            this.yearTableAdapter.Fill(this.dataSetMovies.Year);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'dataSetMovies.Director' . Możesz go przenieść lub usunąć.
            this.directorTableAdapter.Fill(this.dataSetMovies.Director);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'dataSetMovies.Category' . Możesz go przenieść lub usunąć.
            this.categoryTableAdapter.Fill(this.dataSetMovies.Category);

        }

        private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoryTextBox.Text = categoryComboBox.Text;
        }

        private void directorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            directorTextBox.Text = directorComboBox.Text;
        }

        private void yearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            yearDateTimePicker.Text = yearComboBox.Text;
        }

        private void countryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            countryTextBox.Text = countryComboBox.Text;
        }

        private void ShowData()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\maknez\source\repos\FilmWebProject\FilmWebProject\DatabaseMovies.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();
                SqlDataAdapter[] sqlDataAdapter = new SqlDataAdapter[5];

                sqlDataAdapter[0] = new SqlDataAdapter("Select * from Movie", sqlConnection);
                sqlDataAdapter[1] = new SqlDataAdapter("Select * from Category", sqlConnection);
                sqlDataAdapter[2] = new SqlDataAdapter("Select * from Director", sqlConnection);
                sqlDataAdapter[3] = new SqlDataAdapter("Select * from Year", sqlConnection);
                sqlDataAdapter[4] = new SqlDataAdapter("Select * from Country", sqlConnection);
                for (int i = 0; i < 5; i++)
                {
                    DataSet dataSet = new DataSet("MOVIE");
                    sqlDataAdapter[i].Fill(dataSet, "MOVIE");
                    if (i == 0)
                    {
                        movieDataGridView.DataSource = dataSet.Tables["MOVIE"];
                    }
                    if (i == 1)
                    {
                        categoryBindingSource.DataSource = dataSet.Tables["MOVIE"];
                    }
                    if (i == 2)
                    {
                        directorBindingSource.DataSource = dataSet.Tables["MOVIE"];
                    }
                    if (i == 3)
                    {
                        yearBindingSource.DataSource = dataSet.Tables["MOVIE"];
                    }
                    if (i == 4)
                    {
                        countryBindingSource.DataSource = dataSet.Tables["MOVIE"];
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {

            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
