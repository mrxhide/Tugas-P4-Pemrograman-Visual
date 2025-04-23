using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MocupAppBiodataDiri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cek koneksi ke database
            string connString = "Server=localhost;Database=biodata_db;Uid=root;Pwd=;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    MessageBox.Show("Berhasil koneksi ke database!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal koneksi ke database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Pilih Foto 3x4";
            openFileDialog.Filter = "File Gambar (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = new Bitmap(img, new Size(120, 160)); // Ukuran 3x4
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                // Logika saat memilih jenis kelamin Perempuan
            }
        }

        private void SimpanData(object sender, EventArgs e)
        {
            string nama = textBox1.Text;
            string tempatLahir = textBox2.Text;
            DateTime tanggalLahir = dateTimePicker1.Value;
            string alamat = textBox3.Text;
            string noTelepon = textBox4.Text;
            string jenisKelamin = radioButton1.Checked ? "Laki-laki" : "Perempuan";

            // Konversi gambar ke format byte[] dengan pengecekan gambar
            byte[] foto = null;
            if (pictureBox1.Image != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Tentukan encoder untuk format gambar (misalnya PNG atau JPEG)
                        ImageFormat format = pictureBox1.Image.RawFormat;
                        if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) ||
                            format.Equals(System.Drawing.Imaging.ImageFormat.Png))
                        {
                            pictureBox1.Image.Save(ms, format);  // Simpan gambar dengan format yang sesuai
                        }
                        else
                        {
                            // Jika format tidak dikenali, simpan sebagai JPEG
                            pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        foto = ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengonversi gambar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Keluar dari metode jika gambar gagal dikonversi
                }
            }
            else
            {
                MessageBox.Show("Foto belum diunggah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Koneksi ke database
            string connString = "Server=localhost;Database=biodata_db;Uid=root;Pwd=;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    // Tampilkan pesan jika koneksi berhasil
                    MessageBox.Show("Berhasil koneksi ke database!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Query untuk memasukkan data
                    string query = "INSERT INTO biodata (nama, tempat_lahir, tanggal_lahir, alamat, jenis_kelamin, no_telepon, foto) " +
                                   "VALUES (@nama, @tempatLahir, @tanggalLahir, @alamat, @jenisKelamin, @noTelepon, @foto)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@tempatLahir", tempatLahir);
                        cmd.Parameters.AddWithValue("@tanggalLahir", tanggalLahir);
                        cmd.Parameters.AddWithValue("@alamat", alamat);
                        cmd.Parameters.AddWithValue("@jenisKelamin", jenisKelamin);
                        cmd.Parameters.AddWithValue("@noTelepon", noTelepon);
                        cmd.Parameters.AddWithValue("@foto", foto);

                        cmd.ExecuteNonQuery();
                    }

                    // Menampilkan pesan setelah data disimpan
                    MessageBox.Show("Data berhasil disimpan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
