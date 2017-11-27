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
using System.Threading;

namespace Lab1_CRUD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Connection connection = new Connection();
        public readonly string connectionString = Connection.ConnectionString;
        private int SelectedDirector;

        public MainWindow()
        {
            InitializeComponent();
            PopulateDirectorListBox();
        }

        private int DirectorId;

        // Metod to populate the director listbox, gets called when program starts and when a director is added or removed. 
        private void PopulateDirectorListBox()
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    DisableButtonsWhenLoading();
                });
                Thread.Sleep(1500);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Dispatcher.Invoke(() =>
                    {
                        listDirector.Items.Clear();
                    });
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM Directors";
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string fullanme = reader.GetString(1);
                            DateTime birthday = reader.GetDateTime(2);

                            Dispatcher.Invoke(() =>
                            {
                                listDirector.Items.Add(new Director(id, fullanme, birthday));
                            });
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        Dispatcher.Invoke(() =>
                        {
                            btnInsertDirector.IsEnabled = true;
                            btnInsertMovie.IsEnabled = true;
                            btnClearTextboxes.IsEnabled = true;
                        });
                    }
                }
            });
        }

        // Gets called when a director is selected in the director listbox and when a movie is added or removed 
        private void PopulateMovieListBox(int directorId)
        {
            this.DirectorId = directorId;
            Task.Run(() =>
            {
                int dirId = directorId;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM Movies INNER JOIN Directors ON " +
                        "Directors.Id = Movies.DirectorId WHERE DirectorId = @directorId";
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader;
                            command.Parameters.AddWithValue("@directorId", directorId);
                        reader = command.ExecuteReader();

                        Dispatcher.Invoke(() =>
                        {
                            listMovies.Items.Clear();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.GetString(1);
                                int releaseDate = reader.GetInt32(2);
                                int director = reader.GetInt32(3);
                                listMovies.Items.Add(new Movie(id, title, releaseDate, directorId));
                            }
                        });
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
            });

        }

        // Method to write out the database values of the selected director in textboxes
        private void PopulateDirectorTextboxes()
        {
            if (listDirector.SelectedIndex >= 0)
            {
                Director d = (Director)listDirector.SelectedItem;
                SelectedDirector = (d).Id;
                Dispatcher.Invoke(() =>
                {
                    txtDirectorId.Text = (d).Id.ToString();
                    txtDirectorName.Text = (d).Fullname;
                    txtDirectorBirthday.Text = (d).Birthday.ToShortDateString();
                });
            }
        }

        // Method to write out the database values of the selected movie in textboxes
        private void PopulateMovieTextboxes()
        {
            if (listMovies.SelectedIndex >= 0)
            {
                Dispatcher.Invoke(() =>
                {
                    txtMovieId.Text = ((Movie)listMovies.SelectedItem).Id.ToString();
                    txtMovieTitle.Text = ((Movie)listMovies.SelectedItem).Title;
                    txtMovieReleaseDate.Text = ((Movie)listMovies.SelectedItem).ReleaseYear.ToString();
                    txtMovieDirector.Text = ((Movie)listMovies.SelectedItem).DirectorId.ToString();
                });
            }
        }

        // Button clicks director
        private void btnInsertDirector_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "INSERT INTO Directors (Fullname, Birthday) VALUES (@name, @birthday)";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@birthday", this.txtDirectorBirthday.Text);
                            command.Parameters.AddWithValue("@name", this.txtDirectorName.Text);
                        });
                        command.ExecuteNonQuery();
                        MessageBox.Show("Saved.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        PopulateDirectorListBox();
                    }
                }
            });
        }

        private void btnUpdateDirector_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE Directors SET Fullname = @name, Birthday = @birthday WHERE Id = @id";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@birthday", this.txtDirectorBirthday.Text);
                            command.Parameters.AddWithValue("@name", this.txtDirectorName.Text);
                            command.Parameters.AddWithValue("@id", this.txtDirectorId.Text);
                        });
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
                        Dispatcher.Invoke(() =>
                        {
                            listDirector.Items.Clear();
                        });
                        PopulateDirectorListBox();
                    }
                }
            });
        }

        private void btnDeleteDirector_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM Directors WHERE Id = @id";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue(@"id", txtDirectorId.Text);
                        });
                        command.ExecuteNonQuery();
                        MessageBox.Show("Successfully deleted.");
                        Dispatcher.Invoke(() =>
                        {
                            txtDirectorId.Text = "";
                            txtDirectorName.Text = "";
                            txtDirectorBirthday.Text = "";
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        PopulateDirectorListBox();
                        Dispatcher.Invoke(() =>
                        {
                            listDirector.Items.Clear();
                        });
                    }
                }
            });
        }

        // Button clicks movies
        private void btnInsertMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        // HÄR KOMMER ETT FEL NÄR MAN INSERTAR FILMER UTAN ATT MAN HAR KLICKAT PÅ NÅTT ANNAT FÖRST
                        connection.Open();
                        string query = "INSERT INTO Movies (Title, ReleaseYear, DirectorId) VALUES (@title, @year, @director)";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@title", this.txtMovieTitle.Text);
                            command.Parameters.AddWithValue("@year", this.txtMovieReleaseDate.Text);
                            command.Parameters.AddWithValue("@director", this.txtMovieDirector.Text);
                        });
                        command.ExecuteNonQuery();
                        MessageBox.Show("Saved.");
                        Dispatcher.Invoke(() =>
                        {
                            listMovies.Items.Clear();
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        PopulateMovieListBox(SelectedDirector);
                        ClearTextboxes();
                    }
                }
            });
        }

        private void btnUpdateMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE Movies SET Title = @title, ReleaseYear = @year, DirectorId = @directorId WHERE Id = @movieId";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@title", this.txtMovieTitle.Text);
                            command.Parameters.AddWithValue("@year", this.txtMovieReleaseDate.Text);
                            command.Parameters.AddWithValue("@directorId", this.txtMovieDirector.Text);
                            command.Parameters.AddWithValue("@movieId", this.txtMovieId.Text);
                        });
                        command.ExecuteNonQuery();
                        MessageBox.Show("Updated.");
                        Dispatcher.Invoke(() =>
                        {
                            listMovies.Items.Clear();
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        PopulateMovieListBox(SelectedDirector);
                        ClearTextboxes();
                    }
                }
            });
        }

        private void btnDeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM Movies WHERE Id = @movieId";
                        SqlCommand command = new SqlCommand(query, connection);
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@movieId", this.txtMovieId.Text);
                        });
                        command.ExecuteNonQuery();
                        MessageBox.Show("Successfully deleted.");
                        Dispatcher.Invoke(() =>
                        {
                            listMovies.Items.Clear();
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        PopulateMovieListBox(SelectedDirector);
                        ClearTextboxes();
                    }
                }
            });
        }

        // Clear the textboxes from text
        private void btnClearTextboxes_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtDirectorId.Text = "";
                txtDirectorName.Text = "";
                txtDirectorBirthday.Text = "";
                txtMovieDirector.Text = "";
                txtMovieReleaseDate.Text = "";
                txtMovieTitle.Text = "";
                txtMovieId.Text = "";
            });
        }

        private void DisableButtonsWhenLoading()
        {
            btnInsertDirector.IsEnabled = false;
            btnUpdateDirector.IsEnabled = false;
            btnDeleteDirector.IsEnabled = false;
            btnInsertMovie.IsEnabled = false;
            btnUpdateMovie.IsEnabled = false;
            btnDeleteMovie.IsEnabled = false;
            btnClearTextboxes.IsEnabled = false;
        }

        private void listDirector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listDirector.SelectedIndex >= 0)
            {
                SelectedDirector = ((Director)listDirector.SelectedItem).Id;
                PopulateMovieListBox(SelectedDirector);
                PopulateDirectorTextboxes();
            }

            // Buttons get enabled/disabled depending on selections in listboxes
            if (listDirector.SelectedIndex != -1)
            {
                btnDeleteDirector.IsEnabled = true;
                btnUpdateDirector.IsEnabled = true;
                btnDeleteMovie.IsEnabled = false;
                btnUpdateMovie.IsEnabled = false;
                ClearTextboxes();
            }

        }

        private void listMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateMovieTextboxes();

            // Buttons get enabled/disabled depending on selections in listboxes
            if (listMovies.SelectedIndex != -1)
            {
                btnDeleteMovie.IsEnabled = true;
                btnUpdateMovie.IsEnabled = true;
                btnDeleteDirector.IsEnabled = false;
                btnUpdateDirector.IsEnabled = false;
                ClearTextboxes();
            }
        }

        private void ClearTextboxes()
        {
            Dispatcher.Invoke(() =>
            {
                if (listMovies.SelectedIndex != -1)
                {
                    //txtDirectorId.Text = "";
                    //txtDirectorName.Text = "";
                    //txtDirectorBirthday.Text = "";
                }
                else if (listDirector.SelectedIndex != -1 || listDirector.SelectedItem == null)
                {
                    txtMovieDirector.Text = "";
                    txtMovieReleaseDate.Text = "";
                    txtMovieTitle.Text = "";
                    txtMovieId.Text = "";
                }
            });
        }

    }
}
