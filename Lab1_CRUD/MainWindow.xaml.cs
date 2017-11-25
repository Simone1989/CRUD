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
        List<Director> directors = new List<Director>();
        Connection connection = new Connection();
        public readonly string connectionString = Connection.ConnectionString;

        public MainWindow()
        {
            InitializeComponent();
            PopulateDirectorListBox();
            // FÖR ATT FÅ ALLT ATT FUNGERA IGEN: TA BORT ALLA TASKS OCH DISPATCHERS
        }

        // Metod to populate the director listbox
        private void PopulateDirectorListBox()
        {
            // FÖRSÖKER ANVÄNDA KLASSERNA HÄR ISTÄLLET
            //listDirector.ItemsSource = directors;
            //listDirector.DisplayMemberPath = "DirectorInfo";
            //DataAccess db = new DataAccess();

            //directors = db.GetDirector();
            //listDirector. //visa här

            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        // När man använder whilen och reader.read ska man ladda in alla värden, som sedan ploppas in i objekt. 
                        connection.Open();
                        string query = "SELECT Fullname FROM Directors";
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            while (reader.Read())
                            {
                                listDirector.Items.Add(reader["Fullname"]);
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

        private void PopulateMovieListBox()
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM Movies INNER JOIN Directors ON Directors.Id = Movies.DirectorId WHERE DirectorId = @directorId";
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader;
                        Dispatcher.Invoke(() =>
                        {
                            command.Parameters.AddWithValue("@directorId", this.txtDirectorId.Text);
                        });
                        reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            listMovies.Items.Clear();
                            while (reader.Read())
                            {
                                listMovies.Items.Add(reader["Title"]);
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
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Directors WHERE Fullname = @fullname";
                    SqlCommand command = new SqlCommand(query, connection);
                    Dispatcher.Invoke(() =>
                    {
                        command.Parameters.AddWithValue("@fullname", listDirector.SelectedItem ?? (object)DBNull.Value);
                    });
                    SqlDataReader reader;
                    try
                    {
                        connection.Open();
                        reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            while (reader.Read())
                            {
                                string id = reader.GetInt32(0).ToString();
                                string fullanme = reader.GetString(1);
                                string birthday = reader.GetDateTime(2).Date.ToShortDateString();

                                txtDirectorId.Text = id;
                                txtDirectorName.Text = fullanme;
                                txtDirectorBirthday.Text = birthday;
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

        private void PopulateMovieTextboxes()
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Movies WHERE Title = @title";
                    SqlCommand command = new SqlCommand(query, connection);
                    Dispatcher.Invoke(() =>
                    {
                        command.Parameters.AddWithValue(@"title", listMovies.SelectedItem ?? (object)DBNull.Value);
                    });
                    SqlDataReader reader;
                    try
                    {
                        connection.Open();
                        reader = command.ExecuteReader();

                        Dispatcher.Invoke(() =>
                        {
                            while (reader.Read())
                            {
                                // FÖRSÖK JOINA TABELLERNA SÅ ATT DET STÅR ETT NAMN I DIRECTOR ISTÄLLET FÖR EN SIFFRA
                                string id = reader.GetInt32(0).ToString();
                                string title = reader.GetString(1);
                                string releaseDate = reader.GetInt32(2).ToString();
                                string director = reader.GetInt32(3).ToString();

                                txtMovieId.Text = id;
                                txtMovieTitle.Text = title;
                                txtMovieReleaseDate.Text = releaseDate;
                                txtMovieDirector.Text = director;
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
                        Dispatcher.Invoke(() =>
                        {
                            listDirector.Items.Add(this.txtDirectorName.Text);
                        });
                        MessageBox.Show("Saved.");

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
                        PopulateMovieListBox();
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
                        PopulateMovieListBox();
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
                        PopulateMovieListBox();
                        ClearTextboxes();
                    }
                }
            });
        }

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

        private void listDirector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateMovieListBox();
            PopulateDirectorTextboxes();

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
                else if (listDirector.SelectedIndex != -1)
                {
                    txtMovieDirector.Text = "";
                    txtMovieReleaseDate.Text = "";
                    txtMovieTitle.Text = "";
                    txtMovieId.Text = "";
                }
            });
        }

        private void txtDirectorId_TextChanged(object sender, TextChangedEventArgs e)
        {
            //btnInsertDirector.IsEnabled = !string.IsNullOrWhiteSpace(txtDirectorId.Text);
        }

        private void txtDirectorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //btnInsertDirector.IsEnabled = !string.IsNullOrWhiteSpace(txtDirectorName.Text);
        }
    }
}
