using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MarketManagement.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketManagement
{
    public class OrderManager
    {
        private readonly string _ordersJsonPath;

        public OrderManager(string ordersJsonPath)
        {
            _ordersJsonPath = ordersJsonPath;
        }

        public DataTable GetAllOrders()
        {
            ValidateJsonFile(out string jsonData);
            DataTable dt = CreateOrdersDataTable();

            if (!LoadOrdersDataToTable(jsonData, dt) || dt.Rows.Count == 0)
            {
                throw new InvalidDataException("No orders found or data format is not recognized.");
            }

            return dt;
        }

        public DataTable SearchOrders(string fieldName, string searchValue)
        {
            ValidateJsonFile(out string jsonData);
            DataTable dt = CreateOrdersDataTable();

            try
            {
                JObject jsonObject = JObject.Parse(jsonData);
                JArray ordersArray = (JArray)jsonObject["Orders"];

                if (ordersArray != null)
                {
                    ProcessOrdersArray(ordersArray, dt, fieldName, searchValue);
                }
            }
            catch (Exception ex)
            {
                throw new JsonException($"Error parsing orders data: {ex.Message}", ex);
            }

            return dt;
        }

        private void ValidateJsonFile(out string jsonData)
        {
            if (!File.Exists(_ordersJsonPath))
            {
                throw new FileNotFoundException($"Orders file not found. Checked path: {_ordersJsonPath}");
            }

            jsonData = File.ReadAllText(_ordersJsonPath);

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                throw new InvalidDataException("Orders file is empty");
            }
        }

        private void ProcessOrdersArray(JArray ordersArray, DataTable dt, string fieldName, string searchValue)
        {
            for (int i = 0; i < ordersArray.Count; i++)
            {
                JToken orderToken = ordersArray[i];
                string fieldValue = orderToken[fieldName]?.ToString() ?? "";

                if (!string.IsNullOrEmpty(fieldValue) &&
                    string.Compare(fieldValue, searchValue, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AddOrderToDataTable(orderToken, dt);
                }
            }
        }

        private DataTable CreateOrdersDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InvoiceDate", typeof(string));
            dt.Columns.Add("InvoiceNo", typeof(string));
            dt.Columns.Add("CustomerId", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("Contact", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("GrandTotal", typeof(decimal));
            return dt;
        }

        private void AddOrderToDataTable(JToken orderToken, DataTable dt)
        {
            DataRow row = dt.NewRow();

            row["InvoiceDate"] = orderToken["InvoiceDate"]?.ToString() ?? "";
            row["InvoiceNo"] = orderToken["InvoiceNo"]?.ToString() ?? "";
            row["CustomerId"] = orderToken["CustomerId"]?.ToString() ?? "";
            row["CustomerName"] = orderToken["CustomerName"]?.ToString() ?? "";
            row["Contact"] = orderToken["Contact"]?.ToString() ?? "";
            row["Address"] = orderToken["Address"]?.ToString() ?? "";

            decimal grandTotal = 0;
            decimal.TryParse(orderToken["GrandTotal"]?.ToString(), out grandTotal);
            row["GrandTotal"] = grandTotal;

            dt.Rows.Add(row);
        }

        private bool LoadOrdersDataToTable(string jsonData, DataTable dt)
        {
            bool dataLoaded = false;

            try
            {
                dynamic fullData = JsonConvert.DeserializeObject(jsonData);
                foreach (JProperty prop in fullData)
                {
                    if (prop.Value is JArray array)
                    {
                        for (int i = 0; i < array.Count; i++)
                        {
                            if (array[i]["InvoiceNo"] != null || array[i]["CustomerName"] != null)
                            {
                                AddOrderToDataTable(array[i], dt);
                                dataLoaded = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading JSON data: " + ex.Message);
            }

            return dataLoaded;
        }
    }
}