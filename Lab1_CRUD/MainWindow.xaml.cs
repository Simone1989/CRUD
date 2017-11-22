using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Lab1_CRUD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Cinema;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public MainWindow()
        {
            InitializeComponent();
            PopulateDirectorListBox();
        }

        // Metod to populate the director listbox
        private void PopulateDirectorListBox()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Fullname FROM Directors";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listDirector.Items.Add(reader["Fullname"]);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void PopulateMovieListBox()
        {
            listMovies.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // FUNGERAR INTE SOM DET SKA ÄN, LÄSER IN EN FILMTITEL I TAGET, GÅR IGENOM LISTAN MED WHILEN
                    string query = "SELECT * FROM Movies INNER JOIN Directors ON Directors.Id = Movies.DirectorId WHERE Movies.Id = '" + txtDirectorId.Text + "' ";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listMovies.Items.Add(reader["Title"]);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Method to write out the database values of the selected director in textboxes
        private void PopulateDirectorTextboxes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Directors WHERE Fullname = '" + listDirector.SelectedItem + "' ";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // FUNGERAR INTE SOM DET SKA ÄN MED BIRTHDAY
                        string id = reader.GetInt32(0).ToString();
                        string fullanme = reader.GetString(1);
                        //string birthday = reader.GetInt32(2).ToString();

                        txtDirectorId.Text = id;
                        txtDirectorName.Text = fullanme;
                        //txtDirectorName.Text = birthday;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void PopulateMovieTextboxes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Movies WHERE Title = '" + listMovies.SelectedItem + "' ";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // FÖRSÖK JOINA TABELLERNA SÅ ATT DET STÅR ETT NAMN I DIRECTOR ISTÄLLET FÖR EN SIFFRA
                        string title = reader.GetString(1);
                        string releaseDate = reader.GetInt32(2).ToString();
                        string director = reader.GetInt32(3).ToString();

                        txtMovieTitle.Text = title;
                        txtMovieReleaseDate.Text = releaseDate;
                        txtMovieDirector.Text = director;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Button clicks
        private void btnInsertDirector_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Directors (Fullname, Birthday) VALUES ('" + this.txtDirectorName.Text + "' , '" + this.txtDirectorBirthday.Text + "')";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    listDirector.Items.Add(this.txtDirectorName.Text);
                    MessageBox.Show("Saved");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnUpdateDirector_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Directors SET Id = '" + this.txtDirectorId.Text + "' ," +
                        " Fullname = '" + this.txtDirectorName.Text + "', Birthday = '" + this.txtDirectorBirthday.Text + 
                        "' WHERE Id = '" + this.txtDirectorId.Text + "' ";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Updated");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // What happens when something is written in the director id textbox
        private void txtDirectorId_TextChanged(object sender, TextChangedEventArgs e)
        {
            //btnInsertDirector.IsEnabled = !string.IsNullOrWhiteSpace(txtDirectorId.Text);
        }

        // What happens when a selection is changed in the director listbox
        private void listDirector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listDirector.SelectedIndex != -1)
            {
                btnDeleteDirector.IsEnabled = true;
                btnUpdateDirector.IsEnabled = true;
            }
            PopulateDirectorTextboxes();
            PopulateMovieListBox();
            // Här ska director info dyka upp i director textboxarna
            // Filmer som är kopplade till directorn ska även visas i Movie-listboxen.

        }

        private void listMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateMovieTextboxes();
        }
    }
}
