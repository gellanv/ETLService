using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TaskOne;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json").Build();

DirectoryWorkClass directoryWorkClass = new DirectoryWorkClass()
{
    SourceFolder = configuration.GetSection("path:folderA").Value,
    ResultFolder = configuration.GetSection("path:folderB").Value,
    SearchFiles = new string[2] { "*.csv", "*.txt" }
};

MetaData metaData = new MetaData() { invalid_files = new List<string>() };
while (true)
{
    DateTime dateTime = DateTime.Now;
    if (dateTime.Hour == 23 && dateTime.Minute == 59 && dateTime.Second == 59&& dateTime.Millisecond == 01)
    {
        directoryWorkClass.WriteMetaDataToFile(metaData);
    }
    foreach (string filePath in directoryWorkClass.SearchFiles
                            .AsParallel()
                            .SelectMany(SearchPattern => Directory.EnumerateFiles(directoryWorkClass.SourceFolder, SearchPattern)))
    {
        List<PaymentTransaction> listObject = new List<PaymentTransaction>();
        try
        {
            foreach (List<string> LinesPart in directoryWorkClass.ReadFile(filePath))
            {
                LinesPart.ForEach(x =>
                {
                    try
                    {
                        x = x.Replace("“", "").Replace("\"", "").Replace("”", "");
                        List<string> mas = x.Split(new char[] { ',' }).ToList<string>();

                        Payer payer = new Payer() { Name = mas[0].Trim(), Account_number = Convert.ToInt64(mas[7].Trim()), date = mas[6].Trim(), Payment = Convert.ToDecimal(mas[5].Trim().Replace(".", ",")) };

                        var city = listObject.FirstOrDefault(x => x.City == mas[2].Trim());
                        if (city != null)
                        {
                            var service = city.Services.FirstOrDefault(x => x.Name == mas[8].Trim());
                            if (service != null)
                            {
                                service.Payers.Add(payer);
                                service.Total += payer.Payment;
                                service.Total = service.Payers.Sum(x => x.Payment);
                            }
                            else
                            {
                                Service serviceNew = new Service() { Name = mas[8].Trim(), Total = payer.Payment, Payers = new List<Payer>() };
                                serviceNew.Payers.Add(payer);
                                city.Services.Add(serviceNew);
                            }
                            city.Total = city.Services.Sum(x => x.Total);
                        }
                        else
                        {
                            PaymentTransaction paymentTransaction = new PaymentTransaction() { City = mas[2].Trim(), Total = payer.Payment, Services = new List<Service>() };

                            Service service = new Service() { Name = mas[8].Trim(), Payers = new List<Payer>() };
                            service.Payers.Add(payer);
                            service.Total = payer.Payment;

                            paymentTransaction.Services.Add(service);
                            listObject.Add(paymentTransaction);
                        }
                        metaData.parsed_lines++;
                    }
                    catch
                    {
                        metaData.found_errors++;
                    }

                });
            }

            string jsonString = JsonSerializer.Serialize(listObject);
            string pathNewFile = directoryWorkClass.CreateResultFileName();

            directoryWorkClass.WriteDataToFile(filePath, pathNewFile, jsonString);

            metaData.parsed_files++;
            metaData.invalid_files.Add(filePath);
        }
        catch
        {
            continue;
        }
    }
}