# PhoneForge

Sistema web para gerenciar celulares de marcas diferentes, construído em **C# / .NET 9** com foco em **Programação Orientada a Objetos**.

Cada marca nasce do mesmo molde, a classe abstrata `Smartphone`, e define o próprio comportamento onde ele realmente muda: a instalação de aplicativos.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)

## Funcionalidades

- Cadastrar celulares **Nokia** ou **Iphone** (modelo, número, IMEI e memória)
- Ligar e receber ligações
- Instalar aplicativos, cada marca pela sua loja (Nokia Store ou App Store)
- Bloquear a instalação duplicada de um mesmo aplicativo
- Validar os dados de cadastro (IMEI com 15 dígitos, memória maior que zero, marca válida)
- Remover celulares
- Registro das atividades na tela

## Arquitetura

- **Backend**: API REST com ASP.NET Core Minimal API
- **Frontend**: HTML, CSS e JavaScript puro, servido pelo próprio backend
- **Dados**: repositório em memória (os dados reiniciam junto com o servidor)

```
Models/        domínio: Smartphone (abstrata), Nokia, Iphone
Services/      repositório em memória
Program.cs     endpoints da API e validações
wwwroot/       frontend (index.html, style.css, app.js)
```

## Modelagem

```
Smartphone (abstrata)
├── + Numero
├── - Modelo, - IMEI, - Memoria
├── Ligar()
├── ReceberLigacao()
└── InstalarAplicativo(nome)  ← abstrato
    ├── Nokia   → instala pela Nokia Store
    └── Iphone  → instala pela App Store
```

## Conceitos de POO aplicados

- **Abstração**: `Smartphone` é abstrata e não pode ser instanciada diretamente.
- **Herança**: `Nokia` e `Iphone` herdam propriedades e métodos de `Smartphone`.
- **Polimorfismo**: cada marca sobrescreve (`override`) `InstalarAplicativo` do seu jeito, e a API trabalha com o tipo abstrato `Smartphone`.
- **Encapsulamento**: `Modelo`, `IMEI` e `Memoria` são privados e só são lidos pelo método `ObterInformacoes()`.

## Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/smartphones` | Lista os celulares |
| POST | `/api/smartphones` | Cadastra um celular |
| DELETE | `/api/smartphones/{id}` | Remove um celular |
| POST | `/api/smartphones/{id}/ligar` | Faz uma ligação |
| POST | `/api/smartphones/{id}/receber-ligacao` | Recebe uma ligação |
| POST | `/api/smartphones/{id}/aplicativos` | Instala um aplicativo |

Exemplo de cadastro:

```json
{ "marca": "Nokia", "numero": "11 91234-5678", "modelo": "Nokia 3310", "imei": "111111111111111", "memoria": 64 }
```

## Como executar

Pré-requisito: [.NET 9 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/barbozadevti/phoneforge-dotnet.git
cd phoneforge-dotnet
dotnet run
```

Depois abra no navegador o endereço que aparecer no terminal (por exemplo, `http://localhost:5000`).

## Versão de console

A primeira versão do projeto, um programa de console, está no ramo [`console`](https://github.com/barbozadevti/phoneforge-dotnet/tree/console).

---

Projeto desenvolvido durante a trilha .NET da [DIO](https://www.dio.me/).
