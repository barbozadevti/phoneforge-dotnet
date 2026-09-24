using DesafioPOO.Models;

Console.WriteLine("Smartphone Nokia:");
Smartphone nokia = new Nokia(numero: "11 91234-5678", modelo: "Nokia 3310", imei: "111111111111111", memoria: 64);
nokia.ExibirInformacoes();
nokia.Ligar();
nokia.ReceberLigacao();
nokia.InstalarAplicativo("WhatsApp");

Console.WriteLine();

Console.WriteLine("Smartphone iPhone:");
Smartphone iphone = new Iphone(numero: "21 99876-5432", modelo: "iPhone 15", imei: "222222222222222", memoria: 128);
iphone.ExibirInformacoes();
iphone.Ligar();
iphone.ReceberLigacao();
iphone.InstalarAplicativo("Telegram");
