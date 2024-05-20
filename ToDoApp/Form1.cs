using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int rowIndex = -1;
        public int rowId = -1;
        public int colIndex = -1;  

        private void Form1_Load(object sender, EventArgs e)
        {
            getAllToDoList();
        }

        private void newbutton_Click(object sender, EventArgs e)
        {
            reset();
            rowId = -1;
            rowIndex = -1;
        }

        private async void deletebutton_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var resp = await client.DeleteAsync("https://localhost:7281/api/todoitems/" + rowId);
            rowIndex = -1;
            rowId = -1;
            getAllToDoList();
            reset();




        }

        private async void savebutton_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();  // get post gibi istekler için
            
            var obj = new
            {
                //rowIndex,
                id = rowId,
                name = TitleTextBox.Text,
                isComplete = checkBox1.Checked,
            };

            
            if (rowId <= -1)
            {
                

                var resp = await client.PostAsync("https://localhost:7281/api/todoitems",
                    new StringContent(JsonConvert.SerializeObject(obj),
                    Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync();
            }
            else
            {
                var resp = await client.PutAsync("https://localhost:7281/api/todoitems/" + rowId, 
                    new StringContent(JsonConvert.SerializeObject(obj),
                    Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync();

                Console.WriteLine(resp);
            }
            
             getAllToDoList();
            reset();
            rowIndex = -1;
            rowId = -1;


        }

        public async void getAllToDoList()
        {   

            todolistview.Rows.Clear(); // tüm verileri tekrar kaydetmemek için listeyi temizler.
            var client = new HttpClient(); 
    
            var resp = await client.GetAsync("https://localhost:7281/api/todoitems").Result.Content.ReadAsStringAsync();
            if (resp != null)
            {
                var respList = JsonConvert.DeserializeObject<listModel[]>(resp);
                foreach (var item in respList)
                {
                    var index = todolistview.Rows.Add();
                    todolistview.Rows[index].Cells["Column1"].Value = item.name;
                    todolistview.Rows[index].Cells["Column2"].Value = item.isComplete == true ? "\u2714" : "X";
                    todolistview.Rows[index].Cells["Column3"].Value = item.id;

                    // respe gelenleri listmodel şeklinde respliste liste şeklinde çeviriyor


                    
                        // Satırın "isComplete" özelliğini kontrol etmek için bir if bloğu
                    if (item.isComplete == true)
                    {
                    // "isComplete" özelliği true olduğunda, ilgili satırın arka plan rengini yeşil yap
                        todolistview.Rows[index].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        todolistview.Rows[index].DefaultCellStyle.BackColor = Color.Red;
                    }
                  

                }
              }

                                                    
                 
        }
        private void todolistview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow clickedRow = todolistview.Rows[e.RowIndex];
            rowIndex = e.RowIndex;
            TitleTextBox.Text = (string)todolistview.Rows[e.RowIndex].Cells["Column1"].Value;
            checkBox1.Checked = todolistview.Rows[e.RowIndex].Cells["Column2"].Value == "\u2714" ? true : false;       
            rowId = (int)todolistview.Rows[e.RowIndex].Cells["Column3"].Value;



            /*  todolistview.Rows.Add(item.name);

              if (item.isComplete == true)
              {
                 todolistview.Rows[index].Cells["Column2"].Value = "yapıldı";
                  Console.WriteLine("yapıldı");
              }
              else
              {
                  todolistview.Rows[index].Cells["Column2"].Value = "yapılmadı";
              }*/


            //todolistview.Rows.RemoveAt(e.RowIndex);
        }


        public void reset()
        {
            TitleTextBox.Text = "";
            checkBox1.Checked = false;
        }
    }
    public class listModel
    {
        public int id {  get; set; }
        public string name { get; set; }
        public bool isComplete { get; set; }
    }
}


