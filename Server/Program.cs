using DataAccess.Models;
using Microsoft.VisualBasic;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using ProductService.Models;
using DataAccess.Logics;
using System.Text;

namespace Server
{
    public class Program
    {
        static int numberOfClient = 0;
        static void ProcessClient(object parameter)
        {
            TcpClient client = (TcpClient)parameter;
            string data;
            int count;
            NetworkStream stream = client.GetStream();
            ProductServices productServices = new ProductServices();

            Byte[] bytes = new Byte[1024];
            try
            {
                while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, count);
                    Console.WriteLine($"Received: {data} at {DateTime.Now} by Client");
                    MyRequest<object> request = JsonSerializer.Deserialize<MyRequest<object>>(data);
                    bool result = false;
                    if (request != null)
                    {
                        MyRequest<Product>? requestProduct = JsonSerializer.Deserialize<MyRequest<Product>>(data);
                        switch (request.RequestType)
                        {
                            case "Add":
                                Product productAdd = requestProduct.Data;
                                productServices.AddProduct(productAdd);
                                result = true;
                                break;
                            case "Edit":
                                Product productEdit = requestProduct.Data;
                                if (productEdit != null)
                                {
                                    productServices.UpdateProduct(productEdit.ProductId, productEdit.ProductName, productEdit.CategoryId, productEdit.UnitPrice, productEdit.Discontinued, productEdit.QuantityPerUnit);
                                }
                                result = true;
                                break;
                            case "Delete":
                                Product productDelete = requestProduct.Data;
                                if(productServices.DeleteProduct(productDelete.ProductId))
                                {
                                    result = true;
                                }
                                result = false;
                                break;
                            case "Error":
                                result = false;
                                break;
                            default: break;
                        }
                        MyResponse<string> response;
                        if (result)
                        {
                            response = new MyResponse<string>()
                            {
                                IsSuccess = true,
                                Data = data
                            };
                        }
                        else
                        {
                            response = new MyResponse<string>()
                            {
                                IsSuccess = false,
                                Data = data
                            };
                        }

                        Byte[] msg = System.Text.Encoding.ASCII.GetBytes(response.ToString());
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine($"Response: Data {response.Data} IsSuccess {response.IsSuccess}");
                    }
                }

            }
            catch (Exception e)
            {
                client.Close();
                Console.WriteLine(e);
            }
            client.Close();

            Console.WriteLine($"Number of client connected: {--numberOfClient}");
        }
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 1500;
            Console.WriteLine("Server App");
            IPAddress localAddr = IPAddress.Parse(host);
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("************************");
            Console.WriteLine("waiting....");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.Write("*************************");
                Console.WriteLine($"Number of client connected: {++numberOfClient}");
                Thread thread = new Thread(new ParameterizedThreadStart(ProcessClient));
                thread.Start(client);
            }
        }
    }
}
