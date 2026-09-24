# Desafio POO em .NET — Smartphones

Projeto do desafio da trilha .NET da [DIO](https://www.dio.me/), com base no repositório [trilha-net-poo-desafio](https://github.com/digitalinnovationone/trilha-net-poo-desafio).

## Objetivo

Modelar um sistema de celulares usando Programação Orientada a Objetos: uma abstração de smartphone que permite que diferentes marcas e modelos tenham comportamento próprio, reaproveitando código.

Além do que o desafio pede, o projeto virou uma aplicação web completa:

- **Backend**: API REST em ASP.NET Core (Minimal API).
- **Frontend**: página em HTML, CSS e JavaScript puro, servida pelo próprio backend.

Na página é possível cadastrar celulares Nokia ou Iphone, ligar, receber ligação, instalar aplicativos e remover celulares.

## Estrutura

```
Smartphone (abstrata)
├── Numero, Modelo, IMEI, Memoria
├── Ligar()
├── ReceberLigacao()
└── InstalarAplicativo(nome)  ← abstrato
    ├── Nokia   → instala pela Nokia Store
    └── Iphone  → instala pela App Store
```

```
Models/        classes do domínio (Smartphone, Nokia, Iphone)
Services/      repositório em memória
Program.cs     endpoints da API
wwwroot/       frontend (index.html, style.css, app.js)
```

## Conceitos aplicados

- **Abstração**: `Smartphone` é abstrata e não pode ser instanciada diretamente.
- **Herança**: `Nokia` e `Iphone` herdam propriedades e métodos de `Smartphone`.
- **Polimorfismo**: cada marca sobrescreve (`override`) `InstalarAplicativo` do seu jeito.
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

Os dados ficam em memória, então somem quando o servidor reinicia.

## Como executar

```bash
dotnet run
```

Depois abra no navegador o endereço que aparecer no terminal (por exemplo, `http://localhost:5000`).
