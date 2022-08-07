using System.Text.Json;

namespace TaskOne
{
    public class MainFunction
    {
        public DirectoryWorkClass directoryWorkClass { get; set; }

        public void Start(ref CancellationTokenSource cancelTokenSource)
        {
            MetaData metaData = new MetaData() { invalid_files = new List<string>() };
            while (!cancelTokenSource.Token.IsCancellationRequested)
            {
                DateTime dateTime = DateTime.Now;
                if (dateTime.Hour == 14 && dateTime.Minute == 04 && dateTime.Second == 01 && dateTime.Millisecond == 01)
                {
                    directoryWorkClass.WriteMetaDataToFile(metaData);
                }
                string[] files = Directory.GetFiles(directoryWorkClass.SourceFolder);

                foreach (string filePath in files)
                {
                    string extension = Path.GetExtension(filePath);
                    if (extension != ".txt" && extension != ".csv")
                    {
                        Directory.CreateDirectory(directoryWorkClass.SourceFolder + "\\invalid\\");
                        string newPath = directoryWorkClass.SourceFolder + "\\invalid\\" + Path.GetFileName(filePath);
                        try
                        {
                            File.Move(filePath, newPath);
                            metaData.invalid_files.Add(filePath);
                        }
                        catch
                        {
                            continue;
                        }

                        continue;
                    }

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
                                        mas.ForEach(x => { x.Trim(); });

                                        var temp = mas.Skip(2).Where(x => x == "").FirstOrDefault();
                                        if (temp != null)
                                            throw new Exception();

                                        string name = mas[0] != "" ? mas[0] : mas[1];
                                        if (name == "")
                                            throw new Exception();

                                        Payer payer = new Payer() { Name = name, Account_number = Convert.ToInt64(mas[7]), date = mas[6], Payment = Convert.ToDecimal(mas[5].Replace(".", ",")) };

                                        var city = listObject.FirstOrDefault(x => x.City == mas[2]);
                                        if (city != null)
                                        {
                                            var service = city.Services.FirstOrDefault(x => x.Name == mas[8]);
                                            if (service != null)
                                            {
                                                service.Payers.Add(payer);
                                                service.Total += payer.Payment;
                                                service.Total = service.Payers.Sum(x => x.Payment);
                                            }
                                            else
                                            {
                                                Service serviceNew = new Service() { Name = mas[8], Total = payer.Payment, Payers = new List<Payer>() };
                                                serviceNew.Payers.Add(payer);
                                                city.Services.Add(serviceNew);
                                            }
                                            city.Total = city.Services.Sum(x => x.Total);
                                        }
                                        else
                                        {
                                            PaymentTransaction paymentTransaction = new PaymentTransaction() { City = mas[2], Total = payer.Payment, Services = new List<Service>() };

                                            Service service = new Service() { Name = mas[8], Payers = new List<Payer>() };
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
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}
