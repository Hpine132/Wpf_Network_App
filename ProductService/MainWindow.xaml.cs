using DataAccess.Logics;
using DataAccess.Models;
using Microsoft.VisualBasic;
using ProductService.Models;
using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;


namespace ProductService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static System.Text.Json.Serialization.ReferenceHandler IgnoreCycles { get; }
        ProductServices productService;
        CategoryService categoryService;
        TcpClient client;
        NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            productService = new ProductServices();
            categoryService = new CategoryService();

            string server = "127.0.0.1";
            int port = 1500;
            client = new TcpClient(server, port);
            stream = client.GetStream();
            LoadProducts();
        }

        private void LoadProducts()
        {
            cbCategory.ItemsSource = categoryService.GetCategories();
            dgProducts.ItemsSource = productService.GetProducts();
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem != null)
            {
                Product product = (Product)dgProducts.SelectedItem;
                tbProductID.Text = product.ProductId.ToString();
                tbProductName.Text = product.ProductName;
                tbUnitPrice.Text = product.UnitPrice.ToString();
                cbDiscontinued.IsChecked = product.Discontinued;
                tbQuantityPerUnit.Text = product.QuantityPerUnit;
                cbCategory.SelectedIndex = categoryService.GetCategories().FindIndex(c => c.CategoryId == product.CategoryId);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem != null)
            {
                Product product = (Product)dgProducts.SelectedItem;
                string ProductName = (String.IsNullOrEmpty(tbProductName.Text.Trim())) ? "" : tbProductName.Text.Trim();
                decimal unitPrice;
                bool Discontinued = (bool)cbDiscontinued.IsChecked;
                string? QuantityPerUnit = tbQuantityPerUnit.Text.Trim();
                int CategoryId = ((Category)cbCategory.SelectedItem).CategoryId;
                if (tbProductName.Text.Trim() != "" && decimal.TryParse(tbUnitPrice.Text, out unitPrice))
                {
                    product.ProductName = ProductName;
                    product.UnitPrice = unitPrice;
                    product.Discontinued = Discontinued;
                    product.QuantityPerUnit = QuantityPerUnit;
                    product.CategoryId = CategoryId;
                    MyRequest<Product> myRequest = new MyRequest<Product>()
                    {
                        RequestType = "Edit",
                        Data = product
                    };
                    SendRequest(JsonSerializer.Serialize(myRequest));
                    MessageBox.Show("Product updated successfully");
                }
                else
                {
                    MyRequest<Product> myRequest = new MyRequest<Product>()
                    {
                        RequestType = "Error",
                        Data = product
                    };
                    SendRequest(JsonSerializer.Serialize(myRequest));
                    MessageBox.Show("Product cannot be updated");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem != null)
            {
                Product product = (Product)dgProducts.SelectedItem;
                if (productService.GetProductById(product.ProductId)!= null)
                {
                    MyRequest<Product> myRequest = new MyRequest<Product>()
                    {
                        RequestType = "Delete",
                        Data = product
                    };
                    SendRequest(JsonSerializer.Serialize(myRequest));
                }
                else
                {
                    MyRequest<Product> myRequest = new MyRequest<Product>()
                    {
                        RequestType = "Error",
                        Data = product
                    };
                    SendRequest(JsonSerializer.Serialize(myRequest));
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product();
            product.ProductName = (String.IsNullOrEmpty(tbProductName.Text.Trim())) ? "" : tbProductName.Text.Trim();
            decimal unitPrice;
            product.Discontinued = (bool)cbDiscontinued.IsChecked;
            product.QuantityPerUnit = tbQuantityPerUnit.Text.Trim();
            product.CategoryId = ((Category)cbCategory.SelectedItem == null) ? null : ((Category)cbCategory.SelectedItem).CategoryId;
            if ((Category)cbCategory.SelectedItem != null && tbProductName.Text.Trim() != "" && decimal.TryParse(tbUnitPrice.Text, out unitPrice))
            {
                product.UnitPrice = unitPrice;
                MyRequest<Product> myRequest = new MyRequest<Product>()
                {
                    RequestType = "Add",
                    Data = product
                };
                SendRequest(JsonSerializer.Serialize(myRequest));
                MessageBox.Show("Product added successfully");
            }
            else
            {
                MyRequest<Product> myRequest = new MyRequest<Product>()
                {
                    RequestType = "Error",
                    Data = product
                };
                SendRequest(JsonSerializer.Serialize(myRequest));
                MessageBox.Show("Product cannot be added");
            }
        }

        private void SendRequest(string request)
        {
            if (request.Equals(string.Empty)) return;
            else
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(request);
                stream.Write(data, 0, data.Length);
                int count = stream.Read(data, 0, data.Length);
                string response = System.Text.Encoding.ASCII.GetString(data, 0, count);
                LoadProducts();
            }
        }
    }
}
