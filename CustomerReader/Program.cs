using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Configuration;

namespace CustomerReader {
  class CustomerReader {

    static string FileXML, FileCSV, FileJSON;
    static bool isFileXML = false, isFileCSV = false, isFileJSON = false;

    static void Main(string[] args) {

      FileXML = ConfigurationManager.AppSettings["FileXML"];
      FileCSV = ConfigurationManager.AppSettings["FileCSV"];
      FileJSON = ConfigurationManager.AppSettings["FileJSON"];

      CustomerReader cr = new CustomerReader();

      CheckFiles();
      if (isFileCSV) {
        cr.readCustomersCsv(FileCSV);
      }
      else {
        Console.WriteLine("Info: no feeds from file: " + FileCSV);
      }
      if (isFileXML) {
        cr.readCustomersXml(FileXML);
      }
      else {
        Console.WriteLine("Info: no feeds from file: " + FileXML);
      }
      if (isFileJSON) {
        cr.readCustomersJson(FileJSON);
      }
      else {
        Console.WriteLine("Info: no feeds from file: " + FileJSON);
      }


      Console.WriteLine("Added this many customers: " + cr.getCustomers().Count + "\n");
      cr.displayCustomers();
      Console.ReadLine();
    }

    //private String filePath;
    private List<Customer> customers;

    static void CheckFiles() {
      if (File.Exists(FileXML)) {
        isFileXML = true;
      }
      if (File.Exists(FileCSV)) {
        isFileCSV = true;
      }
      if (File.Exists(FileJSON)) {
        isFileJSON = true;
      }
    }

    public CustomerReader() {
      customers = new List<Customer>();
    }

    public List<Customer> getCustomers() {
      return customers;
    }

    /*
     * This method reads customers from a CSV file and puts them into the customers list.
     */
    public void readCustomersCsv(String customer_file_path) {

      try {
        StreamReader br = new StreamReader(File.Open(customer_file_path, FileMode.Open));
        String line = br.ReadLine();

        while (line != null) {
          String[] attributes = line.Split(',');

          // Skip Header
          if (attributes[0].ToLower() == "email") {
            line = br.ReadLine();
            continue;
          }

          Customer customer = new Customer();
          customer.email = attributes[0].Trim().ToLower();
          customer.fn = FirstCapital(attributes[1]);
          customer.ln = FirstCapital(attributes[2]);
          customer.phone = attributes[3].Trim();
          customer.streetAddress = EachFirstCapital(attributes[4]);
          customer.city = FirstCapital(attributes[5]);
          customer.state = attributes[6].ToUpper();
          customer.zipCode = attributes[7];

          customers.Add(customer);
          line = br.ReadLine();
        }
      }
      catch (IOException ex) {
        Console.Write("OH NO!!!!");
        Console.Write(ex.StackTrace);
      }
    }

    public void readCustomersXml(String customerFilePath) {
      try {
        var doc = new XmlDocument();
        doc.Load(customerFilePath);

        XmlNodeList nList = doc.GetElementsByTagName("Customer");  

        for (int temp = 0; temp < nList.Count; temp++) {
          XmlNode nNode = nList[temp];
          Console.WriteLine("\nCurrent Element :" + nNode.Name);
          if (nNode.NodeType == XmlNodeType.Element) {
            Customer customer = new Customer();
            XmlElement eElement = (XmlElement)nNode;

            customer.email = eElement.GetElementsByTagName("Email").Item(0).InnerText.ToLower();
            customer.fn = FirstCapital(eElement.GetElementsByTagName("FirstName").Item(0).InnerText);
            customer.ln = FirstCapital(eElement.GetElementsByTagName("LastName").Item(0).InnerText);
            customer.phone = eElement.GetElementsByTagName("PhoneNumber").Item(0).InnerText.Trim();

            XmlElement aElement = (XmlElement)eElement.GetElementsByTagName("Address").Item(0);

            customer.streetAddress = EachFirstCapital(aElement.GetElementsByTagName("StreetAddress").Item(0).InnerText);
            customer.city = FirstCapital(aElement.GetElementsByTagName("City").Item(0).InnerText);
            customer.state = aElement.GetElementsByTagName("State").Item(0).InnerText.ToUpper();
            customer.zipCode = aElement.GetElementsByTagName("ZipCode").Item(0).InnerText;

            customers.Add(customer);
          }
        }
      }
      catch (Exception e) {
        Console.Write(e.StackTrace);
      }
    }


    public void readCustomersJson(String customersFilePath) {
      //JsonTextReader reader = (new JsonTextReader(System.IO.File.OpenText(customersFilePath));
      //JObject reader = JObject.Parse(File.ReadAllText(customersFilePath));
      JsonTextReader reader = new JsonTextReader(System.IO.File.OpenText(customersFilePath));

      try {
        JArray obj = (JArray)JToken.ReadFrom(reader);
        //JArray a = (JArray)reader);
        //

        foreach (JObject o in obj) {
          Customer customer = new Customer();
          JObject record = (JObject)o;

          String email = (String)record["Email"];
          customer.email = email.ToLower();

          String firstName = (String)record["FirstName"];
          customer.fn = FirstCapital(firstName);

          String lastName = (String)record["LastName"];
          customer.ln = FirstCapital(lastName);

          String phone = (String)record["PhoneNumber"];
          customer.phone = phone.Trim();

          JObject address = (JObject)record["Address"];

          String streetAddress = (String)address["StreetAddress"];
          customer.streetAddress = EachFirstCapital(streetAddress);

          String city = (String)address["City"];
          customer.city = FirstCapital(city);

          String state = (String)address["State"];
          customer.state = state.ToUpper();

          String zipCode = (String)address["ZipCode"];
          customer.zipCode = zipCode;

          customers.Add(customer);
        }
      }
      catch (FileNotFoundException e) {
        Console.Write(e.StackTrace);
      }
      catch (IOException e) {
        Console.Write(e.StackTrace);
      }
      catch (JsonException e) {
        Console.Write(e.StackTrace);
      }
    }

    /// <summary>
    /// Capitalizes first letter
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string FirstCapital(string input) {
      string returnedValue = string.Empty, tempString = string.Empty;
      if (String.IsNullOrEmpty(input)) {
        return returnedValue;
      }
      tempString = input.Trim();
      returnedValue = tempString.Substring(0, 1).ToUpper() + tempString.Substring(1).ToLower();
      return returnedValue;
    }

    /// <summary>
    /// Capitalizes first letter of each word
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string EachFirstCapital(string input) {
      string returnedValue = string.Empty, tempString = string.Empty;
      String[] words;

      if (String.IsNullOrEmpty(input)) {
        return returnedValue;
      }

      StringBuilder sb = new StringBuilder();
      tempString = input.Trim().ToLower();
      words = tempString.Split(' ');

      for (int i = 0; i < words.Length; i++) {
        if (i > 0) {
          sb.Append(" ");
        }
        sb.Append(words[i].Substring(0, 1).ToUpper());
        sb.Append(words[i].Substring(1).ToLower());
      }
      returnedValue = sb.ToString();
      return returnedValue;
    }

    public void displayCustomers() {
      foreach (Customer customer in this.customers) {
        String customerString = "";
        customerString += "Email: " + customer.email + "\n";
        customerString += "First Name: " + customer.fn + "\n";
        customerString += "Last Name: " + customer.ln + "\n";
        customerString += "Phone Number: " + customer.phone + "\n";
        customerString += "Street Address: " + customer.streetAddress + "\n";
        customerString += "City: " + customer.city + "\n";
        customerString += "State: " + customer.state + "\n";
        customerString += "Zip Code: " + customer.zipCode + "\n";

        Console.WriteLine(customerString);
      }
    }
  }
}