using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace 日曆
{
    public partial class diarycs : Form
    {
        private DateTime selectedDate;
        private List<PictureBox> pictureBoxes = new List<PictureBox>();

        public diarycs(DateTime diaryDate)
        {
            InitializeComponent();
            selectedDate = diaryDate;
            pictureBoxes.Add(pictureBox1);
            pictureBoxes.Add(pictureBox2);
            pictureBoxes.Add(pictureBox3);
            pictureBoxes.Add(pictureBox4);
            pictureBoxes.Add(pictureBox5);
            pictureBoxes.Add(pictureBox6);

            foreach (var pictureBox in pictureBoxes)
            {
                pictureBox.Hide();
            }

            button1.Hide();
            button2.Hide();
            button3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();
        }

        private void moodcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox moodComboBox = new ComboBox();
            moodComboBox.Items.AddRange(new string[] { "😊", "😔", "😡", "😄", "😢" });
        }

        private void weathercomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox weathercomboBox = new ComboBox();
            weathercomboBox.Items.AddRange(new string[] { "☀️", "☁️", "🌧️", "❄️", "🌈" });
        }

        int totalphoto = 0;

        private void addbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
            openFileDialog.Multiselect = false; // 僅允許選擇一個文件
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // 獲取所選圖片的路徑
                string selectedImagePath = openFileDialog.FileName;

                for (int i = 0; i < pictureBoxes.Count; i++)
                {
                    if (pictureBoxes[i].Image == null)
                    {
                        pictureBoxes[i].Image = Image.FromFile(selectedImagePath);
                        pictureBoxes[i].Show(); // 確保 PictureBox 顯示
                        ShowDeleteButton(i); // 顯示對應的刪除按鈕
                        totalphoto++;
                        break;
                    }
                }
            }
        }

        private void ShowDeleteButton(int index)
        {
            switch (index)
            {
                case 0:
                    button1.Show();
                    break;
                case 1:
                    button2.Show();
                    break;
                case 2:
                    button3.Show();
                    break;
                case 3:
                    button4.Show();
                    break;
                case 4:
                    button5.Show();
                    break;
                case 5:
                    button6.Show();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemovePhoto(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemovePhoto(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemovePhoto(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RemovePhoto(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RemovePhoto(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RemovePhoto(5);
        }

        private void RemovePhoto(int index)
        {
            for (int i = index; i < pictureBoxes.Count - 1; i++)
            {
                pictureBoxes[i].Image = pictureBoxes[i + 1].Image;
            }
            pictureBoxes[pictureBoxes.Count - 1].Image = null;
            totalphoto--;
            UpdatePictureBoxVisibility();
        }

        private void UpdatePictureBoxVisibility()
        {
            if (totalphoto >= 6) { pictureBox6.Show(); button6.Show(); } else { pictureBox6.Hide(); button6.Hide(); }
            if (totalphoto >= 5) { pictureBox5.Show(); button5.Show(); } else { pictureBox5.Hide(); button5.Hide(); }
            if (totalphoto >= 4) { pictureBox4.Show(); button4.Show(); } else { pictureBox4.Hide(); button4.Hide(); }
            if (totalphoto >= 3) { pictureBox3.Show(); button3.Show(); } else { pictureBox3.Hide(); button3.Hide(); }
            if (totalphoto >= 2) { pictureBox2.Show(); button2.Show(); } else { pictureBox2.Hide(); button2.Hide(); }
            if (totalphoto >= 1) { pictureBox1.Show(); button1.Show(); } else { pictureBox1.Hide(); button1.Hide(); }
        }

        public void SetDateTimePickerValue(DateTime date)
        {
            dateTimePicker1.Value = date;
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            // 创建日记条目对象
            DiaryEntry entry = new DiaryEntry
            {
                Date = dateTimePicker1.Value,
                Mood = moodcomboBox.SelectedItem?.ToString(),
                Weather = weathercomboBox.SelectedItem?.ToString(),
                Context = context.Text,
                SelectedColor = selectedColor,
                PhotoFileNames = new List<string>()
            };

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                PictureBox pictureBox = pictureBoxes[i];
                if (pictureBox.Image != null)
                {
                    try
                    {
                        string photoFileName = $"{entry.Date.ToString("yyyy-MM-dd")}_photo{(i + 1):00}.jpg";
                        string photoFilePath = Path.Combine(DairyManager.DiariesFolder, selectedDate.ToString("yyyy-MM-dd"), photoFileName);
                        pictureBox.Image.Save(photoFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        entry.PhotoFileNames.Add(photoFileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存照片失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            DairyManager.SaveToFile(entry, selectedDate);
            MessageBox.Show("日记保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void OpenDiaryForm(DateTime selectedDate)
        {
            // 清空现有数据
            moodcomboBox.SelectedItem = null;
            weathercomboBox.SelectedItem = null;
            context.Text = string.Empty;
            foreach (var pictureBox in pictureBoxes)
            {
                pictureBox.Image = null;
                pictureBox.Hide();
            }
            BackColor = SystemColors.Control; // 重置背景色

            // 显示添加照片按钮
            addbutton.Show();

            // 生成文件名（可以使用日期作为文件名）
            string fileName = selectedDate.ToString("yyyy-MM-dd") + ".json";
            string filePath = Path.Combine(DairyManager.DiariesFolder, selectedDate.ToString("yyyy-MM-dd"), fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    // 读取 JSON 文件内容
                    string json = File.ReadAllText(filePath);

                    // 将 JSON 反序列化为 DiaryEntry 对象
                    DiaryEntry diaryEntry = JsonConvert.DeserializeObject<DiaryEntry>(json);

                    dateTimePicker1.Value = diaryEntry.Date;
                    moodcomboBox.SelectedItem = diaryEntry.Mood;
                    weathercomboBox.SelectedItem = diaryEntry.Weather;
                    context.Text = diaryEntry.Context;
                    BackColor = diaryEntry.SelectedColor;

                    for (int i = 0; i < diaryEntry.PhotoFileNames.Count; i++)
                    {
                        string photoFileName = diaryEntry.PhotoFileNames[i];
                        string photoFilePath = Path.Combine(DairyManager.DiariesFolder, selectedDate.ToString("yyyy-MM-dd"), photoFileName);
                        if (File.Exists(photoFilePath))
                        {
                            Image image = Image.FromFile(photoFilePath);
                            pictureBoxes[i].Show(); // 显示对应的 PictureBox
                            pictureBoxes[i].Image = image;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开 JSON 文件时出错: {ex.Message}");
            }
        }

        public Color selectedColor;

        private void colorbutton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                selectedColor = colorDialog.Color;
                this.BackColor = Color.White;
                this.Paint += (s, pe) =>
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.White, colorDialog.Color, LinearGradientMode.Horizontal))
                    {
                        pe.Graphics.FillRectangle(brush, this.ClientRectangle);
                    }
                };
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            selectedDate = dateTimePicker1.Value;
            OpenDiaryForm(selectedDate);
        }
    }
}
